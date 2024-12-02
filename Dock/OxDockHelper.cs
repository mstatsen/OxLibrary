using OxLibrary.Controls;
using OxLibrary.Dock;

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

        public static OxDockVariable Variable(OxDock dock) =>
            dock switch
            {
                OxDock.Left or
                OxDock.Right =>
                    OxDockVariable.Width,
                OxDock.Top or
                OxDock.Bottom =>
                    OxDockVariable.Height,
                OxDock.Fill =>
                    OxDockVariable.Fill,
                _ =>
                    OxDockVariable.None
            };

        public static bool IsVertical(OxDock dock) =>
            Variable(dock) is OxDockVariable.Height;

        public static bool IsHorizontal(OxDock dock) =>
            Variable(dock) is OxDockVariable.Width;

        public static bool IsVariableHeight(OxDock dock) =>
            IsVertical(dock)
            || dock is OxDock.None;

        public static bool IsVariableWidth(OxDock dock) =>
            IsHorizontal(dock)
            || dock is OxDock.None;


        private static readonly List<OxDock> all = new();
        public static List<OxDock> All
        {
            get
            {
                if (all.Count is 0)
                    foreach (OxDock dock in Enum.GetValues(typeof(OxDock)))
                        all.Add(dock);

                return all;
            }
        }

        public static bool IsSingleDirectionDock(OxDock dock) =>
            dock is not OxDock.Fill
                and not OxDock.None;

        public static OxDockType DockType(IOxControl control) =>
            DockType(control.Dock);

        public static OxDockType DockType(OxDock dock) =>
            dock is OxDock.None
                ? OxDockType.Undocked
                : OxDockType.Docked;
    }
}