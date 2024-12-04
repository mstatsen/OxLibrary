namespace OxLibrary.Data.Countries;

public class OxCountry
{
    public OxCountry() { }

    private string shortName = string.Empty;
    private string fullName = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string ShortName
    {
        get => shortName.Equals(string.Empty) ? Name : shortName;
        set => shortName = value;
    }
    public string FullName
    {
        get => fullName.Equals(string.Empty) ? Name : fullName;
        set => fullName = value;
    }

    public string Alpha2 { get; set; } = string.Empty;
    public string Alpha3 { get; set; } = string.Empty;
    public string ISO { get; set; } = "000";
    public OxCountryLocation Location { get; set; } = OxCountryLocation.Other;
    public Bitmap? Flag { get; set; }
    public bool IsPSN { get; set; }

    public object? this[OxCountryField field]
    {
        get =>
            field switch
            {
                OxCountryField.Name => Name,
                OxCountryField.ShortName => ShortName,
                OxCountryField.FullName => FullName,
                OxCountryField.Alpha2 => Alpha2,
                OxCountryField.Alpha3 => Alpha3,
                OxCountryField.ISO => ISO,
                OxCountryField.Location => Location,
                OxCountryField.Flag => Flag,
                OxCountryField.IsPSN => IsPSN,
                _ => null,
            };
        set
        {
            string stringValue = 
                value is not null 
                    ? value.ToString()! 
                    : string.Empty;

            switch (field)
            {
                case OxCountryField.Name: 
                    Name = stringValue;
                    break;
                case OxCountryField.ShortName:
                    ShortName = stringValue;
                    break;
                case OxCountryField.FullName:
                    fullName = stringValue;
                    break;
                case OxCountryField.Alpha2:
                    Alpha2 = stringValue;
                    break;
                case OxCountryField.Alpha3:
                    Alpha3 = stringValue;
                    break;
                case OxCountryField.ISO:
                    ISO = stringValue;
                    break;
                case OxCountryField.Location:
                    Location = value is OxCountryLocation countryLocation ? countryLocation : OxCountryLocation.Other;
                    break;
                case OxCountryField.Flag:
                    Flag = value is Bitmap bitmap ? bitmap : null;
                    break;
                case OxCountryField.IsPSN:
                    IsPSN = bool.Parse(stringValue);
                    break;
            }
        }
    }

    public override string ToString() => Name;
}