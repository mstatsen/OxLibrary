namespace OxLibrary
{
    public static class OxTextHelper
    {
        public static string ToString(object? obj) =>
            obj is null || obj.ToString() is null ? string.Empty : obj.ToString()!;

        public static OxWidth GetTextWidth(string text, Font font) =>
            OxWh.W(TextRenderer.MeasureText(text, font).Width);
    }
}