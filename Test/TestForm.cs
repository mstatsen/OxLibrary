using OxLibrary.Dialogs;
using OxLibrary.Panels;

namespace OxLibrary.Test
{
    public partial class TestForm : OxForm
    {
        public override Bitmap? FormIcon =>
            OxIcons.TestCode;

        private readonly OxFrame frame;

        public TestForm()
        {
            InitializeComponent();
            BaseColor = Color.FromArgb(135, 165, 195);
            MoveToScreenCenter();
            frame = new OxFrame()
            {
                Parent = this,
                Dock = OxDock.Fill,
                BlurredBorder = false,
                ToolTipText = "This is frame with Dock = Fill",
            };
            frame.Margin.Size = OxWh.W24;
            frame.Click += Frame_Click;
        }

        private void Frame_Click(object? sender, EventArgs e)
        {
            frame.BlurredBorder = !frame.BlurredBorder;
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