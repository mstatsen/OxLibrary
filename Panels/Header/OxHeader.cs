using OxLibrary.Controls;
using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxHeader : OxUnderlinedPanel, IOxHeader
    {

        private readonly OxPicture icon = new()
        {
            Dock = OxDock.Left,
            Visible = false
        };

        private readonly OxLabel label = new()
        {
            AutoSize = false,
            Dock = OxDock.Fill,
            Text = string.Empty,
            Font = new(
                OxStyles.FontFamily,
                OxStyles.DefaultFontSize + 0.5f, 
                FontStyle.Bold | FontStyle.Italic
            ),
            TextAlign = ContentAlignment.MiddleLeft
        };

        private readonly OxHeaderToolBar toolBar = new()
        {
            Dock = OxDock.Right,
            Width = OxWh.W1
        };

        public OxHeaderToolBar ToolBar => toolBar;

        public OxClickFrameList<OxIconButton> Buttons
        {
            get => ToolBar.Buttons;
            set => ToolBar.Buttons = value;
        }

        public OxHeader(string title = "") : base(new(OxWh.W0, OxWh.W26))
        {
            Dock = OxDock.Top;
            label.Text = title;
            label.DoubleClick += (s, e) => ToolBar.ExecuteDefault();
            label.Click += LabelClickHandler;
        }

        public override OxDock Dock 
        { 
            get => base.Dock; 
            set => base.Dock = OxDock.Top; 
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

        protected override Bitmap? GetIcon() => 
            (Bitmap?)icon.Image;

        protected override void SetIcon(Bitmap? value)
        {
            icon.Image = value;
            icon.Visible = icon.Image is not null;
        }

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            icon.Parent = this;
            ToolBar.Parent = this;
            label.Parent = this;
            //SendToBack();
        }

        public override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = Colors.Darker(6);
            ToolBar.BaseColor = BaseColor;
            icon.BaseColor = BaseColor;
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
            {
                icon.Width = icon.Height;
            }
        }

        public void AddToolButton(OxIconButton button, bool startGroup = false) =>
            ToolBar.AddButton(button, startGroup);

        public override bool HandleParentPadding => false;
    }
}