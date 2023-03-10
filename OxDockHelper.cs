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
                _ => DockStyle.Left,
            };

        public static OxDock Dock(DockStyle dock) => 
            dock switch
            {
                DockStyle.Top => OxDock.Top,
                DockStyle.Bottom => OxDock.Bottom,
                DockStyle.Left => OxDock.Left,
                DockStyle.Right => OxDock.Right,
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
            dock == OxDock.Top || dock == OxDock.Bottom;

        public static List<OxDock> All()
        {
            List<OxDock> list = new();

            foreach (OxDock dock in Enum.GetValues(typeof(OxDock)))
                list.Add(dock);

            return list;
        }
    }
}