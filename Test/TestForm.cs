using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Panels;

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
           
            toolBarButton = new OxButton("Test action", OxIcons.Cross);
            toolbar = new()
            { 
                Dock = OxDock.Top
            };
            toolbar.AddButton(toolBarButton);
            toolBarButton.Click += ToolBarButton_Click;
            toolbar.Parent = this;
            toolbar.Margin.Left = OxWh.W4;

            frame = new OxFrameWithHeader
            {
                BlurredBorder = false,
                Parent = this,
                ToolTipText = "This is frame with Dock = Fill",
                Name = "FillFrame",
                Text = "Frame with Dock = Fill",
                Dock = OxDock.Left,
                Width = OxWh.W400
            };

            bluredCheckBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.W8,
                Width = OxWh.W200,
                Text = "Set parent margin to 4px",
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            button = new OxButton("OxButton text", OxIcons.Go)
            {
                Parent = frame,
                Top = OxWh.Add(bluredCheckBox.Bottom, OxWh.W8),
                Left = bluredCheckBox.Left,
                Height = OxWh.W36
            };
            button.Click += Button_Click;

            SetFrameMarginSize();
            //MainPanel.Padding.Size = OxWh.W100;
            MainPanel.Borders.Size = OxWh.W12;
            MainPanel.Margin.Size = OxWh.W20;
        }

        private void ToolBarButton_Click(object? sender, EventArgs e)
        {
            button.Enabled = !button.Enabled;
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            frame.Dock = frame.Dock is OxDock.Fill ? OxDock.Left : OxDock.Fill;
        }

        private void BluredCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            SetFrameMarginSize();
        }

        private void SetFrameMarginSize()
        {
            frame.Margin.Size = bluredCheckBox.Checked ? OxWh.W40 : OxWh.W4;
        }
    }
}