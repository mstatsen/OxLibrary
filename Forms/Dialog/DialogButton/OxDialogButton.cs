namespace OxLibrary.Forms;

[Flags]
public enum OxDialogButton
{
    OK = 1,
    Yes = 2,
    Apply = 4,
    ApplyForAll = 8,
    Save = 16,
    No = 32,
    Discard = 64,
    Cancel = 128,
}