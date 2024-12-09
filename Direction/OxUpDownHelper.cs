namespace OxLibrary.Controls;

public static class OxUpDownHelper
{
    public static short Delta(OxUpDown upDown) =>
        upDown switch
        {
            OxUpDown.Up => -1,
            OxUpDown.Down => 1,
            _ => 0,
        };
}