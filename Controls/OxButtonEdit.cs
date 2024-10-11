using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxButtonEdit : OxPane
    {
        public readonly OxTextBox TextBox = new()
        {
            ReadOnly = true
        };

        public readonly OxIconButton Button = 
            new (OxIcons.Elipsis, 16)
            {
                Dock = DockStyle.Right,
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
            Button.SetContentSize(Math.Min(26, TextBox.Height - 4), TextBox.Height);
            Button.Borders.LeftOx = OxSize.None;
            Button.FixBorderColor = true;
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
            TextBox.Multiline = true;
            TextBox.WordWrap = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            int calcedButtonWidth = Math.Min(22, TextBox.Height - 4);
            Button.SetContentSize(calcedButtonWidth, Height - 2);
            Button.Width = calcedButtonWidth;
            Button.ReAlign();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            TextBox.BackColor = Colors.Lighter(6);
            Button.BaseColor = Colors.Darker();

            if (TextBox.Focused)
                Button.BorderColor = Color.LightBlue;
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
