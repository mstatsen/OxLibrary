namespace OxLibrary.Controls
{
    public class OxButton : OxIconButton
    {
        private readonly OxLabel Label = new()
        {
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };

        public static readonly OxWidth DefaultWidth = OxWh.W100;
        public static readonly OxWidth DefaultHeight = OxWh.W20;

        public OxButton() : base() { }
        public OxButton(string? text, Bitmap? icon) : base(icon, DefaultHeight)
        {
            Size = new(DefaultWidth, DefaultHeight);
            Text = text;
            MinimumSize = OxSize.Empty;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Picture.Dock = OxDock.Left;
            Picture.WidthInt = 16;
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

        protected override void SetText(string value)
        {
            Label.Text = value;
            Label.Visible = !value.Equals(string.Empty);
            CalcLabelWidth();
            RecalcPaddings();
        }

        private void RecalcPaddings() => 
            Padding.Left = OxWh.Max(
                OxWh.Div(
                    OxWh.Sub(
                        Width,
                        RealPictureWidth | RealLabelWidth),
                    OxWh.W2),
                OxWh.W0);

        private void CalcLabelWidth()
        {
            if (Label is null)
                return;

            Label.AutoSize = true;
            int calcedLabelWidth = Label.Width;
            Label.AutoSize = false;
            calcedLabelWidth = calcedLabelWidth + (int)RealPictureWidth < WidthInt 
                ? calcedLabelWidth 
                : WidthInt - (int)RealPictureWidth;
            Label.Width = Math.Max(calcedLabelWidth, 0);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalcLabelWidth();
        }

        private OxWidth RealPictureWidth => Picture.Visible ? Picture.Width : OxWh.W0;
        private OxWidth RealLabelWidth => Label.Visible ? OxWh.W(Label.Width) : OxWh.W0;

        protected override void SetWidth(OxWidth value)
        {
            base.SetWidth(value);
            CalcLabelWidth();
            RecalcPaddings();
            base.SetWidth(value);
        }

        protected override OxWidth GetCalcedWidth() => 
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