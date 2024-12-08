namespace OxLibrary.Handlers;

public class OxBoolEventArgs : OxEventArgs<bool>
{
    public OxBoolEventArgs(bool oldValue, bool newValue) : base(oldValue, newValue)
    { }
}