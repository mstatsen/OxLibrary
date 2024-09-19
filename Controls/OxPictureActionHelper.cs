namespace OxLibrary.Controls
{
    public static class OxPictureActionHelper
    {
        public static Bitmap? Icon(OxPictureAction action) =>
            action switch
            {
                OxPictureAction.Replace => OxIcons.pencil,
                OxPictureAction.Download => OxIcons.down,
                OxPictureAction.Clear => OxIcons.eraser,
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

        public static int DefaultHeight => 20;

        public static OxSize ButtonMargin = OxSize.Medium;

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
}