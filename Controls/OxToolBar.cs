using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class ToolbarActionEventArgs : EventArgs
    {
        public OxToolbarAction Action { get; internal set; }
        public ToolbarActionEventArgs(OxToolbarAction action) : base() =>
            Action = action;
    }

    public delegate void ToolbarActionClick(object? sender, ToolbarActionEventArgs EventArgs);

    public class OxToolBar : OxFrame
    {
        private const int DefaultToolBarHeight = 28;
        private OxClickFrameList buttons = new();

        public OxToolBar() : base() => 
            UseDisabledStyles = false;

        public readonly Dictionary<OxToolbarAction, OxButton> Actions = new();
        public ToolbarActionClick? ToolbarActionClick;

        public void RecalcWidth() => 
            SetContentSize(buttons.Width(), SavedHeight);

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetToolBarPaddings();
            SetContentSize(Width, DefaultToolBarHeight);
        }

        private readonly Dictionary<OxClickFrame, OxBorder> separators = new();

        private void PlaceButtons()
        {
            if (buttons == null)
                return;

            OxClickFrame? lastButton = null;

            foreach (OxClickFrame button in buttons)
            {
                button.Parent = this;
                button.Dock = button.Dock == DockStyle.Right ? DockStyle.Right : DockStyle.Left;

                if (button.BeginGroup && 
                    lastButton != null)
                {
                    button.Margins.LeftOx = OxSize.Large;
                    lastButton.Margins.RightOx = OxSize.Large;

                    if (lastButton.Dock == button.Dock)
                        SeparateButtonsGroup(button);
                }
                else
                    button.Margins.LeftOx = lastButton == null ? OxSize.None : OxSize.Small;

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
                    DockStyle.Right => OxBorder.NewRight(this, BorderColor, OxSize.Small),
                    _ => OxBorder.NewLeft(this, BorderColor, OxSize.Small),
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
            if (buttons == null)
                return;

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

            if (buttons == null)
                return;

            PlaceButtons();
        }

        protected virtual void SetToolBarPaddings()
        {
            Paddings.LeftOx = OxSize.None;
            Paddings.RightOx = OxSize.Small;
            Paddings.TopOx = OxSize.Medium;
            Paddings.BottomOx = OxSize.Large;

            if (buttons?.Last?.CalcedHeight > ContentContainer.CalcedHeight)
            {
                Paddings.TopOx = OxSize.Small;
                Paddings.BottomOx = OxSize.Small;
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

            if (buttons != null)
                foreach (OxClickFrame button in buttons)
                    button.BaseColor = BaseColor;
        }

        protected override void SetEnabled(bool value)
        {
            base.SetEnabled(value);

            if (buttons != null)
                foreach (OxClickFrame button in buttons)
                    button.Enabled = value;
        }

        protected override void ApplyBordersColor() => 
            BorderColor = Colors.Lighter(4);

        public void ExecuteDefault() => 
            Buttons?.ExecuteDefault();

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetToolBarPaddings();
        }

        public bool AllowEditingActions
        {
            get
            {
                foreach (KeyValuePair<OxToolbarAction, OxButton> action in Actions)
                    if (OxToolbarActionHelper.ActionForExistItems(action.Key))
                        return action.Value.Enabled;

                return true;
            }
            set
            {
                foreach (KeyValuePair<OxToolbarAction, OxButton> action in Actions)
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
            button.Click += ButtonClickHandler;
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
            foreach (KeyValuePair<OxToolbarAction, OxButton> item in Actions)
                if (item.Value == button)
                    return item.Key;

            return OxToolbarAction.Empty;
        }

        private void ButtonClickHandler(object? sender, EventArgs e) =>
            ToolbarActionClick?.Invoke(sender, 
                new ToolbarActionEventArgs(
                    sender == null 
                        ? OxToolbarAction.Empty 
                        : GetActionByButton((OxButton)sender)
                )
            );
    }

    public class OxHeaderToolBar : OxToolBar
    {
        protected override void ApplyBordersColor() => 
            BorderColor = Color.Transparent;

        protected override void SetToolBarPaddings() => 
            Paddings.SetSize(OxSize.Small);
    }
}