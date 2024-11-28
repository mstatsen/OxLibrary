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

        public List<IOxControl> UndockedControls => 
            FindAll(c => c.Dock is OxDock.None);

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
            if (e.Control.Dock is OxDock.None)
                UndockedControls.Add(e.Control);

            ControlAdded?.Invoke(Container, e);
            Container.RealignControls();
        }

        private void OnControlRemoved(OxControlEventArgs e)
        {
            ControlRemoved?.Invoke(Container, e);
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
    }
}
