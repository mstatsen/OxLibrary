using OxLibrary.Panels;

namespace OxLibrary.Interfaces
{
    public interface IOxWithMargin
    {
        OxBorders Margin { get; }
        bool BlurredBorder { get; set; }
    }
}