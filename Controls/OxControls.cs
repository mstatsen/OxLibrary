using OxLibrary.Dock;
using OxLibrary.Handlers;

namespace OxLibrary.Controls
{
    public class OxControls : List<IOxControl>
    {
        public readonly IOxContainer Container;
        public OxControls(IOxContainer container) => 
            Container = container;

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

        public event OxControlEvent? ControlAdded;
        public event OxControlEvent? ControlRemoved;

        private void OnControlAdded(OxControlEventArgs e)
        {
            ControlAdded?.Invoke(Container, e);
            Container.RealignControls(
                OxDockHelper.DockType(e.Control.Dock)
            );
        }

        private void OnControlRemoved(OxControlEventArgs e)
        {
            ControlRemoved?.Invoke(Container, e);

            if (OxDockHelper.DockType(e.Control) is OxDockType.Docked)
                Container.RealignControls();
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
