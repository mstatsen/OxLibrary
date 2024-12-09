using OxLibrary.Interfaces;

namespace OxLibrary.Handlers;

public class OxParentChangedEventArgs : OxEventArgs<IOxBox>
{
    public OxParentChangedEventArgs(IOxBox? oldValue, IOxBox? newValue) :
        base(oldValue, newValue)
    { }
}