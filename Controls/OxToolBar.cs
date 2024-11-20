using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxToolBar<TButton> : OxFrame
        where TButton : OxClickFrame, new()
    {
        private const int DefaultToolBarHeight = 28;
        private OxClickFrameList<TButton> buttons = new();

        public OxToolBar() : base() => 
            UseDisabledStyles = false;

        public readonly Dictionary<OxToolbarAction, TButton> Actions = new();
        public OxActionClick<OxToolbarAction>? ToolbarActionClick;

        public void RecalcWidth() => 
            Width = buttons.Width();

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetToolBarPaddings();
            Size = new(Width, DefaultToolBarHeight);
        }

        private readonly Dictionary<TButton, OxPane> separators = new();

        private void PlaceButtons()
        {
            TButton? lastButton = null;

            foreach (TButton button in buttons)
            {
                button.Parent = this;
                button.Dock = 
                    button.Dock is DockStyle.Right 
                        ? DockStyle.Right 
                        : DockStyle.Left;

                if (button.BeginGroup && 
                    lastButton is not null)
                {
                    button.Margin.Left = OxSize.S;
                    lastButton.Margin.Right = OxSize.S;

                    if (lastButton.Dock.Equals(button.Dock))
                        SeparateButtonsGroup(button);
                }
                else
                    button.Margin.Left = 
                        lastButton is null 
                            ? OxSize.None 
                            : OxSize.XXS;

                button.BringToFront();
                button.VisibleChanged += ButtonVisibleChangedHandler;
                lastButton = button;
            }

            ReAlign();
            RecalcWidth();
            SetToolBarPaddings();
        }

        private void SeparateButtonsGroup(TButton startButton)
        {
            if (!separators.TryGetValue(startButton, out var separator))
            {
                separator = new OxPane(new(1, 1))
                {
                    Parent = this,
                    Dock =
                        startButton.Dock.Equals(DockStyle.Right)
                            ? DockStyle.Right
                            : DockStyle.Left,
                    BackColor = BorderColor
                };
                separators.Add(startButton, separator);
            }

            separator?.BringToFront();
        }

        private void ButtonVisibleChangedHandler(object? sender, EventArgs e)
        {
            RecalcWidth();
            OnButtonVisibleChange?.Invoke(sender, e);
        }

        private void ClearButtons() 
        {
            foreach (TButton button in buttons)
            {
                button.VisibleChanged -= ButtonVisibleChangedHandler;
                button.Parent = null;
            }
        }

        private void SetButtons(OxClickFrameList<TButton> buttonList)
        {
            ClearButtons();
            buttons = buttonList;
            PlaceButtons();
        }

        protected virtual void SetToolBarPaddings()
        {
            Padding.Left = OxSize.None;
            Padding.Right = OxSize.XXS;
            Padding.Top = OxSize.XS;
            Padding.Bottom = OxSize.S;

            if (buttons.Last?.CalcedHeight > CalcedHeight)
            {
                Padding.Top = OxSize.XXS;
                Padding.Bottom = OxSize.XXS;
            }
        }

        public OxClickFrameList<TButton> Buttons
        {
            get => buttons;
            set => SetButtons(value);
        }

        public EventHandler? OnButtonVisibleChange;

        protected override void PrepareColors()
        {
            base.PrepareColors();

            foreach (TButton button in buttons)
                button.BaseColor = BaseColor;
        }

        protected override void OnEnabledChanged(EventArgs e)
        { 
            base.OnEnabledChanged(e);

            foreach (TButton button in buttons)
                button.Enabled = Enabled;
        }

        public override Color GetBordersColor() => 
            Colors.Lighter(4);

        public bool ExecuteDefault() =>
            Buttons.ExecuteDefault();

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetToolBarPaddings();
        }

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
            DockStyle dockStyle = DockStyle.Left)
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

        public TButton AddButton(TButton button)
        {
            Buttons.Insert(0, button);
            PlaceButtons();
            return button;
        }

        private OxToolbarAction GetActionByButton(OxButton button)
        {
            foreach (var item in Actions)
                if (item.Value.Equals(button))
                    return item.Key;

            return OxToolbarAction.Empty;
        }
    }

    public class OxToolBar : OxToolBar<OxClickFrame>
    { 
    }
}