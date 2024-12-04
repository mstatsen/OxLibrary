namespace OxLibrary.Controls
{
    /// <summary>
    /// Interface OxLibrary controls box 
    /// </summary>
    
    public interface IOxBox : IOxControl, IOxBoxManager
    {
        IOxBoxManager Manager { get; }
    }
}