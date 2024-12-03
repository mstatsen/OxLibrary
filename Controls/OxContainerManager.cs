using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxContainerManager :
        OxControlManager,
        IOxContainerManager
    {
        public OxContainerManager(Control managingControl) : base(managingControl)
        {
            OxControls = new(Container);
            Aligner = new(Container);
        }

        public IOxContainer Container =>
            (IOxContainer)ManagingControl;

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
            Container.ControlAdded += ControlAddedHandler;
            Container.ControlRemoved += ControlRemovedHandler;

            if (Container is IOxWithPadding controlWithPadding)
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
                || e.Control.Equals(Container))
                return;

            OxControls.Add(oxControl);

            if (Container is IOxWithColorHelper colorHelperControl)
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
                OxRectangle outerZone = new(Container.ClientRectangle);

                if (Container is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (Container is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (Container is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;

                return outerZone;
            }
        }

        public bool HandleParentPadding => Container.HandleParentPadding;
    }
}