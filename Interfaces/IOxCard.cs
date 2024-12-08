namespace OxLibrary.Interfaces;

public interface IOxCard : IOxFrameWithHeader, IOxExpandable
{
    bool ExpandButtonVisible { get; set; }
}