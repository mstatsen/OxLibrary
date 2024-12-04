namespace OxLibrary.Interfaces
{
    public interface IOxBox :
        IOxControl,
        IOxBoxManager
    {
    }

    public interface IOxBox<TOxControl> :
        IOxBox,
        IOxControlWithManager<IOxBoxManager>
    {
    }
}