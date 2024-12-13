using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

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

        public override bool IsHovered => hovered;
        public override void SetHovered(bool value)
        {
            hovered = value;
            SetBaseColor();
            HoveredChanged?.Invoke(this, EventArgs.Empty);
        }


        public override void PrepareColors()
        {
            base.PrepareColors();

            if (!ClickFrameBaseColorProcess)
                Colors.BaseColor = BaseColor;
        }

        public override void OnEnabledChanged(OxBoolChangedEventArgs e)
        {
            base.OnEnabledChanged(e);
            Hovered = OxB.F;
        }

        protected override Color GetForeColor() =>
            Colors.Darker(IsEnabled || !UseDisabledStyles ? 7 : -4);

        private bool ClickFrameBaseColorProcess = false;

        public new OxColorHelper Colors { get; private set; } = new(DefaultBackColor);

        private void SetBaseColor()
        {
            ClickFrameBaseColorProcess = true;

            try
            {
                BaseColor =
                    (IsEnabled && !IsReadOnly)
                    || !UseDisabledStyles
                        ? hovered
                            || FreezeHovered
                                ? HoveredColor
                                : Colors.BaseColor
                        : Colors.Lighter(IsReadOnly ? 1 : 0);
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

        private OxBool readOnly = OxB.F;

        public OxBool ReadOnly 
        { 
            get => OxB.B(IsReadOnly);
            set => SetReadOnly(OxB.B(value));
        }

        protected virtual void SetReadOnly(bool value) => 
            readOnly = OxB.B(value);

        protected virtual bool IsReadOnly => OxB.B(readOnly);

        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = true;
            BeginGroup = false;
        }

        protected override Color GetBorderColor() => 
            (fixedBorderColor || hovered)
            && !IsReadOnly
                ? Colors.Darker(3)
                : HiddenBorder 
                    || !IsEnabled
                    ? Colors.Lighter(7)
                    : base.GetBorderColor();


        public void PerformClick() =>
            InvokeOnClick(this, null);

        protected void SetClickHandler(IOxControl control) =>
            control.Click += ControlClickHandler;
        private void ControlClickHandler(object? sender, EventArgs e) =>
            PerformClick();

        public void SetHoverHandlers(IOxControl control)
        {
            control.MouseEnter += MouseEnterHandler;
            control.MouseLeave += MouseLeaveHandler;
        }

        protected virtual void MouseEnterHandler(object? sender, EventArgs e)
        {
            if (!IsEnabled
                || hovered)
                return;
            
            Hovered = OxB.T;

            if (HandHoverCursor)
                Cursor = Cursors.Hand;

            if (IncreaceIfHovered)
            {
                Left -= OxSh.Half(HoveredIncreaseSize);
                Top -= OxSh.Half(HoveredIncreaseSize);
                Height += HoveredIncreaseSize;
                Width += HoveredIncreaseSize;
            }
        }

        protected override Cursor DefaultCursor => IsEnabled ? Cursors.Default : Cursors.No;

        protected virtual void MouseLeaveHandler(object? sender, EventArgs e)
        {
            if (!hovered)
                return;

            Hovered = OxB.F;

            if (HandHoverCursor)
                Cursor = Cursors.Default;

            if (IncreaceIfHovered)
            {
                Left += OxSh.Half(HoveredIncreaseSize);
                Top += OxSh.Half(HoveredIncreaseSize);
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