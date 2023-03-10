namespace OxLibrary
{
    public static class Styles
    {
        public const string FontFamily = "Calibri Light";
        public const float DefaultFontSize = 11;
        public const int ToolBarButtonWidth = 100;
        public static readonly Color ElementControlColor = Color.FromArgb(235, 241, 241);
        public static readonly Color DefaultGridRowColor = Color.FromArgb(254, 254, 255);
        public static readonly Color DefaultGridFontColor = Color.Black;
        public static DataGridViewCellStyle Cell_Default { get; private set; }
        public static DataGridViewCellStyle Cell_LeftAlignment { get; private set; }
        public static Font DefaultFont { get; private set; }
        public const int DefaultControlHeight = 24;

        public static readonly Color FieldsColor = Color.FromArgb(195, 145, 195);
        public static readonly Color CardColor = DefaultGridRowColor;

        static Styles()
        {
            Cell_Default = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                WrapMode = DataGridViewTriState.True,
                Font = new Font(FontFamily, 10),
            };

            Cell_LeftAlignment = Cell_Default.Clone();
            Cell_LeftAlignment.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DefaultFont = new Font(FontFamily, DefaultFontSize, FontStyle.Regular);
        }
    }
}