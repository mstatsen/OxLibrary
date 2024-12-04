namespace OxLibrary.Dock;

public static class OxDockTypeHelper
{
    public static bool ContainsIn(OxDockType searchType, OxDockType list) =>
        (list is OxDockType.Unknown)
        || (searchType & list) == searchType;
}