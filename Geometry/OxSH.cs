namespace OxLibrary.Geometry
{
    /// <summary>
    /// Helper class for working with short type
    /// </summary>
    public static class OxSH
    {
        public static short Short(int value) => (short)value;

        public static short IfElse(bool condition, int trueValue, int falseValue) =>
            (short)(condition ? trueValue : falseValue);
        public static short IfElseZero(bool condition, int trueValue) =>
            (short)(condition ? trueValue : 0);

        public static short Add(int value1, int value2) => (short)(value1 + value2);
        public static short Add(int value1, int[] value2)
        {
            foreach (int value in value2)
                value1 += value;

            return (short)value1;
        }

        public static short Sub(int value1, int value2) => (short)(value1 - value2);
        public static short Sub(int value1, int[] value2)
        {
            foreach (int value in value2)
                value1 += value;

            return (short)value1;
        }

        public static short Mul(int value1, int value2) => (short)(value1 * value2);
        public static short Mul(int value1, double value2) => (short)(value1 * value2);
        public static short Mul(double value1, int value2) => (short)(value1 * value2);
        public static short Mul(double value1, double value2) => (short)(value1 * value2);

        public static short Div(int value1, int value2) => (short)(value1 / value2);
        public static short Div(int value1, double value2) => (short)(value1 / value2);
        public static short Div(double value1, int value2) => (short)(value1 / value2);
        public static short Div(double value1, double value2) => (short)(value1 / value2);
        public static short Half(int value) => Div(value, 2);

        public static short Max(int value1, int value2) => (short)Math.Max(value1, value2);
        public static short Min(int value1, int value2) => (short)Math.Min(value1, value2);

        public static short Ceiling(decimal value) => (short)decimal.Ceiling(value);

        public static short Double(int value) => Mul(value, 2);
    }
}
