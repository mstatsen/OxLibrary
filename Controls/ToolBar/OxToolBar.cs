using OxLibrary.Controls.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxToolBar<TButton> : OxPanel
        where TButton : OxClickFrame, new()
    {
        private readonly OxWidth DefaultToolBarHeight = OxWh.W24;
        private OxClickFrameList<TButton> buttons = new();

        public OxToolBar() : base() =>
            UseDisabledStyles = false;

        public readonly Dictionary<OxToolbarAction, TButton> Actions = new();
        public OxActionClick<OxToolbarAction>? ToolbarActionClick;

        private void RecalcWidth()
        {
            if (!OxDockHelper.IsVariableWidth(Dock))
                return;

            OxWidth calcedWidth = OxWh.W0;
            calcedWidth = OxWh.A(calcedWidth, Margin.Horizontal);
            calcedWidth = OxWh.A(calcedWidth, buttons.Width);

            foreach (OxPanel separator in Separators.Values)
                calcedWidth = 
                    OxWh.A(
                        OxWh.A(calcedWidth, separator.Width),
                        separator.Margin.Horizontal
                    );

            Width = calcedWidth;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetToolBarPaddings();
            Size = new(Width, DefaultToolBarHeight);
        }

        private readonly Dictionary<TButton, OxPanel> Separators = new();
        protected virtual OxWidth ButtonMargin => OxWh.W1;
        protected virtual OxWidth GroupsSeparatorWidth => OxWh.W1;
        protected virtual OxWidth GroupSeparatorMargin => OxWh.W2;

        protected bool PlacingButtons { get; private set; } = false;

        private void PlaceButtons()
        {
            if (PlacingButtons)
                return;

            PlacingButtons = true;
            SuspendLayout();

            try
            {
                foreach (TButton button in buttons)
                {
                    button.VisibleChanged -= ButtonVisibleChangedHandler;
                    button.Parent = null;
                }

                TButton? lastButton = null;

                foreach (TButton button in buttons)
                {
                    button.FixedBorderColor = false;

                    if (OxDockHelper.Variable(button.Dock) is not OxDockVariable.Width)
                        button.Dock = OxDock.Left;

                    TButton? beginGroupButton = button.Dock is OxDock.Left ? button : lastButton;

                    if (beginGroupButton is not null
                        && beginGroupButton.BeginGroup
                        && beginGroupButton.Dock.Equals(button.Dock))
                        CreateSeparator(button);
                    else
                        button.Margin.SetSize(
                            button.Dock,
                            lastButton is null
                            || !lastButton.Dock.Equals(button.Dock)
                                ? OxWh.W0
                                : ButtonMargin
                        );

                    lastButton = button;
                }

                SetToolBarPaddings();
                RecalcWidth();

                foreach (TButton button in buttons)
                {
                    button.VisibleChanged += ButtonVisibleChangedHandler;
                    button.Parent = this;
                }
            }
            finally
            {
                PlacingButtons = false;
                ResumeLayout();
            }
        }

        private OxPanel CreateSeparator(TButton startButton)
        {
            if (!Separators.TryGetValue(startButton, out var separator))
            {
                separator = new(new(GroupsSeparatorWidth, OxWh.W0))
                {
                    Dock = startButton.Dock,
                    Parent = startButton.Parent,
                };
                separator.Margin.Size = GroupSeparatorMargin;
                separator.Margin.Top = startButton.Margin.Top;
                separator.Margin.Bottom = startButton.Margin.Bottom;
                startButton.ParentChanged += SynchronizeSeparatorParentHandler;
                Separators.Add(startButton, separator);
            }

            return separator;
        }

        private void SynchronizeSeparatorParentHandler(object? sender, EventArgs e)
        {
            if (sender is TButton button
                && Separators.TryGetValue(button, out var separator))
                separator.Parent = button.Parent;
        }

        private void ButtonVisibleChangedHandler(object? sender, EventArgs e) =>
            RecalcWidth();

        private void ClearButtons() 
        {
            foreach (TButton button in buttons)
            {
                button.VisibleChanged -= ButtonVisibleChangedHandler;
                button.Parent = null;
            }
        }

        protected void SetButtons(OxClickFrameList<TButton> buttonList)
        {
            ClearButtons();
            buttons = buttonList;
            PlaceButtons();
        }

        protected virtual void SetToolBarPaddings()
        {
            Padding.Left = OxWh.W0;
            Padding.Right = OxWh.W1;
            Padding.Top = OxWh.W2;
            Padding.Bottom = OxWh.W4;

            if (buttons.Last?.Height > Height)
            {
                Padding.Top = OxWh.W1;
                Padding.Bottom = OxWh.W1;
            }
        }

        public OxClickFrameList<TButton> Buttons
        {
            get => buttons;
            set => SetButtons(value);
        }

        public override void PrepareColors()
        {
            base.PrepareColors();

            foreach (TButton button in buttons)
                button.BaseColor = BaseColor;

            foreach(OxPanel separator in Separators.Values)
                separator.BaseColor = Colors.Darker(7);
        }

        protected override void OnEnabledChanged(EventArgs e)
        { 
            base.OnEnabledChanged(e);

            foreach (TButton button in buttons)
                button.Enabled = Enabled;
        }

        public bool ExecuteDefault() =>
            Buttons.ExecuteDefault();

        /*
        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!Initialized
                || !e.Changed
                || !base.OnSizeChanged(e))
                return false;

            PlaceButtons();
            return true;
        }
        */

        public bool AllowEditingActions
        {
            get
            {
                foreach (var action in Actions)
                    if (OxToolbarActionHelper.ActionForExistItems(action.Key))
                        return action.Value.Enabled;

                return true;
            }
            set
            {
                foreach (var action in Actions)
                    if (OxToolbarActionHelper.ActionForExistItems(action.Key))
                        action.Value.Enabled = value;
            }
        }

        public TButton AddButton(OxToolbarAction action, bool beginGroup = false,
            OxDock dockStyle = OxDock.Left)
        {
            TButton button = new()
            {
                Text = OxToolbarActionHelper.Text(action),
                Icon = OxToolbarActionHelper.Icon(action),
                BeginGroup = beginGroup,
                Dock = dockStyle
            };
            button.Click += (s, e) => ToolbarActionClick?.Invoke(s,
                new OxActionEventArgs<OxToolbarAction>(
                    s is null
                        ? OxToolbarAction.Empty
                        : GetActionByButton((OxButton)s)
                )
            );
            button.Size = new(OxToolbarActionHelper.Width(action), button.Height);
            Actions.Add(action, button);
            Buttons.Add(button);
            PlaceButtons();
            return button;
        }

        public TButton AddButton(TButton button, bool? beginGroup = null, bool InsertAsFirst = false)
        {
            if (!Buttons.Contains(button))
            {
                if (InsertAsFirst)
                    Buttons.Insert(0, button);
                else
                    Buttons.Add(button);
            }

            if (beginGroup is bool boolBeginGroup)
                button.BeginGroup = boolBeginGroup;

            PlaceButtons();
            button.SizeChanged += ButtonSizeChangedHandler;
            return button;
        }

        private void ButtonSizeChangedHandler(object sender, OxSizeChangedEventArgs args) =>
            PlaceButtons();

        private OxToolbarAction GetActionByButton(OxButton button)
        {
            foreach (var item in Actions)
                if (item.Value.Equals(button))
                    return item.Key;

            return OxToolbarAction.Empty;
        }
    }
}