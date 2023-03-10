using System.Drawing;

namespace OxLibrary.Panels
{
    public interface IOxFrame : IOxPanel
    {
        OxBorders Borders { get; }
        OxBorders Margins { get; }
        int BorderWidth { get; set; }
        Color BorderColor { get; set; }
        bool BorderVisible { set; }
        bool BlurredBorder { get; set; }
    }
}