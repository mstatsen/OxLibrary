namespace OxLibrary.Panels
{
    public class OxUnderlinedPanel : OxPanel
    {
        private OxBorder underline = default!;

        public OxBorder Underline => underline!;

        protected void CreateUnderline() =>
            underline = OxBorder.NewBottom(this, Colors.Darker(4));

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            CreateUnderline();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            underline.BackColor = BaseColor;
        }

        protected override int GetCalcedHeight() =>
            base.GetCalcedHeight() + (underline != null ? underline.CalcedSize : 0);

        public OxUnderlinedPanel(Size contentSize) : base(contentSize) { }

        public override void ReAlignControls()
        {
            base.ReAlignControls();
            underline.ReAlign();
        }

        public int UnderlineSize
        {
            get => underline.GetSize();
            set => underline.SetSize(value);
        }

        public Color UnderlineColor
        {
            get => underline.BackColor;
            set => underline.BackColor = value;
        }

        public bool UnderlineVisible
        {
            get => underline.Visible;
            set => underline.Visible = value;
        }
    }
}