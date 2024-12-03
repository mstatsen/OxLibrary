using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxControlContainerManager<TContainer> : OxControlManager<TContainer>, IOxControlContainerManager
        where TContainer : Control, new()
    {
        public OxControlContainerManager(TContainer managingControl) : base(managingControl)
        {
            OxControls = new(ManagingControl);
            Aligner = new(ManagingControl);
        }

        public new IOxControlContainer<TContainer> ManagingControl =>
            (IOxControlContainer<TContainer>)base.ManagingControl;

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
            ManagingControl.ControlAdded += ControlAddedHandler;
            ManagingControl.ControlRemoved += ControlRemovedHandler;

            if (ManagingControl is IOxWithPadding controlWithPadding)
                controlWithPadding.Padding.SizeChanged += PaddingSizeChangedHandler;

            if (ManagingControl is IOxWithBorders controlWithBorders)
                controlWithBorders.Borders.SizeChanged += BordersSizeChangedHandler;

            if (ManagingControl is IOxWithMargin controlWithMargin)
                controlWithMargin.Margin.SizeChanged += MarginSizeChangedHandler;

            base.SetHandlers();
        }

        private void ControlRemovedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl)
                return;

            OxControls.Remove(oxControl);
        }

        private void ControlAddedHandler(object? sender, ControlEventArgs e)
        {
            if (e.Control is not IOxControl oxControl
                || e.Control.Equals(ManagingControl))
                return;

            OxControls.Add(oxControl);

            if (ManagingControl is IOxWithColorHelper colorHelperControl)
                colorHelperControl.PrepareColors();
        }

        private void BordersSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
            RealignParent();

        private void PaddingSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
            RealignControls();

        private void MarginSizeChangedHandler(object sender, OxBordersChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            if (OxDockHelper.IsVariableWidth(Dock))
                Width = OxWh.S(OriginalWidth, e.OldValue.Horizontal);

            if (OxDockHelper.IsVariableHeight(Dock))
                Height = OxWh.S(OriginalHeight, e.OldValue.Vertical);

            RealignParent();
        }

        public OxControls OxControls { get; private set; }

        public OxRectangle OuterControlZone
        {
            get
            {
                OxRectangle outerZone = new(ManagingControl.ClientRectangle);

                if (ManagingControl is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (ManagingControl is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (ManagingControl is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;

                return outerZone;
            }
        }

        public bool HandleParentPadding => ManagingControl.HandleParentPadding;
    }
}