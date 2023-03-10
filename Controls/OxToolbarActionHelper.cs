namespace OxLibrary.Controls
{
    public static class OxToolbarActionHelper
    {
        public static Bitmap? Icon(OxToolbarAction action) => 
            action switch
            {
                OxToolbarAction.New => OxIcons.plus_thick,
                OxToolbarAction.Edit => OxIcons.pencil,
                OxToolbarAction.Copy => OxIcons.copy,
                OxToolbarAction.Delete => OxIcons.trash,
                OxToolbarAction.Update => OxIcons.batch_edit,
                OxToolbarAction.Save => OxIcons.save,
                OxToolbarAction.Export => OxIcons.export,
                OxToolbarAction.Settings => OxIcons.settings,
                _ => null
            };

        public static bool ActionForExistItems(OxToolbarAction action) =>
            action == OxToolbarAction.Edit
            || action == OxToolbarAction.Copy
            || action == OxToolbarAction.Delete
            || action == OxToolbarAction.Update;

        public static string Text(OxToolbarAction action) => 
            action switch
            {
                OxToolbarAction.New => "New",
                OxToolbarAction.Edit => "Edit",
                OxToolbarAction.Copy => "Copy",
                OxToolbarAction.Delete => "Delete",
                OxToolbarAction.Update => "Batch Update",
                OxToolbarAction.Save => "Save",
                OxToolbarAction.Export => "Export",
                OxToolbarAction.Settings => "Settings",
                _ => string.Empty,
            };

        public static int Width(OxToolbarAction action) =>
            action switch
            {
                OxToolbarAction.Update => 140,
                _ => Styles.ToolBarButtonWidth
            };
    }
}