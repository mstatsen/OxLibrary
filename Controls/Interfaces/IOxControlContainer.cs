namespace OxLibrary.Controls
{
    public interface IOxContainer : IOxBaseControl, IOxContainerManager
    { 
    }

    public interface IOxControlContainer : IOxControl,
        IOxContainer,
        IOxContainerManager
    {
    }

    /// <summary>
    /// Interface OxLibrary controls container 
    /// </summary>
    public interface IOxControlContainer<TBaseControl> :
        IOxControl<TBaseControl, IOxContainerManager>,
        IOxContainer,
        IOxContainerManager
        where TBaseControl : Control
    { 
    }
}