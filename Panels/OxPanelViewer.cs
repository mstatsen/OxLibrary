using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPanelViewer : OxDialog
    {
        public OxPanelViewer(OxPane contentPanel, OxDialogButton buttons) : base()
        {
            ButtonsWithBorders = new List<OxIconButton>();
            ContentPanel = contentPanel;
            Text = ContentPanel.Text;
            DialogButtons = buttons;
            BaseColor = ContentPanel.BaseColor;
            contentPanel.Parent = MainPanel;
            Size = ContentPanel.Size.Size;
            MainPanel.Padding.Size = OxWh.W4;
            contentPanel.Colors.BaseColorChanged += (s, e) => MainPanel.BaseColor = ContentPanel.BaseColor;
        }

        public override Bitmap? FormIcon => ContentPanel.Icon;

        public List<OxIconButton> ButtonsWithBorders { get; }
        private readonly OxPane ContentPanel;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ContentPanel.PutBack(this);
        }
    }
}