namespace OxLibrary.Dialogs
{
    public static class MessageTypeHelper
    {
        public static string Caption(MessageType type) =>
            type switch
            {
                MessageType.Info => "Information",
                MessageType.Warning => "Warning",
                MessageType.Error => "Error",
                MessageType.Confirmation => "Confirm",
                _ => "Message",
            };

        public static Bitmap Icon(MessageType type) =>
            type switch
            {
                MessageType.Info => OxIcons.Info,
                MessageType.Warning => OxIcons.Warning,
                MessageType.Error => OxIcons.Error,
                MessageType.Confirmation => OxIcons.Question,
                _ => OxIcons.Info,
            };

        public static Color BaseColor(MessageType type) =>
            type switch
            {
                MessageType.Info => Color.FromArgb(146, 143, 170),
                MessageType.Warning => Color.FromArgb(176, 173, 140),
                MessageType.Error => Color.FromArgb(176, 143, 140),
                MessageType.Confirmation => Color.FromArgb(176, 173, 140),
                _ => Color.FromArgb(146, 143, 140),
            };
    }
}