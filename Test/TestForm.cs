using OxLibrary.Dialogs;

namespace OxLibrary.Test
{
    public partial class TestForm : OxForm
    {
        public override Bitmap? FormIcon =>
            OxIcons.TestCode;

        public TestForm()
        {
            InitializeComponent();
            BaseColor = Color.FromArgb(116, 138, 138);
            MainPanel.Header.ToolBar.Click += (s, e) => MessageBox.Show("Toolbar");
            MainPanel.Header.Margin.Size = OxWh.W0;
            MainPanel.Header.Click += (s, e) => MessageBox.Show("Header");
            MainPanel.Click += (s, e) => MessageBox.Show("MainPanel");
            Click += (s, e) => MessageBox.Show("Form");
            OxControlHelper.CenterForm(this);
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