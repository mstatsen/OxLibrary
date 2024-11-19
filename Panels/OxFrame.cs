namespace OxLibrary.Panels
{
    public class OxFrame : OxPanel, IOxFrame
    {
        private OxBorders_old borders = default!;
        private OxBorders_old margins = default!;
        private bool blurredBorder;

        private void CreateBorders()
        {
            borders = new OxBorders_old(this);
            borders.SetSize(OxSize.XXS);
            ApplyBordersColor();
        }

        private void CreateMargins() =>
            margins = new OxBorders_old(this);

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            CreateBorders();
            CreateMargins();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            ApplyBordersColor();
            ApplyMarginsColor();
        }

        protected virtual void ApplyBordersColor() =>
            borders.Color = Enabled || !UseDisabledStyles
                ? BaseColor
                : Colors.Lighter(2);

        private Color GetMarginsColor() =>
            Parent is null 
            || blurredBorder
                ? Color.Transparent
                : Parent.BackColor;

        protected override int GetCalcedWidth() =>
            base.GetCalcedWidth() +
            margins.CalcedSize(OxDock.Left) +
            margins.CalcedSize(OxDock.Right) +
            borders.CalcedSize(OxDock.Left) +
            borders.CalcedSize(OxDock.Right);

        protected override int GetCalcedHeight() =>
            base.GetCalcedHeight() +
            margins.CalcedSize(OxDock.Top) +
            margins.CalcedSize(OxDock.Bottom) +
            borders.CalcedSize(OxDock.Top) +
            borders.CalcedSize(OxDock.Bottom);

        public OxBorders_old Borders => borders;
        public OxBorders_old Margins => margins;

        public int BorderWidth
        {
            get => borders.GetSize();
            set => borders.SetSize(value);
        }

        public Color BorderColor
        {
            get => borders.Color;
            set => SetBordersColor(value);
        }

        public bool BorderVisible
        {
            set
            {
                foreach (OxDock dock in OxDockHelper.All())
                    Borders[dock].Visible = value;
            }
        }

        protected virtual void SetBordersColor(Color value) =>
            borders.Color = value;

        public bool BlurredBorder
        {
            get => blurredBorder;
            set
            {
                blurredBorder = value;
                ApplyMarginsColor();
            }
        }

        public OxFrame() : base() { }
        public OxFrame(Size contentSize) : base(contentSize) { }

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            Borders.ReAlign();
            Margins.ReAlign();
        }

        protected override void OnParentBackColorChanged(EventArgs e) =>
            ApplyMarginsColor();

        private void ApplyMarginsColor() =>
            Margins.Color = GetMarginsColor();

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            ApplyMarginsColor();
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            margins.SizeChanged += BorderSizeEventHandler;
            borders.SizeChanged += BorderSizeEventHandler;
        }

        protected override void SetToolTipText(string value)
        {
            base.SetToolTipText(value);

            foreach (OxBorder_old border in Borders.Borders.Values)
                border.ToolTipText = value;

            foreach (OxBorder_old margin in Margins.Borders.Values)
                margin.ToolTipText = value;
        }
    }
}