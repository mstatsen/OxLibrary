using OxLibrary.Controls;

namespace OxLibrary.Handlers
{
    public class OxParentChangedEventArgs : OxEventArgs<IOxContainer>
    {
        public OxParentChangedEventArgs(IOxContainer? oldValue, IOxContainer? newValue) :
            base(oldValue, newValue)
        { }
    }
}