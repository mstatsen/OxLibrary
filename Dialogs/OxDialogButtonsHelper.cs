namespace OxLibrary.Dialogs
{
    public static class OxDialogButtonsHelper
    {
        public static DialogResult Result(OxDialogButton button) =>
            button switch
            {
                OxDialogButton.OK => DialogResult.OK,
                OxDialogButton.Apply => DialogResult.OK,
                OxDialogButton.ApplyForAll => DialogResult.Continue,
                OxDialogButton.Cancel => DialogResult.Cancel,
                OxDialogButton.Yes => DialogResult.Yes,
                OxDialogButton.No => DialogResult.No,
                _ => DialogResult.None,
            };

        public static List<OxDialogButton> List()
        {
            List<OxDialogButton> list = new();

            foreach (OxDialogButton item in Enum.GetValues(typeof(OxDialogButton)))
                list.Add(item);

            list.Reverse();
            return list;
        }

        public static string Name(OxDialogButton button) =>
            button switch
            {
                OxDialogButton.OK => "OK",
                OxDialogButton.Cancel => "Cancel",
                OxDialogButton.Yes => "Yes",
                OxDialogButton.No => "No",
                OxDialogButton.Apply => "Apply",
                OxDialogButton.ApplyForAll => "Apply for all",
                _ => string.Empty,
            };

        public static Bitmap? Icon(OxDialogButton button) =>
            button switch
            {
                OxDialogButton.Cancel => OxIcons.close,
                OxDialogButton.OK or
                OxDialogButton.Apply => 
                    OxIcons.tick,
                OxDialogButton.ApplyForAll =>
                    OxIcons.double_tick,
                _ => 
                    null,
            };

        public static int Width(OxDialogButton button) =>
            button == OxDialogButton.ApplyForAll ? 140 : 100;
    }
}