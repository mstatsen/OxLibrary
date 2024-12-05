using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary;

public class OxControls : OxControlList
{
    public readonly IOxBox Box;
    public OxControls(IOxBox box) =>
        Box = box;

    public override IOxControl Add(IOxControl control)
    {
        if (Contains(control))
            return control;
            
        control = base.Add(control);
        OnControlAdded(new(control));
        return control;
    }

    public override IOxControl Remove(IOxControl control)
    {
        if (!Contains(control))
            return control;

        control = base.Remove(control);
        OnControlRemoved(new(control));
        return control;
    }

    public event OxControlEvent? ControlAdded;
    public event OxControlEvent? ControlRemoved;

    private void RealignControls(OxDock controlDock)
    {
        if (OxDockHelper.DockType(controlDock) is OxDockType.Docked)
            Box.Realign();
    }

    private void OnControlAdded(OxControlEventArgs e)
    {
        ControlAdded?.Invoke(Box, e);
        RealignControls(e.Control.Dock);
    }

    private void OnControlRemoved(OxControlEventArgs e)
    {
        ControlRemoved?.Invoke(Box, e);
        RealignControls(e.Control.Dock);
    }
}