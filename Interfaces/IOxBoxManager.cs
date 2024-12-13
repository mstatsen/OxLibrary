namespace OxLibrary.Interfaces;

public interface IOxBoxManager : IOxControlManager
{
    OxBool HandleParentPadding { get; }
    bool IsHandleParentPadding => OxB.B(HandleParentPadding);
    OxRectangle InnerControlZone { get; }
    OxRectangle OuterControlZone { get; }
    OxControls OxControls { get; }
    void Realign();
    OxBool Realigning { get; }
    bool IsRealigning => OxB.B(Realigning);
}