namespace OxLibrary.Controls
{
    public static class OxPictureActionHelper
    {
        public static Bitmap? Icon(OxPictureAction action) => 
            action switch
            {
                OxPictureAction.Download => OxIcons.down,
                OxPictureAction.Clear => OxIcons.eraser,
                _ => null
            };

        public static string Text(OxPictureAction action) =>
            action switch
            {
                OxPictureAction.Download => "Download",
                OxPictureAction.Clear => "Clear",
                _ => string.Empty,
            };

        public static int DefaultWidth => 16;

        public static int DefaultHeight => 24;
    }
}