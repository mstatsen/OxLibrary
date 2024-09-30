namespace OxLibrary.Controls
{
    public static class OxToolbarActionHelper
    {
        public static Bitmap? Icon(OxToolbarAction action) => 
            action switch
            {
                OxToolbarAction.New => OxIcons.PlusThick,
                OxToolbarAction.Edit => OxIcons.Pencil,
                OxToolbarAction.Copy => OxIcons.Copy,
                OxToolbarAction.Delete => OxIcons.Trash,
                OxToolbarAction.Update => OxIcons.Batch_edit,
                OxToolbarAction.Save => OxIcons.Save,
                OxToolbarAction.Export => OxIcons.Export,
                OxToolbarAction.ExportSelected => OxIcons.Export,
                OxToolbarAction.Settings => OxIcons.Settings,
                _ => null
            };

        public static bool ActionForExistItems(OxToolbarAction action) =>
            action == OxToolbarAction.Edit
            || action == OxToolbarAction.Copy
            || action == OxToolbarAction.Delete
            || action == OxToolbarAction.Update
            || action == OxToolbarAction.ExportSelected;

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
                OxToolbarAction.ExportSelected => "Export Selected",
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