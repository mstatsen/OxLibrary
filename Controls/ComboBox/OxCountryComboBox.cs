using OxLibrary.Data.Countries;

namespace OxLibrary.Controls
{
    public class OxCountryComboBox : OxPicturedComboBox<OxCountry>
    {
        public OxCountryComboBox() { }

        protected override void OnGetToolTip(OxCountry item, out string toolTipTitle, out string toolTipText)
        {
            base.OnGetToolTip(item, out toolTipTitle, out toolTipText);

            if (SelectedItem is not null)
            {
                toolTipTitle = SelectedItem.FullName;
                toolTipText =
                    $"Region: {OxCountryLocationHelper.Name(SelectedItem.Location)}\n" +
                    $"Alpha3: {SelectedItem.Alpha3}\n" +
                    $"Alpha2: {SelectedItem.Alpha2}\n" +
                    $"ISO: {SelectedItem.ISO}\n";
            }
        }

        public void LoadCountries()
        {
            Items.Clear();

            foreach (OxCountry country in OxCountryList.Countries)
                Items.Add(country);
        }

        public void LoadCountries(OxCountryField field, object value)
        {
            Items.Clear();

            foreach (OxCountry country in OxCountryList.GetCountries(field, value))
                Items.Add(country);
        }

        protected override Bitmap? OnGetPicture(OxCountry item) => 
            base.OnGetPicture(item) ?? item.Flag;
    }
}