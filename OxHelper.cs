namespace OxLibrary
{
    public static class OxHelper
    {
        public static bool Changed(object? oldValue, object? newValue) =>
            oldValue is null && newValue is not null
            || oldValue is not null && !oldValue.Equals(newValue);

        public static new bool Equals(object? value1, object? value2) =>
            !Changed(value1, value2);
    }
}