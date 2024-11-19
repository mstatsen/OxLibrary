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
            MainPanel.Paddings.SetSize(OxSize.S);
            contentPanel.Colors.BaseColorChanged += (s, e) => MainPanel.BaseColor = ContentPanel.BaseColor;
        }

        public override Bitmap? FormIcon => ContentPanel.Icon;

        public List<OxIconButton> ButtonsWithBorders { get; }
        private readonly OxPanel ContentPanel;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ContentPanel.PutBackContentContainer(this);
        }
    }
}