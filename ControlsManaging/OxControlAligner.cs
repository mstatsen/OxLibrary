using OxLibrary.ControlsManaging;
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
            controlBounds.X = OxWh.A(controlBounds.X, paddingBox.Padding.Right);
        else controlBounds.X = OxWh.S(controlBounds.X, paddingBox.Padding.Left);

        if (control.Dock is OxDock.Bottom)
            controlBounds.Y = OxWh.A(controlBounds.Y, paddingBox.Padding.Bottom);
        else controlBounds.Y = OxWh.S(controlBounds.Y, paddingBox.Padding.Top);

        if (OxDockHelper.TouchHeight(control.Dock))
        {
            controlBounds.Width = OxWh.A(controlBounds.Width, paddingBox.Padding.Left);
            controlBounds.Width = OxWh.A(controlBounds.Width, paddingBox.Padding.Right);
        }

        if (OxDockHelper.TouchWidth(control.Dock))
        {
            controlBounds.Height = OxWh.A(controlBounds.Height, paddingBox.Padding.Top);
            controlBounds.Height = OxWh.A(controlBounds.Height, paddingBox.Padding.Bottom);
        }
    }

    private OxRectangle CalcedDockedBounds(IOxControl control)
    {
        OxRectangle controlBounds = new(control.ZBounds.Bounds);

        if (OxDockHelper.Variable(control.Dock) is not OxDockVariable.Width)
            controlBounds.Width = ControlZone.Width;

        if (OxDockHelper.Variable(control.Dock) is not OxDockVariable.Height)
            controlBounds.Height = ControlZone.Height;

        controlBounds.X = control.Dock is OxDock.Right
            ? OxWh.S(ControlZone.Right, controlBounds.Width)
            : ControlZone.X;

        controlBounds.Y = control.Dock is OxDock.Bottom
            ? OxWh.S(ControlZone.Bottom, controlBounds.Height)
            : ControlZone.Y;

        HandleParentPadding(control, controlBounds);

        return controlBounds;
    }

    private OxRectangle CalcedUndockedBounds(IOxControl control)
    {
        //control.ZBounds.RestoreBounds();
        OxWidth left = OxWh.R(control.ZBounds.Left, InnerControlZone.X, ControlZone.X);
        OxWidth top = OxWh.R(control.ZBounds.Top, InnerControlZone.Y, ControlZone.Y);
        OxWidth width = OxWh.W(control.ZBounds.Width);
        OxWidth right = OxWh.A(left, width);
        OxWidth height = OxWh.W(control.ZBounds.Height);
        OxWidth bottom = OxWh.A(top, height);

        if (OxWh.Greater(right, ControlZone.Right))
            width = OxWh.S(ControlZone.Right, left);

        if (OxWh.Greater(bottom, ControlZone.Bottom))
            height = OxWh.S(ControlZone.Bottom, top);

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

            if (control.ZBounds.Left.Equals(newBounds.Z_X)
                && control.ZBounds.Top.Equals(newBounds.Z_Y)
                && control.ZBounds.Width.Equals(newBounds.Z_Width)
                && control.ZBounds.Height.Equals(newBounds.Z_Height))
                return;
        }

        control.ZBounds.Location = newBounds.Z_Location;
        control.ZBounds.Size = newBounds.Z_Size;

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
                ControlZone.X = OxWh.A(ControlZone.X, control.ZBounds.Width);
                break;
            case OxDock.Top:
                ControlZone.Y = OxWh.A(ControlZone.Y, control.ZBounds.Height);
                break;
        }

        if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Width)
            ControlZone.Width = OxWh.S(ControlZone.Width, control.ZBounds.Width);
        else
            if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Height)
                ControlZone.Height = OxWh.S(ControlZone.Height, control.ZBounds.Height);
    }
}