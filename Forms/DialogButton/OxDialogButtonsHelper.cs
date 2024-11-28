namespace OxLibrary.Forms
{
    public static class OxDialogButtonsHelper
    {
        public static DialogResult Result(OxDialogButton button) =>
            button switch
            {
                OxDialogButton.OK or
                OxDialogButton.Apply or
                OxDialogButton.Save => 
                    DialogResult.OK,
                OxDialogButton.ApplyForAll => 
                    DialogResult.Continue,
                OxDialogButton.Cancel => 
                    DialogResult.Cancel,
                OxDialogButton.Yes => 
                    DialogResult.Yes,
                OxDialogButton.No or
                OxDialogButton.Discard => 
                    DialogResult.No,
               
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
                OxDialogButton.Save => "Save",
                OxDialogButton.Discard => "Discard",
                _ => string.Empty,
            };

        public static Bitmap? Icon(OxDialogButton button) =>
            button switch
            {
                OxDialogButton.Cancel or
                OxDialogButton.No or
                OxDialogButton.Discard =>
                    OxIcons.Close,
                OxDialogButton.OK or
                OxDialogButton.Yes or
                OxDialogButton.Apply or
                OxDialogButton.Save =>
                    OxIcons.Tick,
                OxDialogButton.ApplyForAll =>
                    OxIcons.DoubleTick,
                _ => null,
            };

        public static int WidthInt(OxDialogButton button) =>
            (int)Width(button);

        public static OxWidth Width(OxDialogButton button) =>
            button is OxDialogButton.ApplyForAll 
                ? OxWh.W140
                : OxWh.W100;
    }
}