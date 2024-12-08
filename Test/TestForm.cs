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
        private readonly OxCard card;
        private readonly OxButton setMarginButton;
        private readonly OxClickFrame hideCardButton;
        private readonly OxButton toolBarButton;
        private readonly OxCheckBox bluredCheckBox;
        private readonly OxToolBar<OxButton> toolbar;
        private readonly OxColorComboBox colorComboBox;
        private readonly OxCountryComboBox countryComboBox;
        private readonly OxButtonEdit buttonEdit;
        private readonly OxTextBox textBox;

        public TestForm()
        {
            InitializeComponent();
            //BaseColor = Color.FromArgb(135, 165, 195);
            //Width = OxWh.W1000;
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
            toolbar.Margin.Left = OxWh.W1;
            toolbar.BorderVisible = true;
            toolbar.Padding.Left = OxWh.W4;

            card = new OxCard
            {
                BlurredBorder = false,
                Parent = this,
                Name = "LeftCard",
                Text = "Left docked card",
                Dock = OxDock.Top,
                Height = OxWh.W150,
                Icon = OxIcons.Tag,
                Width = OxWh.W400
            };
            card.Margin.Size = OxWh.W10;
            card.Padding.Size = OxWh.W18;
            card.HeaderHeight = OxWh.W40;

            frame = new OxFrameWithHeader
            {
                BlurredBorder = true,
                Parent = this,
                Name = "FillFrame",
                Text = "Fill-docked frame with header",
                Dock = OxDock.Fill,
                Width = OxWh.W400,
                Height = OxWh.W200
            };
            
            bluredCheckBox = new()
            {
                Parent = card,
                Left = OxWh.W8,
                Top = OxWh.W8,
                Text = "Set parent margin to 4px",
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            setMarginButton = new OxButton("Set frame dock as Right", OxIcons.Go)
            {
                Left = OxWh.W8,
                Top = OxWh.W8,
                Parent = frame,
            };
            //setMarginButton.Width = OxWh.W140;
            setMarginButton.Click += Button_Click;
            setMarginButton.AutoSize = true;
            //setMarginButton.Top = OxWh.Add(bluredCheckBox.Bottom, OxWh.W8);

            hideCardButton = new OxButton("Hide card", OxIcons.Eye)
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
            Padding.Size = OxWh.W40;

            /*
            colorComboBox = new()

            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.Add(hideCardButton.Bottom, OxWh.W8)
            };

            countryComboBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.Add(colorComboBox.Bottom, OxWh.W8),
                Width = OxWh.W200,
            };
            countryComboBox.LoadCountries();
            */

            buttonEdit = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.Add(hideCardButton.Bottom, OxWh.W8),
                //Height = OxWh.W70
            };
            buttonEdit.OnButtonClick += ButtonEdit_OnButtonClick;

            textBox = new()
            {
                Parent = frame,
                Left = OxWh.W8,
                Top = OxWh.Add(buttonEdit.Bottom, OxWh.W8),
                Multiline = true,
                WordWrap = true,
                Height = OxWh.W40
            };
           
        }

        private void ButtonEdit_OnButtonClick(object? sender, EventArgs e)
        {
            MessageBox.Show("Click on buttonEdit' button");
        }

        private void Frame_Click(object? sender, EventArgs e)
        {
            MessageBox.Show($"Location: {frame.Location}\nSize: {frame.Size}");
        }

        private void HideCardButton_Click(object? sender, EventArgs e)
        {
            card.Visible = !card.Visible;
            hideCardButton.Text = card.Visible ? "Hide card" : "Show card";
            hideCardButton.AutoSize = !hideCardButton.AutoSize;
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
            frame.Margin.Size = bluredCheckBox.Checked ? OxWh.W40 : OxWh.W0;
        }

        protected override void OnShown(EventArgs e)
        {
            //Width = OxWh.W800;
            WindowState = FormWindowState.Maximized;
            base.OnShown(e);
            //BaseColor = Color.FromArgb(155, 185, 215);
            BaseColor = Color.LightGreen;
        }

        public override void PrepareColors()
        {
            if (card is not null)
                card.BaseColor = Color.FromArgb(195, 145, 125);

            if (frame is not null)
                frame.BaseColor = Color.FromArgb(125, 195, 145);

            //FormPanel.HeaderToolBar.BaseColor = Color.Gray;
        }
    }
}