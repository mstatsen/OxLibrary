using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxHeader : OxUnderlinedPanel, IOxHeader
    {
        public readonly static OxWidth DefaultTitleWidth = OxWh.W240;
        public readonly static OxWidth DefaultTitleHeight = OxWh.W26;
        private const string DefaultFontFamily = "Calibri Light";
        private const float DefaultFontSize = 11;

        private readonly OxPicture icon = new()
        {
            PictureSize = OxWh.W24,
            Dock = OxDock.Left,
            Visible = false
        };

        private readonly OxLabel label = new()
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = string.Empty,
            Font = new(DefaultFontFamily, DefaultFontSize + 0.5f, FontStyle.Bold | FontStyle.Italic),
            TextAlign = ContentAlignment.MiddleLeft
        };

        private readonly OxHeaderToolBar toolBar = new()
        {
            Dock = OxDock.Right,
            Width = OxWh.W0
        };

        public OxHeaderToolBar ToolBar => toolBar;

        public OxClickFrameList<OxIconButton> Buttons
        {
            get => ToolBar.Buttons;
            set => ToolBar.Buttons = value;
        }

        public OxHeader(string title) : base(new(DefaultTitleWidth, DefaultTitleHeight))
        {
            label.Text = title;
            label.DoubleClick += (s, e) => ToolBar.ExecuteDefault();
            label.Click += LabelClickHandler;
            ReAlign();
        }

        private void LabelClickHandler(object? sender, EventArgs e) => 
            Click?.Invoke(sender, e);

        public new EventHandler? Click { get; set; }

        public Label Label => label;

        protected override string GetText() =>
            label.Text;

        protected override void SetText(string? value)
        {
            label.Text = 
                value is not null 
                    ? value.Trim() 
                    : string.Empty;
            label.Visible = !label.Text.Equals(string.Empty);
        }

        public ContentAlignment TitleAlign
        {
            get => label.TextAlign;
            set => label.TextAlign = value;
        }
        public Font TitleFont 
        { 
            get => label.Font; 
            set => label.Font = value;
        }
        protected override Bitmap? GetIcon() => (Bitmap?)icon.Image;
        protected override void SetIcon(Bitmap? value)
        {
            icon.Image = value;
            icon.Visible = icon.Image is not null;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            icon.Parent = this;
            ToolBar.Parent = this;
            ToolBar.BorderVisible = false;
            label.Parent = this;
            SendToBack();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = Colors.Darker(6);
            ToolBar.BaseColor = BaseColor;
        }

        public override void ReAlignControls()
        {
            ToolBar.SendToBack();
            icon.SendToBack();
            base.ReAlignControls();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            icon.Width = icon.Height;
        }

        public void AddToolButton(OxIconButton button) =>
            ToolBar.AddButton(button);
    }
}