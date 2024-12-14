using OxLibrary.Handlers;
using OxLibrary.ControlList;
using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxToolBar<TButton> : OxPanel
        where TButton : OxClickFrame, new()
    {
        private readonly short DefaultToolBarHeight = 24;
        private OxClickFrameList<TButton> buttons = new();

        public OxToolBar() : base() =>
            UseDisabledStyles = false;

        public readonly Dictionary<OxToolbarAction, TButton> Actions = new();
        public OxActionClick<OxToolbarAction>? ToolbarActionClick;

        private void RecalcWidth()
        {
            if (!OxDockHelper.IsVariableWidth(Dock))
                return;

            short calcedWidth = 0;
            calcedWidth += Margin.Horizontal;
            calcedWidth += buttons.VisibleWidth;

            foreach (OxPanel separator in Separators.Values)
            {
                if (!separator.IsVisible)
                    continue;

                calcedWidth += separator.Width;
                calcedWidth += separator.Margin.Horizontal;
            }

            Width = calcedWidth;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetToolBarPaddings();
            Size = new(Width, DefaultToolBarHeight);
        }

        private readonly Dictionary<TButton, OxPanel> Separators = new();
        protected virtual short ButtonMargin => 1;
        protected virtual short GroupsSeparatorWidth => 1;
        protected virtual short GroupSeparatorMargin => 2;
        public override OxBool HandleParentPadding => OxB.T;

        protected bool PlacingButtons { get; private set; } = false;

        internal void PlaceButtons()
        {
            if (PlacingButtons)
                return;

            PlacingButtons = true;
            SuspendLayout();

            try
            {
                foreach (TButton button in buttons)
                    button.Parent = null;

                PrepareButtonsSizes();

                TButton? lastButton = null;

                foreach (TButton button in buttons)
                {
                    if (!button.IsVisible)
                    {
                        HideSeparator(button);
                        continue;
                    }

                    button.FixedBorderColor = false;

                    if (OxDockHelper.Variable(button.Dock) is not OxDockVariable.Width)
                        button.Dock = OxDock.Left;

                    TButton? beginGroupButton =
                        button.Dock is OxDock.Left
                            ? button
                            : lastButton;

                    if (beginGroupButton is not null
                        && beginGroupButton.BeginGroup
                        && beginGroupButton.Dock.Equals(button.Dock))
                        CreateSeparator(button);
                    else
                        button.Margin.SetSize(
                            button.Dock,
                            lastButton is not null
                                && lastButton.Dock.Equals(button.Dock)
                                ? ButtonMargin
                                : 0
                        );

                    lastButton = button;
                }

                SetToolBarPaddings();
                RecalcWidth();

                foreach (TButton button in buttons)
                    button.Parent = this;
            }
            finally
            {
                PlacingButtons = false;
                ResumeLayout();
            }
        }

        protected virtual void PrepareButtonsSizes() { }

        private void HideSeparator(TButton startButton)
        {
            if (Separators.TryGetValue(startButton, out var separator))
                separator.Visible = OxB.F;
        }

        private OxPanel CreateSeparator(TButton startButton)
        {
            if (!Separators.TryGetValue(startButton, out var separator))
            {
                separator = new(new(GroupsSeparatorWidth, 0))
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

            separator.Visible = OxB.T;
            return separator;
        }

        private void SynchronizeSeparatorParentHandler(object? sender, EventArgs e)
        {
            if (sender is TButton button
                && Separators.TryGetValue(button, out var separator))
                separator.Parent = button.Parent;
        }

        private void ButtonVisibleChangedHandler(object? sender, EventArgs e) =>
            PlaceButtons();

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
            Padding.Left = 0;
            Padding.Right = 1;
            Padding.Top = 2;
            Padding.Bottom = 4;

            if (buttons.Last?.Height > Height)
            {
                Padding.Top = 1;
                Padding.Bottom = 1;
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

        public override void OnEnabledChanged(OxBoolChangedEventArgs e)
        { 
            base.OnEnabledChanged(e);

            foreach (TButton button in buttons)
                button.Enabled = Enabled;
        }

        public bool ExecuteDefault() =>
            Buttons.ExecuteDefault();

        public bool IsAllowEditingActions =>
            OxB.B(AllowEditingActions);

        public void SetAllowEditingActions(bool value) =>
            AllowEditingActions = OxB.B(value);

        public OxBool AllowEditingActions
        {
            get
            {
                foreach (var action in Actions)
                    if (OxToolbarActionHelper.ActionForExistItems(action.Key))
                        return action.Value.Enabled;

                return OxB.T;
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
            button.Click += (s, e) => ToolbarActionClick?.Invoke(s!,
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

        public void RemoveButton(TButton button)
        {
            button.VisibleChanged -= ButtonVisibleChangedHandler;
            button.Parent = null;
            Buttons.Remove(button);
        }

        public TButton AddButton(TButton button, bool? beginGroup = null, bool InsertAsFirst = false)
        {
            if (!Buttons.Contains(button))
            {
                if (InsertAsFirst)
                    Buttons.Insert(0, button);
                else
                    Buttons.Add(button);

                button.VisibleChanged += ButtonVisibleChangedHandler;
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