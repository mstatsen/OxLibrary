namespace OxLibrary.Handlers;

public class OxChangingEventArgs : OxEventArgs
{
    public bool Cancel { get; set; } = false;
}