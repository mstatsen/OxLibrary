namespace OxLibrary.Controls
{
    public interface IOxControlWithManager<TManager> : IOxControl
        where TManager : IOxControlManager
    {
        TManager Manager { get; }
    }

    public interface IOxControlWithManager : IOxControlWithManager<IOxControlManager>
    {
    }
}