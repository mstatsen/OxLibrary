using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxBoxManager<TOxControl> :
        OxControlManager,
        IOxBoxManager<TOxControl>
        where TOxControl :
            Control,
            IOxManagingControl<IOxBoxManager<TOxControl>>,
            IOxManagingControl<IOxControlManager>,
            IOxBox<TOxControl>
    {
        public OxBoxManager(Control managingBox) : base(managingBox)
        {
            OxControls = new(Box);
            Aligner = new(Box);
        }

        public TOxControl Box =>
            (TOxControl)ManagingControl;

        private readonly OxControlAligner<TOxControl> Aligner;

        public void RealignControls(OxDockType dockType = OxDockType.Unknown)
        {
            Aligner.RealignControls(dockType);
            ControlZone.CopyFrom(Aligner.ControlZone);
        }

        public bool Realigning =>
            Aligner.Realigning;

        protected override void SetHandlers()
        {
            Box.ControlAdded += ControlAddedHandler;
            Box.ControlRemoved += ControlRemovedHandler;

            if (OxControl is IOxWithPadding controlWithPadding)
                controlWithPadding.Padding.SizeChanged += PaddingSizeChangedHandler;

            base.SetHandlers();
        }

        private void PaddingSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
            RealignControls();

        private void ControlRemovedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl)
                return;

            OxControls.Remove(oxControl);
        }

        private void ControlAddedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl
                || e.Control.Equals(OxControl))
                return;

            OxControls.Add(oxControl);

            if (OxControl is IOxWithColorHelper colorHelperControl)
                colorHelperControl.PrepareColors();
        }

        protected override void RealignParent()
        {
            if (Parent is null)
                RealignControls();
            else
                base.RealignParent();
        }

        public OxControls<TOxControl> OxControls { get; private set; }

        public OxRectangle OuterControlZone
        {
            get
            {
                OxRectangle outerZone = new(OxControl.ClientRectangle);

                if (OxControl is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (OxControl is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (OxControl is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;

                return outerZone;
            }
        }

        public bool HandleParentPadding => Box.HandleParentPadding;
    }
}