namespace OxLibrary.Forms;

public static class OxMessageTypeHelper
{
    public static string Caption(OxMessageType type) =>
        type switch
        {
            OxMessageType.Info => "Information",
            OxMessageType.Warning => "Warning",
            OxMessageType.Error => "Error",
            OxMessageType.Confirmation => "Confirm",
            _ => "Message",
        };

    public static Bitmap Icon(OxMessageType type) =>
        type switch
        {
            OxMessageType.Info => OxIcons.Info,
            OxMessageType.Warning => OxIcons.Warning,
            OxMessageType.Error => OxIcons.Error,
            OxMessageType.Confirmation => OxIcons.Question,
            _ => OxIcons.Info,
        };

    public static Color BaseColor(OxMessageType type) =>
        type switch
        {
            OxMessageType.Info => Color.FromArgb(146, 143, 170),
            OxMessageType.Warning => Color.FromArgb(176, 173, 140),
            OxMessageType.Error => Color.FromArgb(176, 143, 140),
            OxMessageType.Confirmation => Color.FromArgb(176, 173, 140),
            _ => Color.FromArgb(146, 143, 140),
        };
}