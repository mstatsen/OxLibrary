using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxHeader : OxUnderlinedPanel
    {
        public const int DefaultTitleWidth = 240;
        public const int DefaultTitleHeight = 26;
        private const string DefaultFontFamily = "Calibri Light";
        private const float DefaultFontSize = 11;

        private readonly OxPicture icon = new()
        {
            PictureSize = 24,
            Dock = DockStyle.Left,
            Visible = false
        };

        private readonly OxLabel label = new()
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = string.Empty,
            Font = new Font(DefaultFontFamily, DefaultFontSize + 0.5f, FontStyle.Bold | FontStyle.Italic),
            TextAlign = ContentAlignment.MiddleLeft
        };

        public readonly OxHeaderToolBar ToolBar = new()
        {
            Dock = DockStyle.Right,
            Width = 0
        };

        public OxClickFrameList Buttons
        {
            get => ToolBar.Buttons;
            set => ToolBar.Buttons = value;
        }

        public OxHeader(string title) : base(new Size(DefaultTitleWidth, DefaultTitleHeight))
        {
            label.Text = title;
            label.DoubleClick += (s, e) => ToolBar.ExecuteDefault();
            Paddings.LeftOx = OxSize.Large;
            ReAlign();
        }

        public Label Label => label;

        protected override string GetText() =>
            label.Text;
        protected override void SetText(string? value)
        {
            label.Text = value != null ? value.Trim() : string.Empty;
            label.Visible = label.Text != string.Empty;
        }

        public ContentAlignment TitleAlign
        {
            get => label.TextAlign;
            set => label.TextAlign = value;
        }

        public Image? Icon
        {
            get => icon.Image;
            set => SetIcon(value);
        }

        private void SetIcon(Image? value)
        {
            icon.Image = value;
            icon.Visible = icon.Image != null;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            icon.Parent = ContentContainer;
            ToolBar.Parent = ContentContainer;
            label.Parent = ContentContainer;
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = Colors.Darker(6);
            ToolBar.BaseColor = BaseColor;
        }

        public override void ReAlignControls()
        {
            base.ReAlignControls();
            ToolBar.SendToBack();
            icon.SendToBack();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            icon.Width = icon.Height;
        }

        public void AddToolButton(OxClickFrame button) =>
            ToolBar.AddButton(button);
    }
}