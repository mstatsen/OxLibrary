using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxFrameWithHeader : OxFrame, IOxFrameWithHeader
    {
        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            Header.Parent = this;
            Header.ToolBar.OnButtonVisibleChange += ToolBarButtonVisibleChangeHandler;
        }

        public OxHeader Header { get; } = new(string.Empty)
        {
            Dock = DockStyle.Top
        };

        protected override string? GetText() =>
            Header.Text;

        protected override void SetUseDisabledStyles(bool value)
        {
            base.SetUseDisabledStyles(value);
            Header.UseDisabledStyles = value;
        }

        protected override void SetText(string? value) =>
            Header.Text = value?.Trim();

        public OxFrameWithHeader() : base() { }

        public OxFrameWithHeader(Size contentSize) : base(contentSize) { }


        protected override void PrepareColors()
        {
            base.PrepareColors();
            Header.BaseColor = PanelViewer == null ? BaseColor : Colors.Darker(1);
        }

        protected override void SetBordersColor(Color value)
        {
            base.SetBordersColor(value);
            Header.UnderlineColor = value;
        }

        protected virtual void ToolBarButtonVisibleChangeHandler(object? sender, EventArgs e) { }

        protected override int GetCalcedHeight()
        {
            int calcedHeight = base.GetCalcedHeight();
            calcedHeight += Header.CalcedHeight;
            return calcedHeight;
        }

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            Header.ReAlign();
            Borders.ReAlign();
            Margins.ReAlign();
            SendToBack();
        }

        protected override void PrepareDialog(OxPanelViewer dialog)
        {
            base.PrepareDialog(dialog);

            if (Header.Buttons.Count > 0)
            {
                Header.ToolBar.Parent = dialog.MainPanel.Header;
                Header.ToolBar.BringToFront();
                Header.ToolBar.BaseColor = dialog.MainPanel.Header.BaseColor;

                foreach (OxIconButton button in Header.Buttons)
                    if (!button.HiddenBorder)
                    {
                        dialog.ButtonsWithBorders.Add(button);
                        button.HiddenBorder = true;
                    }
            }
        }

        internal override void PutBackContentContainer(OxPanelViewer dialog)
        {
            base.PutBackContentContainer(dialog);

            if (Header.ToolBar.Parent != Header)
            {
                Header.ToolBar.Parent = Header;

                foreach (OxIconButton button in Header.ToolBar.Buttons)
                    if (!dialog.ButtonsWithBorders.Contains(button))
                        button.HiddenBorder = false;
            }

            PrepareColors();
        }
    }
}