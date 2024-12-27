using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Geometry;
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
            //Width = 1000;
            MoveToScreenCenter();
            
            
            toolBarButton = new OxButton("Test action", OxIcons.Cross);
            toolbar = new()
            { 
                Dock = OxDock.Top
            };
            toolbar.AddButton(toolBarButton);
            toolBarButton.Click += ToolBarButton_Click;
            toolBarButton.Height = 36;
            toolbar.Parent = this;
            toolbar.Margin.Left = 1;
            toolbar.SetBorderVisible(true);
            toolbar.Padding.Left = 4;

            card = new OxCard
            {
                BlurredBorder = OxB.F,
                Parent = this,
                Name = "LeftCard",
                Text = "Left docked card",
                Dock = OxDock.Top,
                Height = 150,
                Icon = OxIcons.Tag,
                Width = 400
            };
            card.Margin.Size = 10;
            card.Padding.Size = 18;
            card.HeaderHeight = OxSh.ToDPI(card, 28);

            frame = new OxFrameWithHeader
            {
                BlurredBorder = OxB.T,
                Parent = this,
                Name = "FillFrame",
                Text = "Fill-docked frame with header",
                Dock = OxDock.Fill,
                Width = 400,
                Height = 200
            };
            
            bluredCheckBox = new()
            {
                Parent = card,
                Left = 8,
                Top = 8,
                Text = "Set parent margin to 4px",
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;

            setMarginButton = new OxButton("Set frame dock as Right", OxIcons.Go)
            {
                Left = 8,
                Top = 8,
                Parent = frame,
            };
            //setMarginButton.Width = 140;
            setMarginButton.Click += Button_Click;
            setMarginButton.AutoSize = OxB.T;
            //setMarginButton.Top = OxWh.Add(bluredCheckBox.Bottom, 8);

            hideCardButton = new OxButton("Hide card", OxIcons.Eye)
            {
                Parent = frame,
                Top = (short)(setMarginButton.Bottom + 8),
                Left = setMarginButton.Left,
                Height = OxSh.ToDPI(this, 24),
                Width = OxSh.ToDPI(this, 140)
            };
            hideCardButton.Click += HideCardButton_Click;

            SetFrameMarginSize();
            frame.Click += Frame_Click;
            Padding.Size = 40;

            /*
            colorComboBox = new()

            {
                Parent = frame,
                Left = 8,
                Top = OxWh.Add(hideCardButton.Bottom, 8)
            };

            countryComboBox = new()
            {
                Parent = frame,
                Left = 8,
                Top = OxWh.Add(colorComboBox.Bottom, 8),
                Width = 200,
            };
            countryComboBox.LoadCountries();
            */

            //return;
            buttonEdit = new()
            {
                Parent = frame,
                Left = 8,
                Top = (short)(hideCardButton.Bottom + 8),
                //Height = 70
            };
            buttonEdit.OnButtonClick += ButtonEdit_OnButtonClick;

            textBox = new()
            {
                Parent = frame,
                Left = 8,
                Top = (short)(buttonEdit.Bottom + 8),
                Multiline = true,
                WordWrap = true,
                Height = 40
            };
           
        }

        private void ButtonEdit_OnButtonClick(object? sender, EventArgs e)
        {
            OxMessage.ShowInfo(
                "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button"
                + "Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button Click on buttonEdit Click on buttonEdit Click on buttonEdit Click on buttonEdit' button\n\n\n\n\nA\nB\nC", this);
        }

        private void Frame_Click(object? sender, EventArgs e)
        {
            OxMessage.Confirmation($"Location: {frame.Location}\nSize: {frame.Size}", this);
        }

        private void HideCardButton_Click(object? sender, EventArgs e)
        {
            card.Visible = OxB.Not(card.Visible);
            hideCardButton.Text = card.IsVisible ? "Hide card" : "Show card";
            hideCardButton.AutoSize = OxB.Not(hideCardButton.AutoSize);
        }

        private void ToolBarButton_Click(object? sender, EventArgs e)
        {
            setMarginButton.Enabled = OxB.Not(setMarginButton.Enabled);
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
            frame.Margin.Size = (short)(bluredCheckBox.Checked ? 40 : 0);
        }

        protected override void OnShown(EventArgs e)
        {
            //Width = 800;
            //WindowState = FormWindowState.Maximized;
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