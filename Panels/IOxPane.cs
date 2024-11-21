using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public interface IOxPane : IOxWithIcon
    {
        void StartSizeChanging();
        void EndSizeChanging();
        Color BaseColor { get; set; }
        OxWidth CalcedWidth { get; }
        OxWidth CalcedHeight { get; }
        void ReAlignControls();
        void ReAlign();
        bool Enabled { get; set; }
        OxColorHelper Colors { get; }
        Color DefaultColor { get; }
        string Text { get; set; }
        string Name { get; set; }
        OxDock Dock { get; set; }
        OxSize Size { get; set; }
        OxSize MinimumSize { get; set; }
        OxSize MaximumSize { get; set; }
        //OxPane? Parent { get; set; }
        Control? Parent { get; set; }
        void Dispose();

        OxWidth Bottom { get; }
        OxWidth Right { get; }
        OxWidth Top { get; set; }
        OxWidth Left { get; set; }

        int BottomInt { get; }
        int RightInt { get; }
        int TopInt { get; set; }
        int LeftInt { get; set; }

        bool Visible { get; set; }
        object Tag { get; set; }
        void Update();
        void BringToFront();
        void SendToBack();
        event EventHandler? VisibleChanged;
        event EventHandler? SizeChanged;
        OxWidth Width { get; set; }
        OxWidth Height { get; set; }
        int WidthInt { get; set; }
        int HeightInt { get; set; }

        bool IsHovered { get; }
        OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK);
        DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK);
        OxBorders Padding { get; }
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        OxWidth BorderWidth { get; set; }
        bool BorderVisible { get; set; }
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
        Color BackColor { get; set; }
    }
}