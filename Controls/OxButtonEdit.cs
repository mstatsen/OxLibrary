using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxButtonEdit : OxPane
    {
        public readonly OxTextBox TextBox = new()
        {
            ReadOnly = true
        };

        public readonly OxButton Button = 
            new ("...", null)
            {
                Dock = DockStyle.Right,
                Text = "...",
                Cursor = Cursors.Default
            };

        public OxButtonEdit() : base(new Size(120, 22)) { }

        protected override void PrepareInnerControls() 
        {
            base.PrepareInnerControls();
            PrepareTextBox();
            PrepareButton();
        }

        private void PrepareButton()
        {
            Button.Parent = this;
            Button.BaseColor = BaseColor;
            Button.SetContentSize(TextBox.Height - 4, TextBox.Height);
            Button.Borders.LeftOx = OxSize.None;
        }

        public event EventHandler OnButtonClick
        {
            add => Button.Click += value;
            remove => Button.Click -= value;
        }

        public string? Value
        {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        private void PrepareTextBox()
        {
            TextBox.Parent = this;
            TextBox.Dock = DockStyle.Fill;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            int calcedButtonWidth = Height - 6;
            Button.SetContentSize(calcedButtonWidth, Height - 2);
            Button.Width = calcedButtonWidth;
            Button.ReAlign();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            TextBox.BackColor = Colors.Lighter(6);
            Button.BaseColor = Colors.Darker(1);
        }

        public bool IsEmpty => 
            TextBox.Text.Trim() == string.Empty;

        public bool ReadOnly
        {
            get => Button.Visible;
            set => Button.Visible = value;
        }
    }
}
