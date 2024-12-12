using OxLibrary.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace OxLibrary
{
    public static class OxTextHelper
    {
        public static string ToString(object? obj) =>
            obj is null
            || obj.ToString() is null
                ? string.Empty
                : obj.ToString()!;

        public static OxSize Messure(string text, Font font) =>
            new(TextRenderer.MeasureText(text, font));

        public static short Width(string text, Font font) =>
            Messure(text, font).Width;

        public static short Height(string text, Font font) =>
            Messure(text[0].ToString(), font).Height;

        public static short CalcedHeight(IOxControl control, short boundsWidth) =>
            CalcedHeight(control.Text, control.Font, boundsWidth);

        public static short CalcedHeight(string text, Font font, short boundsWidth)
        {
            short lineHeight = Height(text, font);

            if (text.Equals(string.Empty)
                || boundsWidth <= 0)
                return 0;

            short calcedHeight = lineHeight;
            short calcedWidth;
            short index = 0;
            string calcedText = string.Empty;
            string currentLineText = string.Empty;
            bool isNewLine = false;

            while (index < text.Length)
            {
                try
                {
                    if (text[index].Equals('\n')
                        || text[index].Equals('\r'))
                    {
                        if (isNewLine)
                            continue;
                        else
                        {
                            isNewLine = true;
                            currentLineText = string.Empty;
                            calcedHeight += lineHeight;
                        }
                    }
                    else
                        isNewLine = false;

                    calcedText += text[index].ToString();
                    currentLineText += text[index].ToString();
                    calcedWidth = Width(currentLineText, font);

                    if (calcedWidth > boundsWidth)
                    {
                        calcedHeight += lineHeight;
                        currentLineText = string.Empty;
                    }
                }
                finally
                {
                    index++;
                }
            }

            return calcedHeight;
        }
    }
}