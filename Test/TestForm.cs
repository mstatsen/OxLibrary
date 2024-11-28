using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Panels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OxLibrary.Test
{
    public partial class TestForm : OxForm
    {
        public override Bitmap? FormIcon =>
            OxIcons.TestCode;

        private readonly OxPanel frame;
        private readonly OxClickFrame button;
        private readonly OxButton toolBarButton;
        private readonly OxCheckBox bluredCheckBox;
        private readonly OxToolBar<OxButton> toolbar;

        public TestForm()
        {
            InitializeComponent();
            BaseColor = Color.FromArgb(135, 165, 195);
            MoveToScreenCenter();
            toolBarButton = new OxButton("Test action", OxIcons.Cross)
            { 
                Enabled = false
            };
            toolbar = new()
            { 
                Dock = OxDock.Top
            };
            toolbar.AddButton(toolBarButton);
            toolbar.Parent = this;

            frame = new OxFrameWithHeader()
            {
                Dock = OxDock.Fill,
                BlurredBorder = false,
                Parent = this,
                ToolTipText = "This is frame with Dock = Fill",
                Name = "FillFrame",
                Text = "Frame with Dock = Fill"
            };
            bluredCheckBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.W8,
                Width = OxWh.W200,
                Text = "Set parent margin to 4px",
                Visible = false
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            button = new OxButton("OxButton text", OxIcons.Go)
            {
                Parent = frame,
                Enabled = false,
            };
            button.Click += Button_Click;

            SetFrameMarginSize();
            frame.Padding.Size = OxWh.W16;
            MainPanel.Padding.Size = OxWh.W8;
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            OxMessage.ShowInfo("Test click", this);
        }

        private void BluredCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            SetFrameMarginSize();
        }

        private void SetFrameMarginSize()
        {
            frame.Margin.Size = bluredCheckBox.Checked ? OxWh.W4 : OxWh.W0;
        }
    }
}