using OxLibrary.Interfaces;
using System;
using System.Windows.Forms;
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

        public static short CalcedWidth(IOxControl control) =>
            CalcedWidth(control.Text, control.Font);

        public static short CalcedWidth(string text, Font font)
        {
            string[] lines = text.Split('\n');
            string maximumLine = string.Empty;

            foreach (string line in lines)
                if (line.Length > maximumLine.Length)
                    maximumLine = line;

            return Width(maximumLine, font);
        }

        public static short CalcedHeight(IOxControl control, short boundsWidth) =>
            CalcedHeight(control.Text, control.Font, boundsWidth);

        private static bool IsNewLine(char str) =>
            str.Equals('\n') || str.Equals('\r');

        public static short CalcedHeight(string text, Font font, short boundsWidth)
        {
            if (text.Equals(string.Empty)
                || boundsWidth <= 0)
                return 0;

            short lineHeight = Height(text, font);
            short calcedHeight = lineHeight;
            short calcedWidth;
            short index = 0;
            string calcedText = string.Empty;
            string currentLineText = string.Empty;

            while (index < text.Length)
            {
                try
                {
                    if (IsNewLine(text[index]))
                    {
                        currentLineText = string.Empty;
                        calcedHeight += lineHeight;

                        if ((index < text.Length - 1)
                            && IsNewLine(text[index]))
                        {
                            index++;
                            continue;
                        }
                    }

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