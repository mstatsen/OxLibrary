namespace OxLibrary.Panels
{
    public class OxFrame : OxPanel
    {
        public OxFrame() : this(OxSize.Empty) { }
        public OxFrame(OxSize size) : base(size)
        {
            BorderVisible = true;
            SetBorderWidth(1);
        }
    }
}