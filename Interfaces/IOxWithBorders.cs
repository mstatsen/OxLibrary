using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{
    public interface IOxWithBorders
    {
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        void SetBorderWidth(OxWidth value);
        void SetBorderWidth(OxDock dock, OxWidth value);
        bool BorderVisible { get; set; }
    }
}