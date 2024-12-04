using OxLibrary.Dock;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxControlAligner
    {
        private readonly IOxBox Box;
        public OxRectangle ControlZone => Box.ControlZone;
        public OxRectangle OuterControlZone => Box.OuterControlZone;

        public bool Realigning { get; private set; } = false;

        public OxControlAligner(IOxBox box) =>
            Box = box;

        public void RealignControls(OxDockType dockType = OxDockType.Unknown)
        {
            if (OuterControlZone.IsEmpty
                || Realigning)
                return;

            Realigning = true;

            try
            {
                ControlZone.CopyFrom(OuterControlZone);
                OxRectangle oldControlZone = new(ControlZone);

                Box.DoWithSuspendedLayout(() =>
                {
                    try
                    {
                        if (OxDockTypeHelper.ContainsIn(OxDockType.Docked, dockType))
                            if (RealignDockedControls())
                                return;

                        if (OxDockTypeHelper.ContainsIn(OxDockType.Undocked, dockType))
                            RealignUndockedControls(
                                oldControlZone,
                                dockType is OxDockType.Undocked
                            );
                    }
                    finally
                    {
                        Box.Invalidate();
                    }
                });
            }
            finally
            {
                Realigning = false;
            }
        }

        private OxSize GetRealControlSize(IOxControl control)
        {
            OxDockVariable dockVariable = OxDockHelper.Variable(control.Dock);
            OxWidth realWidth =
                dockVariable is OxDockVariable.Width
                             or OxDockVariable.None
                    ? control.Width
                    : ControlZone.Width;

            OxWidth realHeight =
                dockVariable is OxDockVariable.Height
                             or OxDockVariable.None
                    ? control.Height
                    : ControlZone.Height;

            if (control is IOxWithMargin controlWithMargin
                && !controlWithMargin.Margin.IsEmpty)
            {
                if (dockVariable is OxDockVariable.Width)
                    realWidth = OxWh.A(realWidth, controlWithMargin.Margin.Horizontal);

                if (dockVariable is OxDockVariable.Height)
                    realHeight = OxWh.A(realHeight, controlWithMargin.Margin.Vertical);
            }

            return
                new(
                    OxWh.Min(realWidth, ControlZone.Width),
                    OxWh.Min(realHeight, ControlZone.Height)
                );
        }

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

                if (control is IOxBoxManager childBox
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

                switch (OxDockHelper.Variable(control.Dock))
                {
                    case OxDockVariable.Width:
                        currentBounds.Width = OxWh.Min(control.Width, ControlZone.Width);
                        break;
                    case OxDockVariable.Height:
                        currentBounds.Height = OxWh.Min(control.Height, ControlZone.Height);
                        break;
                }

                SetControlBounds(control, currentBounds);
                SubstractControlFromControlZone(control, realControlSize);
            }

            return true;
        }

        private static void SetControlBounds(IOxControl control, OxRectangle newBounds)
        {
            control.DoWithSuspendedLayout(
                () =>
                {
                    if (!control.Location.Equals(newBounds.Location))
                        control.Location = newBounds.Location;

                    if (!control.Size.Equals(newBounds.Size))
                        control.Size = newBounds.Size;
                }
            );

            if (control is IOxBoxManager box)
                box.RealignControls();

            control.Invalidate();
        }

        private void SubstractControlFromControlZone(IOxControl control, OxSize realControlSize)
        {
            switch (control.Dock)
            {
                case OxDock.Fill:
                    ControlZone.Clear();
                    return;
                case OxDock.Left:
                    ControlZone.X = OxWh.A(ControlZone.X, realControlSize.Width);
                    break;
                case OxDock.Top:
                    ControlZone.Y = OxWh.A(ControlZone.Y, realControlSize.Height);
                    break;
            }

            switch (OxDockHelper.Variable(control.Dock))
            {
                case OxDockVariable.Width:
                    ControlZone.Width = OxWh.S(ControlZone.Width, realControlSize.Width);
                    break;
                case OxDockVariable.Height:
                    ControlZone.Height = OxWh.S(ControlZone.Height, realControlSize.Height);
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
                oxControl.Left = OxWh.S(((Control)oxControl).Left, oldControlZone.X);
                oxControl.Top = OxWh.S(((Control)oxControl).Top, oldControlZone.Y);

                //TODO: cut width and height if its greater then ControlZone
            }
        }
    }
}