namespace OxLibrary.Controls
{
    public class OxDockedControls : Dictionary<OxDock, List<IOxControl>>
    {
        public OxDockedControls()
        {
            foreach (OxDock dock in OxDockHelper.All())
                Add(dock, new());
        }

        private readonly Dictionary<IOxControl, OxDock> Controls = new();

        public void AddControl(IOxControl control)
        {
            if (Controls.ContainsKey(control))
                return;

            Controls.Add(control, control.Dock);
            this[control.Dock].Add(control);
            control.DockChanged += ControlDockChangedHandler;
        }

        private void ControlDockChangedHandler(object? sender, EventArgs e)
        {
            if (sender is not IOxControl oxControl)
                return;

            this[Controls[oxControl]].Remove(oxControl);
            Controls.Remove(oxControl);
            AddControl(oxControl);
        }

        public void RemoveControl(IOxControl control)
        {
            Controls.Remove(control);
            this[control.Dock].Remove(control);
        }
    }
}
