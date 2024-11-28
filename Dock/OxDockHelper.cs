using OxLibrary.Controls;

namespace OxLibrary
{
    public static class OxDockHelper
    {
        public static DockStyle Dock(OxDock oxDock) =>
            oxDock switch
            {
                OxDock.Left => DockStyle.Left,
                OxDock.Right => DockStyle.Right,
                OxDock.Top => DockStyle.Top,
                OxDock.Bottom => DockStyle.Bottom,
                OxDock.Fill => DockStyle.Fill,
                OxDock.None => DockStyle.None,
                _ => DockStyle.Left,
            };

        public static OxDock Dock(DockStyle dock) =>
            dock switch
            {
                DockStyle.Top => OxDock.Top,
                DockStyle.Bottom => OxDock.Bottom,
                DockStyle.Left => OxDock.Left,
                DockStyle.Right => OxDock.Right,
                DockStyle.None => OxDock.None,
                DockStyle.Fill => OxDock.Fill,
                _ => OxDock.Top,
            };

        public static OxDock Opposite(OxDock dock) =>
            dock switch
            {
                OxDock.Left => OxDock.Right,
                OxDock.Right => OxDock.Left,
                OxDock.Top => OxDock.Bottom,
                OxDock.Bottom => OxDock.Top,
                _ => OxDock.Bottom,
            };

        public static bool IsVertical(OxDock dock) =>
            dock is OxDock.Top
                 or OxDock.Bottom;

        public static bool IsHorizontal(OxDock dock) =>
            dock is OxDock.Left
                 or OxDock.Right;

        public static bool IsVariableHeight(OxDock dock) =>
            IsVertical(dock)
            || dock is OxDock.None;

        public static bool IsVariableWidth(OxDock dock) =>
            IsHorizontal(dock)
            || dock is OxDock.None;

        public static List<OxDock> All()
        {
            List<OxDock> list = new();

            foreach (OxDock dock in Enum.GetValues(typeof(OxDock)))
                list.Add(dock);

            return list;
        }

        public static readonly List<OxDock> SingleDirectionDocks = new()
        {
            OxDock.Top,
            OxDock.Left,
            OxDock.Bottom,
            OxDock.Right
        };

        public static readonly List<OxDock> ByPlacingPriority = new()
        {
            OxDock.Top,
            OxDock.Bottom,
            OxDock.Left,
            OxDock.Right,
            OxDock.Fill,
            OxDock.None
        };

        public static OxDockType DockType(IOxControl control) =>
            DockType(control.Dock);

        public static OxDockType DockType(OxDock dock) =>
            dock is OxDock.None
                ? OxDockType.Undocked
                : OxDockType.Docked;
    }
}