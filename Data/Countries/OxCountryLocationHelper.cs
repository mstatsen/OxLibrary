namespace OxLibrary.Data.Countries;

public static class OxCountryLocationHelper
{
    public static string Name(OxCountryLocation part) =>
        part switch
        {
            OxCountryLocation.Africa => "Africa",
            OxCountryLocation.Asia => "Asia",
            OxCountryLocation.NorthAmerica => "North America",
            OxCountryLocation.SouthAmerica => "South America",
            OxCountryLocation.Europe => "Europe",
            OxCountryLocation.Oceania => "Oceania",
            OxCountryLocation.Antarctica => "Antarctica",
            OxCountryLocation.Australia => "Australia",
            _ => string.Empty,
        };

    public static OxCountryLocation Part(string name)
    {
        foreach (OxCountryLocation part in Enum.GetValues(typeof(OxCountryLocation)))
            if (Name(part).Equals(name))
                return part;

        return OxCountryLocation.Other;
    }
}
