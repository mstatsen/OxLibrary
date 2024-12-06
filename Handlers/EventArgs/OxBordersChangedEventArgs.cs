namespace OxLibrary.Handlers
{
    public class OxBordersChangedEventArgs : OxNotNullEventArgs<OxBorders>
    {
        public OxBordersChangedEventArgs(OxBorders oldValue, OxBorders newValue) :
            base(new(oldValue), new(newValue)) { }
    }
}