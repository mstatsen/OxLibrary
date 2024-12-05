namespace OxLibrary.Interfaces;

public interface IOxBoxManager : IOxControlManager
{
    bool HandleParentPadding { get; }
    OxRectangle InnerControlZone { get; }
    OxRectangle OuterControlZone { get; }
    OxControls OxControls { get; }
    void Realign();
    bool Realigning { get; }
}