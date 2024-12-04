using OxLibrary.Dock;
using OxLibrary.Interfaces;

namespace OxLibrary;

public class OxControlAligner
{
    private readonly IOxBox Box;
    public OxRectangle ControlZone = OxRectangle.Empty;
    public OxRectangle OuterControlZone => Box.OuterControlZone;

    public bool Realigning { get; private set; } = false;

    public OxControlAligner(IOxBox box) =>
        Box = box;

    public void RealignControls(OxDockType dockType = OxDockType.Unknown)
    {
        if (OuterControlZone.IsEmpty
            || Realigning)
            return;

        ControlZone.CopyFrom(OuterControlZone);
        OxRectangle oldControlZone = new(ControlZone);
        Realigning = true;

        try
        {
            if (OxDockTypeHelper.ContainsIn(OxDockType.Docked, dockType))
                if (!RealignDockedControls())
                    return;

            if (OxDockTypeHelper.ContainsIn(OxDockType.Undocked, dockType))
                RealignUndockedControls(
                    oldControlZone,
                    dockType is OxDockType.Undocked
                );
        }
        finally
        {
            Realigning = false;
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private OxSize GetRealControlSize(IOxControl control) =>
        new(OxWh.Min(
                OxDockHelper.IsVariableWidth(control.Dock)
                    ? control.OriginalWidth
                    : ControlZone.WidthInt,
                ControlZone.Width
            ),
            OxWh.Min(
                OxDockHelper.IsVariableHeight(control.Dock)
                    ? control.OriginalHeight
                    : ControlZone.HeightInt,
                ControlZone.Height
            )
        );
#pragma warning restore CS0618 // Type or member is obsolete


    private List<IOxControl> GetControls(OxDockType dockType) =>
        Box.OxControls.Controls(dockType);

    private bool RealignDockedControls()
    {
        OxRectangle currentBounds;

        foreach (IOxControl control in GetControls(OxDockType.Docked))
        {
            if (ControlZone.IsEmpty)
                return false;

            currentBounds = new(ControlZone);

            if (!control.Visible)
                continue;

            if (control is IOxBox childBox
                && !childBox.HandleParentPadding
                && Box is IOxWithPadding boxWithPadding
                && !boxWithPadding.Padding.IsEmpty)
                currentBounds += boxWithPadding.Padding;

            OxSize realControlSize = GetRealControlSize(control);

            switch (control.Dock)
            {
                case OxDock.Right:
                    currentBounds.X = OxWh.S(ControlZone.Right, realControlSize.Width);
                    break;
                case OxDock.Bottom:
                    currentBounds.Y = OxWh.S(ControlZone.Bottom, realControlSize.Height);
                    break;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            switch (OxDockHelper.Variable(control.Dock))
            {
                case OxDockVariable.Width:
                    currentBounds.Width = OxWh.Min(realControlSize.Width, ControlZone.Width);
                    break;
                case OxDockVariable.Height:
                    currentBounds.Height = OxWh.Min(realControlSize.Height, ControlZone.Height);
                    break;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            SetControlBounds(control, currentBounds);
            SubstractControlFromControlZone(control, realControlSize);
        }

        return true;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private static void SetControlBounds(IOxControl control, OxRectangle newBounds)
    {
        OxRectangle oldBounds = new(
            control.OriginalLeft, 
            control.OriginalTop, 
            control.OriginalWidth, 
            control.OriginalWidth);

        if (oldBounds.Equals(newBounds))
            return;

        control.OriginalLeft = newBounds.XInt;
        control.OriginalTop = newBounds.YInt;
        control.OriginalWidth = newBounds.WidthInt;
        control.OriginalHeight = newBounds.HeightInt;

        if (control is IOxBox box)
            box.RealignControls();

        control.Invalidate();
    }

    private void SubstractControlFromControlZone(IOxControl control, OxSize realControlSize)
    {
        switch (control.Dock)
        {
            case OxDock.Fill:
                ControlZone.Clear(); //Use only one fill control in one box
                return;
            case OxDock.Left:
                ControlZone.X = OxWh.A(ControlZone.X, control.OriginalWidth);
                break;
            case OxDock.Top:
                ControlZone.Y = OxWh.A(ControlZone.Y, control.OriginalHeight);
                break;
        }

        switch (OxDockHelper.Variable(control.Dock))
        {
            case OxDockVariable.Width:
                ControlZone.Width = OxWh.S(ControlZone.Width, control.OriginalWidth);
                break;
            case OxDockVariable.Height:
                ControlZone.Height = OxWh.S(ControlZone.Height, control.OriginalHeight);
                break;
        }
    }

    private void RealignUndockedControls(OxRectangle oldControlZone, bool force)
    {
        if (!force
            && oldControlZone.Equals(ControlZone))
            return;

        foreach (IOxControl oxControl in GetControls(OxDockType.Undocked))
        {
            oxControl.OriginalLeft += ControlZone.XInt + OuterControlZone.XInt - oldControlZone.XInt;
            oxControl.OriginalTop += ControlZone.YInt + OuterControlZone.YInt - oldControlZone.YInt;

            //TODO: cut width and height if its greater then ControlZone
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}