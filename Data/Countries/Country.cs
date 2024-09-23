namespace OxLibrary.Data.Countries
{
    public class Country
    {
        public Country() { }

        private string shortName = string.Empty;
        private string fullName = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string ShortName
        {
            get => shortName ?? Name;
            set => shortName = value;
        }
        public string FullName
        {
            get => fullName ?? Name;
            set => fullName = value;
        }

        public string Alpha2 { get; set; } = string.Empty;
        public string Alpha3 { get; set; } = string.Empty;
        public string ISO { get; set; } = "0";
        public CountryLocation Location { get; set; } = CountryLocation.Other;
        public Bitmap? Flag { get; set; }

        public object? this[CountryField field]
        {
            get =>
                field switch
                {
                    CountryField.Name => Name,
                    CountryField.ShortName => ShortName,
                    CountryField.FullName => FullName,
                    CountryField.Alpha2 => Alpha2,
                    CountryField.Alpha3 => Alpha3,
                    CountryField.ISO => ISO,
                    CountryField.Location => Location,
                    CountryField.Flag => Flag,
                    _ => null,
                };
            set
            {
                string stringValue = value != null ? value.ToString()! : string.Empty;

                switch (field)
                {
                    case CountryField.Name: 
                        Name = stringValue;
                        break;
                    case CountryField.ShortName:
                        ShortName = stringValue;
                        break;
                    case CountryField.FullName:
                        fullName = stringValue;
                        break;
                    case CountryField.Alpha2:
                        Alpha2 = stringValue;
                        break;
                    case CountryField.Alpha3:
                        Alpha3 = stringValue;
                        break;
                    case CountryField.ISO:
                        ISO = stringValue;
                        break;
                    case CountryField.Location:
                        Location = value is CountryLocation countryLocation ? countryLocation : CountryLocation.Other;
                        break;
                    case CountryField.Flag:
                        Flag = value is Bitmap bitmap ? bitmap : null;
                        break;
                }
            }
        }
    }
}
