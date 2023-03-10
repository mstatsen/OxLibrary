using System.Drawing;

namespace OxLibrary.Panels
{
    public interface IOxPane
    {
        void StartSizeRecalcing();
        void EndSizeRecalcing();
        Color BaseColor { get; set; }
        int CalcedWidth { get; }
        int CalcedHeight { get; }
        int SavedWidth { get; }
        int SavedHeight { get; }
        void SetContentSize(int width, int height);
        void SetContentSize(Size size);
        void ReAlignControls();
        void ReAlign();
        bool Enabled { get; set; }
        OxColorHelper Colors { get; }
        Color DefaultColor { get; }
        string? Text { get; set; }
        DockStyle Dock { get; set; }
        Control Parent { get; set; }
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
        int Width { get; set; }
        int Height { get; set; }
    }
}