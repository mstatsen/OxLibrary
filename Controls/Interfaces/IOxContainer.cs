namespace OxLibrary.Controls
{
    /// <summary>
    /// Interface OxLibrary controls container 
    /// </summary>
    
    public interface IOxContainer : IOxControl, IOxContainerManager
    {
        IOxContainerManager Manager { get; }
    }
}