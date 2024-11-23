namespace OxLibrary.Panels
{
    public class OxFrame : OxPane
    {
        public OxFrame() : this(OxSize.Empty) { }
        public OxFrame(OxSize size) : base(size)
        {
            BorderVisible = true;
            BorderWidth = OxWh.W1;
        }
    }
}