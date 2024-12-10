namespace OxLibrary.Geometry
{
    /// <summary>
    /// Helper class for working with short type
    /// </summary>
    public static class OxSH
    {
        /// <summary>
        /// convert value to short
        /// </summary>
        public static short Short(int value) => (short)value;

        /// <summary>
        /// convert value to short
        /// </summary>
        public static short Short(double value) => (short)value;

        /// <summary>
        /// convert value to short
        /// </summary>
        public static short Short(decimal value) => (short)value;


        /// <returns>
        /// trueValue if condition is true, else falseValue
        /// </returns>
        public static short IfElse(bool condition, int trueValue, int falseValue) =>
            Short(condition ? trueValue : falseValue);

        /// <returns>
        /// trueValue if condition is true, else 0
        /// </returns>
        public static short IfElseZero(bool condition, int trueValue) =>
            IfElse(condition, trueValue, 0);


        /// <returns>
        /// sum of value1 and value2
        /// </returns>
        public static short Add(int value1, int value2) =>
            Short(value1 + value2);

        /// <returns>
        /// sum of value1, value2 and value3
        /// </returns>
        public static short Add(int value1, int value2, int value3) =>
            Short(value1 + value2 + value3);

        /// <returns>
        /// sum of value1, value2, value3 and value4
        /// </returns>
        public static short Add(int value1, int value2, int value3, int value4) =>
            Short(value1 + value2 + value3 + value4);

        /// <returns>
        /// sum of value1, value2, value3, value4 and value5
        /// </returns>
        public static short Add(int value1, int value2, int value3, int value4, int value5) =>
            Short(value1 + value2 + value3 + value4 + value5);

        /// <returns>
        /// sum of value1, value2, value3, value4, value5 and value6
        /// </returns>
        public static short Add(int value1, int value2, int value3, int value4, int value5, int value6) =>
            Short(value1 + value2 + value3 + value4 + value5 + value6);


        /// <returns>minuend - sub1</returns>
        public static short Sub(int minuend, int sub1) =>
            Short(minuend - sub1);

        /// <returns>minuend - sub1 - sub2</returns>
        public static short Sub(int minuend, int sub1, int sub2) =>
            Short(minuend - sub1 - sub2);

        /// <returns>minuend - sub1 - sub2 - sub3</returns>
        public static short Sub(int minuend, int sub1, int sub2, int sub3) =>
            Short(minuend - sub1 - sub2 - sub3);

        /// <returns>minuend - sub1 - sub2 - sub3 - sub4</returns>
        public static short Sub(int minuend, int sub1, int sub2, int sub3, int sub4) =>
            Short(minuend - sub1 - sub2 - sub3 - sub4);

        /// <returns>minuend - sub1 - sub2 - sub3 - sub4 - sub5</returns>
        public static short Sub(int minuend, int sub1, int sub2, int sub3, int sub4, int sub5) =>
            Short(minuend - sub1 - sub2 - sub3 - sub4 - sub5);


        /// <returns>value1 * value2</returns>
        public static short Mul(int value1, int value2) =>
            Short(value1 * value2);

        /// <returns>value1 * value2</returns>
        public static short Mul(int value1, double value2) =>
            Short(value1 * value2);

        /// <returns>value1 * value2</returns>
        public static short Mul(double value1, int value2) =>
            Short(value1 * value2);

        /// <returns>value1 * value2</returns>
        public static short Mul(double value1, double value2) =>
            Short(value1 * value2);


        /// <returns>divisible / divider</returns>
        public static short Div(int divisible, int divider) =>
            Short(divisible / divider);

        /// <returns>divisible / divider</returns>
        public static short Div(int divisible, double divider) =>
            Short(divisible / divider);

        /// <returns>divisible / divider</returns>
        public static short Div(double divisible, int divider) =>
            Short(divisible / divider);

        /// <returns>divisible / divider</returns>
        public static short Div(double divisible, double divider) =>
            Short(divisible / divider);


        /// <returns>value * 2</returns>
        public static short X2(int value) =>
            Mul(value, 2);

        /// <returns>value * 3</returns>
        public static short X3(int value) =>
            Mul(value, 3);


        /// <returns>value / 2</returns>
        public static short Half(int value) =>
            Div(value, 2);

        /// <returns>value / 3</returns>
        public static short Third(int value) =>
            Div(value, 3);


        /// <returns>abs((value1 - value2) / 2)</returns>
        public static short CenterOffset(int value1, int value2)
        {
            int bigValue = value1;
            int smallValue = value2;

            if (value1 < value2)
            {
                bigValue = value2;
                smallValue = value1;
            }

            return Half(Sub(bigValue, smallValue));
        }


        /// <returns>biggest among value1 and value2</returns>
        public static short Max(int value1, int value2) =>
            Short(Math.Max(value1, value2));

        /// <returns>smallest among value1 and value2</returns>
        public static short Min(int value1, int value2) =>
            Short(Math.Min(value1, value2));

        // Rounds a Decimal to an integer value. The Decimal argument is rounded
        // towards positive infinity.
        public static short Ceiling(decimal value) =>
            Short(decimal.Ceiling(value));
    }
}