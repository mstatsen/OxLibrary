namespace OxLibrary.Handlers;

public static class OxHandlerTypeHelper
{
    public static bool UseDependedFromBox(OxHandlerType type) =>
        type is OxHandlerType.AutoSizeChanged or
                OxHandlerType.DockChanged or
                OxHandlerType.EnabledChanged or
                OxHandlerType.LocationChanged or
                OxHandlerType.ParentChanged or
                OxHandlerType.SizeChanged or
                OxHandlerType.VisibleChanged;
}
