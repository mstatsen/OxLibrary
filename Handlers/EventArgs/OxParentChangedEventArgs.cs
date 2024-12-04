using OxLibrary.Controls;

namespace OxLibrary.Handlers
{
    public class OxParentChangedEventArgs : OxEventArgs<IOxBox>
    {
        public OxParentChangedEventArgs(IOxBox? oldValue, IOxBox? newValue) :
            base(oldValue, newValue)
        { }
    }
}