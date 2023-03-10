using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPanelViewer : OxDialog
    {
        public OxPanelViewer(OxPanel contentPanel, OxDialogButton buttons) : base()
        {
            ButtonsWithBorders = new List<OxIconButton>();
            ContentPanel = contentPanel;
            Text = ContentPanel.Text;
            DialogButtons = buttons;
            BaseColor = ContentPanel.BaseColor;
            contentPanel.ContentContainer.Parent = this;
            SetContentSize(ContentPanel.CalcedWidth, contentPanel.CalcedHeight);
            MainPanel.Paddings.SetSize(OxSize.Large);
            contentPanel.Colors.BaseColorChanged += ColorChangeHandler;
        }

        public List<OxIconButton> ButtonsWithBorders { get; }

        private void ColorChangeHandler(object? sender, EventArgs e) =>
            MainPanel.BaseColor = ContentPanel.BaseColor;

        private readonly OxPanel ContentPanel;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ContentPanel.PutBackContentContainer(this);
        }
    }
}