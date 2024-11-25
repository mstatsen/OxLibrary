namespace OxLibrary
{
    public delegate Color GetColor();

    public class OxColorHelper
    {
        private const int Default_Grouth = 15;

        private static int ColorPart(int colorPart) =>
            colorPart < 0 
                ? 0 
                : colorPart > 255 
                    ? 255 
                    : colorPart;

        private static Color GrouthColor(Color color, int grouth) =>
            GrouthColor(color, grouth, grouth, grouth);

        private static Color GrouthColor(Color color, int grouthRed, int grouthGreen, int grouthBlue) =>
            Color.FromArgb(
                ColorPart(color.R + grouthRed),
                ColorPart(color.G + grouthGreen),
                ColorPart(color.B + grouthBlue)
            );

        private Color baseColor;

        public OxColorHelper(Color color) =>
            baseColor = color;

        public Color BaseColor
        {
            get => baseColor;
            set
            {
                BaseColorChanging?.Invoke(this, EventArgs.Empty);
                baseColor = Color.FromArgb(value.R, value.G, value.B);
                BaseColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Color Lightest
        {
            get
            {
                Color result = BaseColor;

                while (result.R < 255 && result.G < 255 && result.B < 255)
                    result = GrouthColor(result, 1);

                return result;
            }
        }

        public Color Darkest
        {
            get
            {
                Color result = BaseColor;

                while (result.R > 0 && result.G > 0 && result.B > 0)
                    result = GrouthColor(result, -1);

                return result;
            }
        }

        public Color Lighter(int multiplier = 1) =>
            GrouthColor(baseColor, multiplier * Default_Grouth);

        public Color Darker(int multiplier = 1) =>
            GrouthColor(baseColor, multiplier * -Default_Grouth);

        public Color Redder(int multiplier = 1) =>
            GrouthColor(baseColor, multiplier * Default_Grouth, 0, 0);

        public Color Greener(int multiplier = 1) =>
            GrouthColor(baseColor, 0, multiplier * Default_Grouth, 0);

        public Color Browner(int multiplier = 1) =>
            GrouthColor(baseColor, multiplier * Default_Grouth, multiplier * Default_Grouth, 0);

        public Color Bluer(int multiplier = 1) =>
            GrouthColor(baseColor, 0, 0, multiplier * Default_Grouth);

        public OxColorHelper HLighter(int multiplier = 1)
        {
            baseColor = Lighter(multiplier);
            return this;
        }

        public OxColorHelper HDarker(int multiplier = 1)
        {
            baseColor = Darker(multiplier);
            return this;
        }

        public OxColorHelper HRedder(int multiplier = 1)
        {
            baseColor = Redder(multiplier);
            return this;
        }

        public OxColorHelper HGreener(int multiplier = 1)
        {
            baseColor = Greener(multiplier);
            return this;
        }

        public OxColorHelper HBrowner(int multiplier = 1)
        {
            baseColor = Browner(multiplier);
            return this;
        }

        public OxColorHelper HBluer(int multiplier = 1)
        {
            baseColor = Bluer(multiplier);
            return this;
        }
            
        public event EventHandler? BaseColorChanged;
        public event EventHandler? BaseColorChanging;
    }
}