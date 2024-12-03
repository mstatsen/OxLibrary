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
        private readonly OxPanel card;
        private readonly OxClickFrame button;
        private readonly OxButton toolBarButton;
        private readonly OxCheckBox bluredCheckBox;
        private readonly OxToolBar<OxButton> toolbar;

        public TestForm()
        {
            InitializeComponent();
            //BaseColor = Color.FromArgb(135, 165, 195);
            MoveToScreenCenter();

            toolBarButton = new OxButton("Test action", OxIcons.Cross);
            toolbar = new()
            { 
                Dock = OxDock.Top
            };
            toolbar.AddButton(toolBarButton);
            toolBarButton.Click += ToolBarButton_Click;
            toolBarButton.Height = OxWh.W36;
            toolbar.Parent = this;
            toolbar.BaseColor = BaseColor;
            toolbar.Margin.Left = OxWh.W4;

            frame = new OxFrameWithHeader
            {
                BlurredBorder = false,
                Parent = MainPanel,
                ToolTipText = "This is frame with Dock = Fill",
                Name = "FillFrame",
                Text = "Fill-docked frame with header",
                Dock = OxDock.Fill,
                Width = OxWh.W400,
                Height = OxWh.W200
            };

            card = new OxCard
            {
                BlurredBorder = false,
                Parent = MainPanel,
                ToolTipText = "This is frame with Dock = Fill",
                Name = "FillFrame",
                Text = "Left docked card",
                Dock = OxDock.Left,
                Width = OxWh.W200,
                Height = OxWh.W300,
                Icon = OxIcons.Tag
            };
            card.Margin.Size = OxWh.W40;
            card.Margin.Right = OxWh.W0;
            card.BaseColor = Color.Red;

            bluredCheckBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.W8,
                //AutoSize = true,
                Text = "Set parent margin to 4px",
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            button = new OxButton("OxButton text", OxIcons.Go)
            {
                Parent = frame,
                Top = OxWh.Add(bluredCheckBox.Bottom, OxWh.W8),
                Left = bluredCheckBox.Left,
                Height = OxWh.W24,
                Width = OxWh.W140
            };
            button.Click += Button_Click;

            
            SetFrameMarginSize();
        }

        private void ToolBarButton_Click(object? sender, EventArgs e)
        {
            button.Enabled = !button.Enabled;
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            frame.Dock = frame.Dock is OxDock.Fill ? OxDock.Right : OxDock.Fill;
        }

        private void BluredCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            SetFrameMarginSize();
        }

        private void SetFrameMarginSize()
        {
            frame.Margin.Size = bluredCheckBox.Checked ? OxWh.W40 : OxWh.W4;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BaseColor = Color.FromArgb(135, 165, 195);
        }

        public override void PrepareColors()
        {
            if (card is not null)
                card.BaseColor = Color.FromArgb(195, 165, 135);
        }
    }
}