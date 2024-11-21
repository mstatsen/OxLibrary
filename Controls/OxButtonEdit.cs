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
            new (OxIcons.Elipsis, OxWh.W16)
            {
                Dock = OxDock.Right,
                Cursor = Cursors.Default
            };

        public OxButtonEdit() : base(new(OxWh.W120, OxWh.W22)) { }

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
            Button.Size = new(
                OxWh.Min(OxWh.W26, TextBox.Height - 4), 
                TextBox.Height
            );
            Button.Borders.Left = OxWh.W0;
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

        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            OxWidth calcedButtonWidth = OxWh.Min(OxWh.W22, TextBox.Height - 4);
            Button.Size = new(calcedButtonWidth, OxWh.Sub(Height, OxWh.W2));
            Button.Width = calcedButtonWidth;
            Button.ReAlign();
            return e.Changed;
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
            TextBox.Text.Trim().Equals(string.Empty);

        public bool ReadOnly
        {
            get => Button.Visible;
            set => Button.Visible = value;
        }
    }
}
