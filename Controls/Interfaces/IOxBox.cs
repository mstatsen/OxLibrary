namespace OxLibrary.Controls
{
    public interface IOxBox :
        IOxControl,
        IOxBoxManager
    {
    }

    public interface IOxBox<TOxControl> :
        IOxBox,
        IOxBoxManager<TOxControl>,
        IOxManagingControl<IOxBoxManager<TOxControl>>
        where TOxControl : Control,
            IOxManagingControl<IOxBoxManager<TOxControl>>,
            IOxManagingControl<IOxControlManager>,
            IOxBox<TOxControl>
    {
    }
}