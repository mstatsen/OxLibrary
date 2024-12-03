using OxLibrary.Panels;

namespace OxLibrary.Handlers
{
    public class OxBordersChangedEventArgs : OxEventArgs
    {
        public OxBorders OldValue;
        public OxBorders NewValue;

        public OxBordersChangedEventArgs(OxBorders oldValue, OxBorders newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool Changed => !NewValue.Equals(OldValue);
    }
}