namespace OxLibrary.Interfaces
{
    public interface IOxWithBorders
    {
        OxBorders Borders { get; }
        Color BorderColor { get; set; }
        void SetBorderWidth(short value);
        void SetBorderWidth(OxDock dock, short value);
        bool BorderVisible { get; set; }
    }
}