using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    /// <summary>
    /// Interface OxLibrary controls container 
    /// </summary>
    public interface IOxControlContainer : IOxControl, IOxControlContainerManager
    {
    }

    public interface IOxControlContainer<TBaseControl> : IOxControl<TBaseControl>, IOxControlContainer
        where TBaseControl : Control
    { 
    }
}