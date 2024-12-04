using OxLibrary.Dock;
using OxLibrary.Handlers;

namespace OxLibrary.Controls
{
    public class OxControls<TOxControl> : List<IOxControl>
        where TOxControl :
            Control,
            IOxManagingControl<IOxBoxManager<TOxControl>>,
            IOxManagingControl<IOxControlManager>,
            IOxBox<TOxControl>
    {
        public readonly IOxBox<TOxControl> Box;
        public OxControls(IOxBox<TOxControl> box) => 
            Box = box;

        public List<IOxControl> Controls(OxDockType dockType)
        {
            List<IOxControl> result = 
                FindAll(c =>
                    c.Dock is not OxDock.Fill
                    && OxDockTypeHelper.ContainsIn(
                            OxDockHelper.DockType(c),
                            dockType
                       )
                );

            if (OxDockTypeHelper.ContainsIn(OxDockType.Docked, dockType))
                result.AddRange(FindAll(c => c.Dock is OxDock.Fill));

            return result;
        }

        public new IOxControl Add(IOxControl control)
        {
            if (Contains(control))
                return control;

            base.Add(control);
            OnControlAdded(new(control));
            return control;
        }

        public event OxControlEvent<TOxControl>? ControlAdded;
        public event OxControlEvent<TOxControl>? ControlRemoved;

        private void OnControlAdded(OxControlEventArgs e)
        {
            ControlAdded?.Invoke(Box, e);
            Box.RealignControls(
                OxDockHelper.DockType(e.Control.Dock)
            );
        }

        private void OnControlRemoved(OxControlEventArgs e)
        {
            ControlRemoved?.Invoke(Box, e);

            if (OxDockHelper.DockType(e.Control) is OxDockType.Docked)
                Box.RealignControls();
        }

        public new IOxControl Remove(IOxControl control)
        {
            if (!Contains(control))
                return control;

            base.Remove(control);
            OnControlRemoved(new(control));
            return control;
        }

        /*
        public List<IOxControl> ByPlacingPriority
        {
            get
            {
                List<IOxControl> result = new();

                foreach (OxDock dock in OxDockHelper.ByPlacingPriority)
                    foreach (IOxControl oxControl in FindAll(c => c.Dock.Equals(dock)))
                        result.Add(oxControl);

                return result;
            }
        }
        */
    }
}
