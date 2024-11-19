using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxToolBar : OxFrame
    {
        private const int DefaultToolBarHeight = 28;
        private OxClickFrameList buttons = new();

        public OxToolBar() : base() => 
            UseDisabledStyles = false;

        public readonly Dictionary<OxToolbarAction, OxButton> Actions = new();
        public OxActionClick<OxToolbarAction>? ToolbarActionClick;

        public void RecalcWidth() => 
            SetContentSize(buttons.Width(), SavedHeight);

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetToolBarPaddings();
            SetContentSize(Width, DefaultToolBarHeight);
        }

        private readonly Dictionary<OxClickFrame, OxBorder_old> separators = new();

        private void PlaceButtons()
        {
            OxClickFrame? lastButton = null;

            foreach (OxClickFrame button in buttons)
            {
                button.Parent = this;
                button.Dock = 
                    button.Dock is DockStyle.Right 
                        ? DockStyle.Right 
                        : DockStyle.Left;

                if (button.BeginGroup && 
                    lastButton is not null)
                {
                    button.Margins.LeftOx = OxSize.S;
                    lastButton.Margins.RightOx = OxSize.S;

                    if (lastButton.Dock.Equals(button.Dock))
                        SeparateButtonsGroup(button);
                }
                else
                    button.Margins.LeftOx = 
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

        private void SeparateButtonsGroup(OxClickFrame startButton)
        {
            if (!separators.TryGetValue(startButton, out var separator))
            {
                separator = startButton.Dock switch
                {
                    DockStyle.Right => OxBorder_old.NewRight(this, BorderColor, OxSize.XXS),
                    _ => OxBorder_old.NewLeft(this, BorderColor, OxSize.XXS),
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
            foreach (OxClickFrame button in buttons)
            {
                button.VisibleChanged -= ButtonVisibleChangedHandler;
                button.Parent = null;
            }
        }

        private void SetButtons(OxClickFrameList buttonList)
        {
            ClearButtons();
            buttons = buttonList;
            PlaceButtons();
        }

        protected virtual void SetToolBarPaddings()
        {
            Paddings.LeftOx = OxSize.None;
            Paddings.RightOx = OxSize.XXS;
            Paddings.TopOx = OxSize.XS;
            Paddings.BottomOx = OxSize.S;

            if (buttons.Last?.CalcedHeight > ContentContainer.CalcedHeight)
            {
                Paddings.TopOx = OxSize.XXS;
                Paddings.BottomOx = OxSize.XXS;
            }
        }

        public OxClickFrameList Buttons
        {
            get => buttons;
            set => SetButtons(value);
        }

        public EventHandler? OnButtonVisibleChange;

        protected override void PrepareColors()
        {
            base.PrepareColors();

            foreach (OxClickFrame button in buttons)
                button.BaseColor = BaseColor;
        }

        protected override void OnEnabledChanged(EventArgs e)
        { 
            base.OnEnabledChanged(e);

            foreach (OxClickFrame button in buttons)
                button.Enabled = Enabled;
        }

        protected override void ApplyBordersColor() => 
            BorderColor = Colors.Lighter(4);

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

        public OxButton AddButton(OxToolbarAction action, bool beginGroup = false, 
            DockStyle dockStyle = DockStyle.Left)
        {
            OxButton button = new(
                OxToolbarActionHelper.Text(action),
                OxToolbarActionHelper.Icon(action))
            {
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
            button.SetContentSize(OxToolbarActionHelper.Width(action), button.Height);
            Actions.Add(action, button);
            Buttons.Add(button);

            PlaceButtons();
            return button;
        }

        public OxClickFrame AddButton(OxClickFrame button)
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

    public class OxHeaderToolBar : OxToolBar
    {
        protected override void ApplyBordersColor() => 
            BorderColor = Color.Transparent;

        protected override void SetToolBarPaddings() => 
            Paddings.SetSize(OxSize.XXS);
    }
}