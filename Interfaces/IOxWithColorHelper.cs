namespace OxLibrary.Interfaces
{
    public interface IOxWithColorHelper
    {
        Color BaseColor { get; set; }
        OxColorHelper Colors { get; }
        void PrepareColors();
    }
}