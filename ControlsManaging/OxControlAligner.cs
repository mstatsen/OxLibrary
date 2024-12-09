using OxLibrary.ControlsManaging;
using OxLibrary.Geometry;
using OxLibrary.Interfaces;

namespace OxLibrary;

[Obsolete("OxControlAligner it is used only for internal needs")]
internal class OxControlAligner
{
    private readonly IOxBox Box;
    internal OxRectangle ControlZone = OxRectangle.Empty;

    private OxRectangle OuterControlZone =>
        Box.OuterControlZone;

    private OxRectangle InnerControlZone =>
        Box.InnerControlZone;

    private OxControls OxControls => Box.OxControls;

    internal bool Realigning { get; private set; } = false;

    internal OxControlAligner(IOxBox box) =>
        Box = box;

    internal void Realign()
    {
        if (OuterControlZone.IsEmpty
            || Realigning)
            return;

        ControlZone.CopyFrom(OuterControlZone);

        Realigning = true;

        try
        {
            if (RealignControls(OxDockType.Docked, CalcedDockedBounds))
                RealignControls(OxDockType.Undocked, CalcedUndockedBounds);
        }
        finally
        {
            Box.Invalidate();
            Realigning = false;
        }
    }

    private bool RealignControls(OxDockType dockType, Func<IOxControl, OxRectangle> calcBounds)
    {
        OxControlDictionary<OxRectangle> controlsBounds = new();

        foreach (IOxControl control in OxControls[dockType])
        {
            if (!control.Visible)
                continue;

            controlsBounds.Add(
                control,
                calcBounds(control));

            if (OxDockHelper.DockType(control.Dock).Equals(OxDockType.Docked))
                SubstractControlFromControlZone(control);
        }

        if (controlsBounds.Count is 0)
            return false;

        SetBounds(controlsBounds);
        return true;
    }

    private void HandleParentPadding(IOxControl control, OxRectangle controlBounds)
    {
        if (control.Dock is OxDock.None
            || control is not IOxBox childBox
            || childBox.HandleParentPadding
            || Box is IOxDependedBox
            || Box is not IOxWithPadding paddingBox
            || paddingBox.Padding.IsEmpty)
            return;

        if (control.Dock is OxDock.Right)
            controlBounds.X = OxSH.Add(controlBounds.X, paddingBox.Padding.Right);
        else controlBounds.X = OxSH.Sub(controlBounds.X, paddingBox.Padding.Left);

        if (control.Dock is OxDock.Bottom)
            controlBounds.Y = OxSH.Add(controlBounds.Y, paddingBox.Padding.Bottom);
        else controlBounds.Y = OxSH.Sub(controlBounds.Y, paddingBox.Padding.Top);

        if (OxDockHelper.TouchHeight(control.Dock))
            controlBounds.Width = 
                OxSH.Short(
                    controlBounds.Width
                    + paddingBox.Padding.Left
                    + paddingBox.Padding.Right
                );

        if (OxDockHelper.TouchWidth(control.Dock))
            controlBounds.Height = 
                OxSH.Short(
                    controlBounds.Height
                    + paddingBox.Padding.Top
                    + paddingBox.Padding.Bottom
                );
    }

    private OxRectangle CalcedDockedBounds(IOxControl control)
    {
        OxRectangle controlBounds = new(control.ZBounds.Bounds);

        if (OxDockHelper.Variable(control.Dock) is not OxDockVariable.Width)
            controlBounds.Width = ControlZone.Width;

        if (OxDockHelper.Variable(control.Dock) is not OxDockVariable.Height)
            controlBounds.Height = ControlZone.Height;

        controlBounds.X = 
            OxSH.IfElse(
                control.Dock is OxDock.Right,
                ControlZone.Right - controlBounds.Width,
                ControlZone.X
            );
        controlBounds.Y = 
            OxSH.IfElse(
                control.Dock is OxDock.Bottom,
                ControlZone.Bottom - controlBounds.Height,
                ControlZone.Y
            );

        HandleParentPadding(control, controlBounds);
        return controlBounds;
    }

    private OxRectangle CalcedUndockedBounds(IOxControl control)
    {
        //control.ZBounds.RestoreBounds();
        short left = OxSH.Add(control.ZBounds.Left, ControlZone.X - InnerControlZone.X);
        short top = OxSH.Add(control.ZBounds.Top, ControlZone.Y - InnerControlZone.Y);
        short width = control.ZBounds.Width;
        short right = OxSH.Add(left, width);
        short height = control.ZBounds.Height;
        short bottom = OxSH.Add(top, height);

        if (right > ControlZone.Right)
            width = OxSH.Sub(ControlZone.Right, left);

        if (bottom > ControlZone.Bottom)
            height = OxSH.Sub(ControlZone.Bottom, top);

        return new(left, top, width, height);
    }

    private static void SetBounds(OxControlDictionary<OxRectangle> boundsDictionary)
    {
        foreach (var item in boundsDictionary)
            SetBounds(item.Key, item.Value);
    }

    private static void SetBounds(IOxControl control, OxRectangle newBounds)
    {
        if (control.Dock is OxDock.None)
        {
            control.ZBounds.RestoreBounds();

            if (control.ZBounds.Left.Equals(newBounds.X)
                && control.ZBounds.Top.Equals(newBounds.Y)
                && control.ZBounds.Width.Equals(newBounds.Width)
                && control.ZBounds.Height.Equals(newBounds.Height))
                return;
        }

        control.ZBounds.Location = newBounds.Location;
        control.ZBounds.Size = newBounds.Size;

        if (control is IOxBox box)
            box.Realign();
    }

    private void SubstractControlFromControlZone(IOxControl control)
    {
        switch (control.Dock)
        {
            case OxDock.Fill:
                ControlZone.Clear(); //Use only one fill control in one box
                return;
            case OxDock.Left:
                ControlZone.X = OxSH.Add(ControlZone.X, control.ZBounds.Width);
                break;
            case OxDock.Top:
                ControlZone.Y = OxSH.Add(ControlZone.Y, control.ZBounds.Height);
                break;
        }

        if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Width)
            ControlZone.Width = OxSH.Sub(ControlZone.Width, control.ZBounds.Width);
        else
            if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Height)
                ControlZone.Height = OxSH.Sub(ControlZone.Height, control.ZBounds.Height);
    }
}