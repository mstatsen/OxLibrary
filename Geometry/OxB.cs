namespace OxLibrary
{
    public static class OxB
    {
        public readonly static OxBool T = OxBool.True;
        public readonly static OxBool F = OxBool.False;

        public readonly static OxBool True = T;
        public readonly static OxBool False = F;

        public static OxBool B(bool value) =>
            value ? True : False;
        public static OxBool Bool(bool value) =>
            B(value);

        public static bool B(OxBool value) =>
            value is OxBool.True;
        public static bool Bool(OxBool value) =>
            B(value);


        public static OxBool Not(OxBool value) =>
            Not(B(value));
        public static OxBool Not(bool value) =>
            B(!value);

        public static bool Equals(OxBool value1, OxBool value2) =>
            B(value1).Equals(B(value2));
        public static bool Equals(bool value1, OxBool value2) =>
            value1.Equals(B(value2));
        public static bool Equals(OxBool value1, bool value2) =>
            B(value1).Equals(value2);

        public static OxBool And(OxBool value1, OxBool value2) =>
            And(B(value1), B(value2));
        public static OxBool And(bool value1, OxBool value2) =>
            And(value1, B(value2));
        public static OxBool And(OxBool value1, bool value2) =>
            And(B(value1), value2);
        public static OxBool And(bool value1, bool value2) =>
            B(value1 && value2);

        public static OxBool Or(OxBool value1, OxBool value2) =>
            Or(B(value1), B(value2));
        public static OxBool Or(bool value1, OxBool value2) =>
            Or(value1, B(value2));
        public static OxBool Or(OxBool value1, bool value2) =>
            Or(B(value1), value2);
        public static OxBool Or(bool value1, bool value2) =>
            B(value1 || value2);

    }
}