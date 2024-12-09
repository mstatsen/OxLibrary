using OxLibrary.Geometry;

namespace OxLibrary.Panels
{
    public class OxClickFrame : OxFrame
    {
        protected bool hovered = false;
        private bool freezeHovered = false;
        public bool HandHoverCursor { get; set; } = false;
        public bool IncreaceIfHovered { get; set; } = false;
        public short HoveredIncreaseSize { get; set; } = 2;

        private bool useDefaultHoveredColor = true;
        public bool UseDefaultHoveredColor 
        { 
            get => useDefaultHoveredColor;
            set
            { 
                useDefaultHoveredColor = value;
                SetBaseColor();
            }
        }
        private Color hoveredColor = Color.Transparent;
        public Color HoveredColor
        {
            get => 
                useDefaultHoveredColor 
                    ? Colors.Darker(4) 
                    : hoveredColor;
            set
            {
                hoveredColor = value;
                useDefaultHoveredColor = false;
                SetBaseColor();
            }
        }

        public bool Hovered
        {
            get => hovered;
            set
            {
                hovered = value;
                SetBaseColor();
                HoveredChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void PrepareColors()
        {
            base.PrepareColors();

            if (!ClickFrameBaseColorProcess)
                Colors.BaseColor = BaseColor;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Hovered = false;
        }

        protected override Color GetForeColor() =>
            Colors.Darker(Enabled || !UseDisabledStyles ? 7 : -4);

        private bool ClickFrameBaseColorProcess = false;

        public new OxColorHelper Colors { get; private set; } = new(DefaultBackColor);

        private void SetBaseColor()
        {
            ClickFrameBaseColorProcess = true;

            try
            {
                BaseColor =
                    (Enabled && !ReadOnly)
                    || !UseDisabledStyles
                        ? hovered
                            || FreezeHovered
                                ? HoveredColor
                                : Colors.BaseColor
                        : Colors.Lighter(ReadOnly ? 1 : 0);
            }
            finally
            { 
                ClickFrameBaseColorProcess = false;
            }
        }

        private bool fixedBorderColor = true;

        public bool FixedBorderColor
        {
            get => fixedBorderColor;
            set
            {
                fixedBorderColor = value;
                SetBaseColor();
            }
        }


        public EventHandler? HoveredChanged;

        public bool FreezeHovered
        {
            get => freezeHovered;
            set
            {
                freezeHovered = value;
                SetBaseColor();
            }
        }

        public OxClickFrame() : this(OxSize.Empty) { }
        public OxClickFrame(OxSize size) : base(size: size)
        {
            Colors ??= new(DefaultColor);
            SetHoverHandlers(this);
        }

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

        protected override Color GetBorderColor() => 
            (fixedBorderColor || hovered)
            && !ReadOnly
                ? Colors.Darker(3)
                : HiddenBorder 
                    || !Enabled
                    ? Colors.Lighter(7)
                    : base.GetBorderColor();


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
            if (!Enabled
                || hovered)
                return;
            
            Hovered = true;

            if (HandHoverCursor)
                Cursor = Cursors.Hand;

            if (IncreaceIfHovered)
            {
                Left -= OxSH.Half(HoveredIncreaseSize);
                Top -= OxSH.Half(HoveredIncreaseSize);
                Height += HoveredIncreaseSize;
                Width += HoveredIncreaseSize;
            }
        }

        protected override Cursor DefaultCursor => Enabled ? Cursors.Default : Cursors.No;

        protected virtual void MouseLeaveHandler(object? sender, EventArgs e)
        {
            if (!hovered)
                return;

            Hovered = false;

            if (HandHoverCursor)
                Cursor = Cursors.Default;

            if (IncreaceIfHovered)
            {
                Left += OxSH.Half(HoveredIncreaseSize);
                Top += OxSH.Half(HoveredIncreaseSize);
                Height -= HoveredIncreaseSize;
                Width -= HoveredIncreaseSize;
            }
        }

        private bool hiddenBorder;

        public bool HiddenBorder
        {
            get => hiddenBorder;
            set
            {
                hiddenBorder = value;
                SetBaseColor();
            }
        }

        public bool Default { get; set; }
        public bool BeginGroup { get; set; }
    }
}