namespace OxLibrary.Panels
{
    public class OxFrame : OxPane
    {
        public OxFrame() : this(OxSize.Empty) { }
        public OxFrame(OxSize size) : base(size)
        {
            BorderVisible = true;
            SetBorderWidth(OxWh.W1);
        }
    }
}