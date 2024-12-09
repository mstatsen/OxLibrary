namespace OxLibrary.Handlers;

public static class OxHandlerTypeHelper
{
    public static bool UseDependedFromBox(OxHandlerType type) =>
        type is OxHandlerType.DockChanged or
                OxHandlerType.LocationChanged or
                OxHandlerType.ParentChanged or
                OxHandlerType.SizeChanged;
}
