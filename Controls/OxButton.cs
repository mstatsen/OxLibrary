namespace OxLibrary.Controls
{
    public class OxButton : OxIconButton
    {
        private readonly OxLabel Label = new()
        {
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };

        public const int DefaultWidth = 100;
        public const int DefaultHeight = 20;

        public OxButton() : base() { }
        public OxButton(string? text, Bitmap? icon) : base(icon, DefaultHeight)
        {
            Size = new(DefaultWidth, DefaultHeight);
            Text = text;
            MinimumSize = Size.Empty;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Picture.Dock = DockStyle.Left;
            Picture.Width = 16;
            HiddenBorder = false;
        }

        protected override void SetIcon(Bitmap? value)
        {
            base.SetIcon(value);
            Picture.Visible = value is not null;
            Label.TextAlign = Picture.Visible ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            Label.Parent = this;
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            SetHoverHandlers(Label);
            Label.Click += (s, e) => PerformClick();
            ForeColorChanged += (s, e) => Label.ForeColor = ForeColor;
        }

        public override void ReAlignControls()
        {
            base.ReAlignControls();
            Label.BringToFront();
        }

        protected override string GetText() => 
            Label.Text;

        protected override void SetText(string? value)
        {
            Label.Text = value;
            Label.Visible = !string.Empty.Equals(value);
            CalcLabelWidth();
            RecalcPaddings();
        }

        private void RecalcPaddings()
        {
            int calcedLeftPadding = (Width - (RealPictureWidth + RealLabelWidth)) / 2;
            Padding.LeftInt = calcedLeftPadding > 0 ? calcedLeftPadding : 0;
        }

        private void CalcLabelWidth()
        {
            if (Label is null)
                return;

            Label.AutoSize = true;
            int calcedLabelWidth = Label.Width;
            Label.AutoSize = false;
            calcedLabelWidth = calcedLabelWidth + RealPictureWidth < Width 
                ? calcedLabelWidth 
                : Width - RealPictureWidth;
            Label.Width = Math.Max(calcedLabelWidth, 0);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalcLabelWidth();
        }

        private int RealPictureWidth => Picture.Visible ? Picture.Width : 0;
        private int RealLabelWidth => Label.Visible ? Label.Width : 0;

        protected override void SetWidth(int value)
        {
            Width = value;
            CalcLabelWidth();
            RecalcPaddings();
            Width = value;
        }

        protected override int GetCalcedWidth() => 
            base.GetCalcedWidth() 
            - Padding.LeftInt 
            - Padding.RightInt;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
            {
                CalcLabelWidth();
                RecalcPaddings();
            }
        }

        protected override void SetToolTipText(string value)
        {
            base.SetToolTipText(value);
            ToolTip.SetToolTip(Label, value);
        }
    }
}