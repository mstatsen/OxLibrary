namespace OxLibrary.Panels
{
    public interface IOxFrame : IOxPanel
    {
        OxBorders_old Borders { get; }
        OxBorders_old Margins { get; }
        int BorderWidth { get; set; }
        Color BorderColor { get; set; }
        bool BorderVisible { set; }
        bool BlurredBorder { get; set; }
    }
}