using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxFrameWithHeader : OxFrame, IOxFrameWithHeader
    {
        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            Header.Parent = this;
        }

        protected override void SetIcon(Bitmap? value) => 
            Header.Icon = value;

        protected override Bitmap? GetIcon() => 
            Header.Icon;

        public OxWidth HeaderHeight 
        { 
            get => Header.Height;
            set => Header.Height = value; 
        }

        public OxHeader Header { get; } = new();

        public bool HeaderVisible 
        {
            get => Header.Visible;
            set => Header.Visible = value;
        }

        public Font HeaderFont 
        { 
            get => Header.TitleFont;
            set => Header.TitleFont = value;
        }

        protected override string GetText() =>
            Header.Text;

        protected override void SetUseDisabledStyles(bool value)
        {
            base.SetUseDisabledStyles(value);
            Header.UseDisabledStyles = value;
        }

        protected override void SetText(string value) =>
            Header.Text = value.Trim();

        public OxFrameWithHeader() : base() { }

        public OxFrameWithHeader(OxSize size) : base(size) { }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            Header.BaseColor = 
                Colors.Darker(PanelViewer is null ? 0 : 3);
        }

        /*
        protected override OxWidth GetCalcedHeight() => 
            base.GetCalcedHeight() 
            | (HeaderVisible 
                ? HeaderHeight 
                : OxWh.W0);
        */

        protected override void PrepareDialog(OxPanelViewer dialog)
        {
            base.PrepareDialog(dialog);

            if (Header.Buttons.Count <= 0)
                return;

            HeaderToolBar.Parent = dialog.MainPanel.Header;
            HeaderToolBar.BringToFront();
            HeaderToolBar.BaseColor = dialog.MainPanel.Header.BaseColor;

            foreach (OxIconButton button in Header.Buttons.Cast<OxIconButton>())
            {
                if (button.HiddenBorder)
                    continue;

                dialog.ButtonsWithBorders.Add(button);
                button.HiddenBorder = true;
            }
        }

        public OxHeaderToolBar HeaderToolBar => 
            Header.ToolBar;

        public override void PutBack(OxPanelViewer dialog)
        {
            base.PutBack(dialog);

            if (!Header.Equals(HeaderToolBar.Parent))
            {
                HeaderToolBar.Parent = Header;

                foreach (OxIconButton button in HeaderToolBar.Buttons.Cast<OxIconButton>())
                    if (!dialog.ButtonsWithBorders.Contains(button))
                        button.HiddenBorder = false;
            }

            PrepareColors();
        }
    }
}