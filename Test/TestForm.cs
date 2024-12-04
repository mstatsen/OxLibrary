using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Panels;
using System.Reflection.Metadata.Ecma335;

namespace OxLibrary.Test
{
    public partial class TestForm : OxForm
    {
        public override Bitmap? FormIcon =>
            OxIcons.TestCode;

        private readonly OxPanel frame;
        private readonly OxCard card;
        private readonly OxClickFrame setMarginButton;
        private readonly OxClickFrame hideCardButton;
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

            card = new OxCard
            {
                BlurredBorder = false,
                Parent = MainPanel,
                Name = "LeftCard",
                Text = "Left docked card",
                Dock = OxDock.Top,
                Width = OxWh.W200,
                Height = OxWh.W300,
                Icon = OxIcons.Tag
            };
            card.Margin.Size = OxWh.W40;
            card.BaseColor = Color.Red;
            card.Click += Card_Click;

            frame = new OxFrameWithHeader
            {
                BlurredBorder = false,
                Parent = MainPanel,
                Name = "FillFrame",
                Text = "Fill-docked frame with header",
                Dock = OxDock.Right,
                Width = OxWh.W400,
                Height = OxWh.W200
            };
            

            bluredCheckBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.W8,
                Text = "Set parent margin to 4px",
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            setMarginButton = new OxButton("Set frame dock as Right", OxIcons.Go)
            {
                Parent = frame,
                Top = OxWh.Add(bluredCheckBox.Bottom, OxWh.W8),
                Left = bluredCheckBox.Left,
                Height = OxWh.W24,
                Width = OxWh.W140
            };
            setMarginButton.Click += Button_Click;

            hideCardButton = new OxButton("Hide card", OxIcons.Go)
            {
                Parent = frame,
                Top = OxWh.Add(setMarginButton.Bottom, OxWh.W8),
                Left = setMarginButton.Left,
                Height = OxWh.W24,
                Width = OxWh.W140
            };
            hideCardButton.Click += HideCardButton_Click;

            SetFrameMarginSize();
            frame.Click += Frame_Click;
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            MessageBox.Show($"Location: {card.Location}\nSize: {card.Size}");
        }

        private void Frame_Click(object? sender, EventArgs e)
        {
            MessageBox.Show($"Location: {frame.Location}\nSize: {frame.Size}");
        }

        private void HideCardButton_Click(object? sender, EventArgs e)
        {
            card.Visible = !card.Visible;
            hideCardButton.Text = card.Visible ? "Hide card" : "Show card";
        }

        private void ToolBarButton_Click(object? sender, EventArgs e)
        {
            setMarginButton.Enabled = !setMarginButton.Enabled;
        }

        private void Button_Click(object? sender, EventArgs e)
        {
            frame.Dock = frame.Dock is OxDock.Fill ? OxDock.Right : OxDock.Fill;
            setMarginButton.Text = frame.Dock is OxDock.Fill ? "Set frame dock as Right" : "Set frame dock as Fill";
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