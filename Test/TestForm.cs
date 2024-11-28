using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Panels;

namespace OxLibrary.Test
{
    public partial class TestForm : OxForm
    {
        public override Bitmap? FormIcon =>
            OxIcons.TestCode;

        private readonly OxFrame frame;
        private readonly OxCheckBox bluredCheckBox;

        public TestForm()
        {
            InitializeComponent();
            BaseColor = Color.FromArgb(135, 165, 195);
            MoveToScreenCenter();
            frame = new()
            {
                Parent = this,
                BlurredBorder = false,
                ToolTipText = "This is frame with Dock = Fill",
                Name = "FillFrame",
                Dock = OxDock.Fill
            };
            frame.Click += Frame_Click;
            bluredCheckBox = new()
            {
                Left = OxWh.W0,
                Top = OxWh.W0,
                Parent = frame
            };
            bluredCheckBox.CheckedChanged += BluredCheckBox_CheckedChanged;
        }

        private void BluredCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            frame.Margin.Size = bluredCheckBox.Checked ? OxWh.W24 : OxWh.W12;
        }

        private void Frame_Click(object? sender, EventArgs e)
        {
            RealignControls();
        }

        private void TestFormShow(object? sender, EventArgs e)
        {
            /*
            try
            {
                Update();

                Size screenSize = Screen.GetWorkingArea(this).Size;
                Size = new(
                    Math.Min(1600, screenSize.Width),
                    Math.Min(800, screenSize.Height));

                Location = new(
                    OxWh.Div(OxWh.Sub(screenSize.Width, Width), OxWh.W2),
                    OxWh.Div(OxWh.Sub(screenSize.Height, Height), OxWh.W2));
                Update();
            }
            finally
            {
                SuspendLayout();
            }
            */
        }
    }
}