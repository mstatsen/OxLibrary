namespace OxLibrary.Interfaces;

public interface IOxCard : IOxFrameWithHeader, IOxExpandable
{
    OxBool ExpandButtonVisible { get; set; }
    bool IsExpandButtonVisible => OxB.B(ExpandButtonVisible);
    void SetExpandButtonVisible(bool value);
}