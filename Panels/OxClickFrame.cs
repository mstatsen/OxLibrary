namespace OxLibrary.Panels
{
    public class OxClickFrame : OxFrame
    {
        protected bool hovered = false;
        private bool freezeHovered = false;
        public bool HandHoverCursor = false;

        protected bool Hovered => hovered;

        public bool FreezeHovered
        {
            get => freezeHovered;
            set => SetFreezeHovered(value);
        }

        protected override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            HiddenBorder = !value;
        }

        private void SetFreezeHovered(bool value)
        {
            freezeHovered = value;
            PrepareColors();
        }

        public OxClickFrame() : base() { }
        public OxClickFrame(Size contentSize) : base(contentSize) { }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = true;
            BeginGroup = false;
        }

        protected override void ApplyBordersColor()
        {
            base.ApplyBordersColor();

            if (hovered)
                BorderColor = Colors.Darker(4);
            else
            if (HiddenBorder)
                BorderColor = Colors.Lighter(7);
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            ContentContainer.BaseColor =
                Enabled || !UseDisabledStyles
                    ? hovered || FreezeHovered
                        ? Colors.Darker(2)
                        : ContentContainer.BaseColor
                    : Colors.Darker();
            Paddings.Color = ContentContainer.BackColor;
        }

        private void ChildClickHandler(object? sender, EventArgs e) =>
            PerformClick();

        public void PerformClick() =>
            InvokeOnClick(this, null);

        protected void SetClickHandler(Control control) =>
            control.Click += ChildClickHandler;

        protected void SetHoverHandlers(Control control)
        {
            control.MouseEnter += MouseEnterHandler;
            control.MouseLeave += MouseLeaveHandler;
        }

        private void MouseEnterHandler(object? sender, EventArgs e)
        {
            if (hovered)
                return;
            
            hovered = true;

            if (HandHoverCursor)
                Cursor = Cursors.Hand;

            PrepareColors();
        }

        protected override Cursor DefaultCursor => Enabled ? Cursors.Default : Cursors.No;

        private void MouseLeaveHandler(object? sender, EventArgs e)
        {
            if (!hovered)
                return;

            hovered = false;

            if (HandHoverCursor)
                Cursor = Cursors.Default;

            PrepareColors();
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            SetClickHandler(ContentContainer);
            SetHoverHandlers(ContentContainer);

            foreach (OxBorder border in Paddings.Borders.Values)
            {
                SetClickHandler(border);
                SetHoverHandlers(border);
            }

            foreach (OxBorder border in Borders.Borders.Values)
            {
                SetClickHandler(border);
                SetHoverHandlers(border);
            }
        }

        private bool hiddenBorder;

        public bool HiddenBorder
        {
            get => hiddenBorder;
            set
            {
                hiddenBorder = value;
                PrepareColors();
            }
        }

        public Color Color
        {
            get => BaseColor;
            set => BaseColor = value;
        }

        public bool Default { get; set; }
        public bool BeginGroup { get; set; }
    }

    public class OxClickFrameList : List<OxClickFrame>
    {
        public OxClickFrame? Last => Count > 0 ? this[Count - 1] : null;

        public int Right => Last != null ? Last.Right : 0;

        public int Width()
        {
            int calcedRight = 0;
            int calcedLeft = -1;

            foreach (OxClickFrame frame in this)
                if (frame.Visible)
                {
                    if (calcedLeft < 0)
                        calcedLeft = frame.Left;

                    calcedRight = frame.Right;
                }

            return calcedRight - Math.Max(calcedLeft, 0);
        }

        public int Height()
        {
            int maxHeight = 0;

            foreach (OxClickFrame frame in this)
                maxHeight = Math.Max(maxHeight, frame.Height);

            return maxHeight;
        }

        private OxClickFrame? Default()
        {
            foreach (OxClickFrame button in this)
                if (button.Visible && button.Enabled && button.Default)
                    return button;

            return null;
        }

        public void ExecuteDefault() =>
            Default()?.PerformClick();
    }
}
