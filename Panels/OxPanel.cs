using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPanel : OxPane, IOxPanel
    {
        private OxBorders paddings = default!;
        private readonly OxPane contentContainer = new()
        {
            Visible = true,
            Dock = DockStyle.Fill
        };

        public OxPanel() : base() { }
        public OxPanel(Size contentSize) : base(contentSize) { }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            paddings = new OxBorders(this);
            contentContainer.Parent = this;
            contentContainer.Colors.BaseColorChanged += ContentContainerBaseColorChangedHandler;
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            paddings.SizeChanged += BorderSizeEventHandler;
        }

        protected void BorderSizeEventHandler(object sender, BorderEventArgs e) =>
            RecalcSize();


        public OxBorders Paddings => paddings;
        public OxPane ContentContainer => contentContainer;

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            SendToBack();
        }

        protected override int GetCalcedHeight() =>
            base.GetCalcedHeight() +
                paddings.CalcedSize(OxDock.Top) +
                paddings.CalcedSize(OxDock.Bottom);

        protected override int GetCalcedWidth() =>
            base.GetCalcedWidth() +
                paddings.CalcedSize(OxDock.Left) +
                paddings.CalcedSize(OxDock.Right);

        protected override void PrepareColors()
        {
            base.PrepareColors();
            ContentContainer.BaseColor = BaseColor;
        }

        protected override void SetUseDisabledStyles(bool value)
        {
            base.SetUseDisabledStyles(value);
            ContentContainer.UseDisabledStyles = value;
        }

        private void ContentContainerBaseColorChangedHandler(object? sender, EventArgs e) =>
            Paddings.Color = BackColor;

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (Initialized)
                ContentContainer.Controls.Add(e.Control);
            else
                base.OnControlAdded(e);
        }

        protected OxPanelViewer? PanelViewer;

        public OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK)
        {
            PanelViewer = new OxPanelViewer(this, buttons);
            PanelViewer.ButtonsWithBorders.Clear();
            PrepareDialog(PanelViewer);
            return PanelViewer;
        }

        protected virtual void PrepareDialog(OxPanelViewer dialog) { }

        internal virtual void PutBackContentContainer(OxPanelViewer dialog)
        {
            foreach (Form form in dialog.OwnedForms)
                dialog.RemoveOwnedForm(form);

            Initialized = false;
            ContentContainer.Parent = this;
            Initialized = true;
        }

        public DialogResult ShowAsDialog(OxDialogButton buttons = OxDialogButton.OK)
        {
            DialogResult result = AsDialog(buttons).ShowDialog();

            if (PanelViewer != null)
                PanelViewer.Dispose();

            PanelViewer = null;
            return result;
        }
    }
}