namespace OxLibrary.Data.Countries
{
    public static class CountryLocationHelper
    {
        public static string Name(CountryLocation part) =>
            part switch
            {
                CountryLocation.Africa => "Africa",
                CountryLocation.Asia => "Asia",
                CountryLocation.NorthAmerica => "North America",
                CountryLocation.SouthAmerica => "South America",
                CountryLocation.Europe => "Europe",
                CountryLocation.Oceania => "Oceania",
                CountryLocation.Antarctica => "Antarctica",
                CountryLocation.Australia => "Australia",
                _ => string.Empty,
            };

        public static CountryLocation Part(string name)
        {
            foreach (CountryLocation part in Enum.GetValues(typeof(CountryLocation)))
                if (Name(part) == name)
                    return part;

            return CountryLocation.Other;
        }
    }
}
