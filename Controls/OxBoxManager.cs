using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxBoxManager :
        OxControlManager,
        IOxBoxManager
    {
        public OxBoxManager(Control managingBox) : base(managingBox)
        {
            OxControls = new(Box);
            Aligner = new(Box);
        }

        public IOxBox Box =>
            (IOxBox)ManagingControl;

        private readonly OxControlAligner Aligner;

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

            if (Box is IOxWithPadding controlWithPadding)
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
                || e.Control.Equals(Box))
                return;

            OxControls.Add(oxControl);

            if (Box is IOxWithColorHelper colorHelperControl)
                colorHelperControl.PrepareColors();
        }

        protected override void RealignParent()
        {
            if (Parent is null)
                RealignControls();
            else
                base.RealignParent();
        }

        public OxControls OxControls { get; private set; }

        public OxRectangle OuterControlZone
        {
            get
            {
                OxRectangle outerZone = new(Box.ClientRectangle);

                if (Box is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (Box is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (Box is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;

                return outerZone;
            }
        }

        public bool HandleParentPadding => Box.HandleParentPadding;
    }
}