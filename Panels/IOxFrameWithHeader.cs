namespace OxLibrary.Panels
{
    public interface IOxFrameWithHeader : IOxFrame
    {
        OxHeader Header { get; }
    }
}