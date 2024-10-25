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
            Header.BaseColor = PanelViewer == null ? BaseColor : Colors.Darker();
        }

        protected override void SetBordersColor(Color value)
        {
            base.SetBordersColor(value);
            Header.UnderlineColor = value;
        }

        protected virtual void ToolBarButtonVisibleChangeHandler(object? sender, EventArgs e) { }

        protected override int GetCalcedHeight() => 
            base.GetCalcedHeight() + (Header.Visible ? Header.CalcedHeight : 0);

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

            if (Header.Buttons.Count <= 0)
                return;

            Header.ToolBar.Parent = dialog.MainPanel.Header;
            Header.ToolBar.BringToFront();
            Header.ToolBar.BaseColor = dialog.MainPanel.Header.BaseColor;

            foreach (OxIconButton button in Header.Buttons.Cast<OxIconButton>())
            {
                if (button.HiddenBorder)
                    continue;

                dialog.ButtonsWithBorders.Add(button);
                button.HiddenBorder = true;
            }
        }

        internal override void PutBackContentContainer(OxPanelViewer dialog)
        {
            base.PutBackContentContainer(dialog);

            if (Header.ToolBar.Parent != Header)
            {
                Header.ToolBar.Parent = Header;

                foreach (OxIconButton button in Header.ToolBar.Buttons.Cast<OxIconButton>())
                    if (!dialog.ButtonsWithBorders.Contains(button))
                        button.HiddenBorder = false;
            }

            PrepareColors();
        }
    }
}