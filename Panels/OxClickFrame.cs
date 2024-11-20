namespace OxLibrary.Panels
{
    public class OxClickFrame : OxFrame
    {
        protected bool hovered = false;
        private bool freezeHovered = false;
        public bool HandHoverCursor = false;

        public GetColor? GetHoveredColor;

        public bool Hovered
        {
            get => hovered;
            set
            {
                hovered = value;
                PrepareColors();
                HoveredChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool fixBorderColor = false;

        public bool FixBorderColor
        {
            get => fixBorderColor;
            set => SetFixBorderColor(value);
        }


        public EventHandler? HoveredChanged;

        public bool FreezeHovered
        {
            get => freezeHovered;
            set => SetFreezeHovered(value);
        }

        private void SetFreezeHovered(bool value)
        {
            freezeHovered = value;
            PrepareColors();
        }

        private void SetFixBorderColor(bool value)
        {
            fixBorderColor = value;
            PrepareColors();
        }

        public OxClickFrame() : base() { }
        public OxClickFrame(Size contentSize) : base(contentSize) { }

        private bool readOnly = false;

        public bool ReadOnly 
        { 
            get => readOnly;
            set => SetReadOnly(value);
        }

        protected virtual void SetReadOnly(bool value) => 
            readOnly = value;

        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = true;
            BeginGroup = false;
        }

        public override Color GetBordersColor() => 
            (fixBorderColor || hovered)
            && !ReadOnly
                ? Colors.Darker(3)
                : HiddenBorder 
                    || !Enabled
                    ? Colors.Lighter(8)
                    : base.GetBordersColor();

        protected override void PrepareColors()
        {
            base.PrepareColors();

            BackColor =
                (Enabled && !ReadOnly)
                || !UseDisabledStyles
                    ? hovered
                        || FreezeHovered
                        ? GetHoveredColor is not null
                            ? GetHoveredColor.Invoke()
                            : Colors.Darker(2)
                        : BaseColor
                    : Colors.Darker(ReadOnly ? 0 : 1);
            /*
            BaseColor =
                (Enabled && !ReadOnly) 
                || !UseDisabledStyles
                    ? hovered 
                        || FreezeHovered
                        ? GetHoveredColor is not null
                            ? GetHoveredColor.Invoke()
                            : Colors.Darker(2)
                        : BaseColor
                    : Colors.Darker(ReadOnly ? 0 : 1);
            */
        }

        public void PerformClick() =>
            InvokeOnClick(this, null);

        protected void SetClickHandler(Control control) =>
            control.Click += (s, e) => PerformClick();

        public void SetHoverHandlers(Control control)
        {
            control.MouseEnter += MouseEnterHandler;
            control.MouseLeave += MouseLeaveHandler;
        }

        protected virtual void MouseEnterHandler(object? sender, EventArgs e)
        {
            if (hovered)
                return;
            
            Hovered = true;

            if (HandHoverCursor)
                Cursor = Cursors.Hand;
        }

        protected override Cursor DefaultCursor => Enabled ? Cursors.Default : Cursors.No;

        protected virtual void MouseLeaveHandler(object? sender, EventArgs e)
        {
            if (!hovered)
                return;

            Hovered = false;

            if (HandHoverCursor)
                Cursor = Cursors.Default;
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
}