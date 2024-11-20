namespace OxLibrary.Panels
{
    public interface IOxFrameWithHeader : IOxPane
    {
        OxHeader Header { get; }
        OxWidth HeaderHeight { get; set; }
        bool HeaderVisible { get; set; }
        Font HeaderFont { get; set; }
    }
}