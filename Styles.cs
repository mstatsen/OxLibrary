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
        public static DataGridViewCellStyle Cell_Default =>
            new()
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                WrapMode = DataGridViewTriState.True,
                Font = new(FontFamily, 10),
            };
        public static DataGridViewCellStyle Cell_LeftAlignment { get; private set; }
        public const int DefaultControlHeight = 24;

        public static readonly Color FieldsColor = Color.FromArgb(195, 145, 195);
        public static readonly Color CardColor = DefaultGridRowColor;
        public static Font DefaultFont => new(FontFamily, DefaultFontSize, FontStyle.Regular);
        public static Font Font(float size)
        {
            if (size < 0)
                size = DefaultFontSize - size;

            return new(FontFamily, size, FontStyle.Regular);
        }

        public static Font Font(FontStyle fontStyle) => new(FontFamily, DefaultFontSize, fontStyle);
        public static Font Font(float size, FontStyle fontStyle)
        {
            if (size < 0)
                size = DefaultFontSize - size;

            return new(FontFamily, size, fontStyle);
        }

        static Styles()
        {
            Cell_LeftAlignment = Cell_Default.Clone();
            Cell_LeftAlignment.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
    }
}