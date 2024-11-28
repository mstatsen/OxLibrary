namespace OxLibrary.Panels
{
    public class OxUnderlinedPanel : OxPanel
    {
        public OxBorder Underline => Borders[OxDock.Bottom];

        public bool UnderlineVisible 
        { 
            get => Borders.Visible(OxDock.Bottom);
            set => Borders.SetVisible(OxDock.Bottom, true); 
        }

        protected override void PrepareInnerComponents()
        {
            Borders.SetVisible(false);
            Borders.SetVisible(OxDock.Bottom, true);
            Borders.Bottom = OxWh.W1;
            base.PrepareInnerComponents();
        }

        public OxUnderlinedPanel(OxSize size) : base(size) { }
        public OxUnderlinedPanel() : this(OxSize.Empty) { }
    }
}