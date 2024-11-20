using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public interface IOxPane : IOxWithIcon
    {
        void StartSizeRecalcing();
        void EndSizeRecalcing();
        Color BaseColor { get; set; }
        int CalcedWidth { get; }
        int CalcedHeight { get; }
        void ReAlignControls();
        void ReAlign();
        bool Enabled { get; set; }
        OxColorHelper Colors { get; }
        Color DefaultColor { get; }
        string? Text { get; set; }
        DockStyle Dock { get; set; }
        //OxPane? Parent { get; set; }
        Control? Parent { get; set; }
        void Dispose();

        int Bottom { get; }
        int Right { get; }
        int Top { get; set; }
        int Left { get; set; }

        bool Visible { get; set; }
        object Tag { get; set; }
        void Update();
        void BringToFront();
        void SendToBack();
        event EventHandler? VisibleChanged;
        event EventHandler? SizeChanged;
        int Width { get; set; }
        int Height { get; set; }
        bool IsHovered { get; }
        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
        OxBorders Padding { get; }
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        int BorderWidth { get; set; }
        bool BorderVisible { get; set; }
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
        Size Size { get; set; }
        Color BackColor { get; set; }
    }
}