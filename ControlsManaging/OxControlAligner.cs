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

    internal OxBool Realigning { get; private set; } = OxB.F;
    private bool IsRealigning => OxB.B(Realigning);

    internal OxControlAligner(IOxBox box) =>
        Box = box;

    internal void Realign()
    {
        if (OuterControlZone.IsEmpty
            || IsRealigning)
            return;

        ControlZone.CopyFrom(OuterControlZone);
        Realigning = OxB.T;

        try
        {
            Box.WithSuspendedLayout(
                () =>
                {
                    if (RealignControls(OxDockType.Docked, CalcedDockedBounds))
                        RealignControls(OxDockType.Undocked, CalcedUndockedBounds);

                    Box.Invalidate();
                }
            );
        }
        finally
        {
            Realigning = OxB.F;
        }
    }

    private bool RealignControls(OxDockType dockType, Func<IOxControl, OxRectangle> calcBounds)
    {
        OxControlDictionary<OxRectangle> controlsBounds = new();

        foreach (IOxControl control in OxControls[dockType])
        {
            if (!control.IsVisible)
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
            || childBox.IsHandleParentPadding
            || Box is IOxDependedBox
            || Box is not IOxWithPadding paddingBox
            || paddingBox.Padding.IsEmpty)
            return;

        controlBounds.X = 
            control.Dock is OxDock.Right
                ? OxSh.Add(controlBounds.X, paddingBox.Padding.Right)
                : OxSh.Sub(controlBounds.X, paddingBox.Padding.Left);
        controlBounds.Y = 
            control.Dock is OxDock.Bottom
                ? OxSh.Add(controlBounds.Y, paddingBox.Padding.Bottom)
                : OxSh.Sub(controlBounds.Y, paddingBox.Padding.Top);

        if (OxDockHelper.TouchHeight(control.Dock))
            controlBounds.Width = 
                OxSh.Short(
                    controlBounds.Width
                    + paddingBox.Padding.Left
                    + paddingBox.Padding.Right
                );

        if (OxDockHelper.TouchWidth(control.Dock))
            controlBounds.Height = 
                OxSh.Short(
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
            control.Dock is OxDock.Right
                ? OxSh.Sub(ControlZone.Right, controlBounds.Width)
                : ControlZone.X;
        controlBounds.Y =
            control.Dock is OxDock.Bottom
                ? OxSh.Sub(ControlZone.Bottom, controlBounds.Height)
                : ControlZone.Y;

        HandleParentPadding(control, controlBounds);
        return controlBounds;
    }

    private OxRectangle CalcedUndockedBounds(IOxControl control)
    {
        //control.ZBounds.RestoreBounds();
        short left = OxSh.Add(control.ZBounds.Left, ControlZone.X - InnerControlZone.X);
        short top = OxSh.Add(control.ZBounds.Top, ControlZone.Y - InnerControlZone.Y);
        short width = control.ZBounds.Width;
        short right = OxSh.Add(left, width);
        short height = control.ZBounds.Height;
        short bottom = OxSh.Add(top, height);

        if (right > ControlZone.Right)
            width = OxSh.Sub(ControlZone.Right, left);

        if (bottom > ControlZone.Bottom)
            height = OxSh.Sub(ControlZone.Bottom, top);

        return new(left, top, width, height);
    }

    private static void SetBounds(OxControlDictionary<OxRectangle> boundsDictionary)
    {
        foreach (var item in boundsDictionary)
            SetBounds(item.Key, item.Value);

        foreach (IOxControl control in boundsDictionary.Keys)
            if (control is IOxBox box)
                box.Realign();
    }

    private static void SetBounds(IOxControl control, OxRectangle newBounds)
    {
/*
   if (control.Dock is OxDock.None)
        {
*/
            control.ZBounds.RestoreSize();

            if (control.Dock is OxDock.None)
                control.ZBounds.RestoreLocation();

            if (control.ZBounds.Left.Equals(newBounds.X)
                && control.ZBounds.Top.Equals(newBounds.Y)
                && control.ZBounds.Width.Equals(newBounds.Width)
                && control.ZBounds.Height.Equals(newBounds.Height))
                return;
            /*
        }
*/

        control.WithSuspendedLayout(
            () =>
            {
                control.ZBounds.Location = newBounds.Location;
                control.ZBounds.Size = newBounds.Size;
                control.ZBounds.ApplyBoundsToControl();
            }
        );
    }

    private void SubstractControlFromControlZone(IOxControl control)
    {
        switch (control.Dock)
        {
            case OxDock.Fill:
                ControlZone.Clear(); //Use only one fill control in one box
                return;
            case OxDock.Left:
                ControlZone.X = OxSh.Add(ControlZone.X, control.ZBounds.Width);
                break;
            case OxDock.Top:
                ControlZone.Y = OxSh.Add(ControlZone.Y, control.ZBounds.Height);
                break;
        }

        if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Width)
            ControlZone.Width = OxSh.Sub(ControlZone.Width, control.ZBounds.Width);
        else
            if (OxDockHelper.Variable(control.Dock) is OxDockVariable.Height)
                ControlZone.Height = OxSh.Sub(ControlZone.Height, control.ZBounds.Height);
    }
}