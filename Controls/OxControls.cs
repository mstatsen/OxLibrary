using OxLibrary.Dock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace OxLibrary.Controls
{
    public class OxControlEventArgs : EventArgs
    { 
        public readonly IOxControl Control;

        public OxControlEventArgs(IOxControl control) =>
            Control = control;
    }

    public delegate void OxControlEvent(IOxControlContainer sender, OxControlEventArgs EventArgs);

    public class OxControls : List<IOxControl>
    {
        public readonly IOxControlContainer Container;
        public OxControls(IOxControlContainer container) => 
            Container = container;

        public List<IOxControl> Controls(OxDockType dockType) =>
            FindAll(c => OxDockTypeHelper.ContainsIn(
                OxDockHelper.DockType(c), dockType
            )
        );

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
