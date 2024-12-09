#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace OxLibrary.Controls;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class OxPictureActionHelper
{
    public static Bitmap? Icon(OxPictureAction action) =>
        action switch
        {
            OxPictureAction.Replace => OxIcons.Pencil,
            OxPictureAction.Download => OxIcons.Down,
            OxPictureAction.Clear => OxIcons.Eraser,
            _ => null
        };

    public static string Text(OxPictureAction action) =>
        action switch
        {
            OxPictureAction.Replace => "Replace",
            OxPictureAction.Download => "Download",
            OxPictureAction.Clear => "Clear",
            _ => string.Empty,
        };

    public static short DefaultHeight => 20;

    public static readonly short ButtonMargin = 2;

    public static List<OxPictureAction> List
    {
        get
        {
            List<OxPictureAction> list = new();

            foreach (OxPictureAction action in Enum.GetValues(typeof(OxPictureAction)))
                list.Add(action);

            list.Reverse();
            return list;
        }
    }
}