using OxLibrary.Data.Countries;

namespace OxLibrary.Controls
{
    public class OxCountryComboBox : OxPicturedComboBox<Country>
    {
        public OxCountryComboBox() { }

        protected override void OnGetToolTip(Country item, out string toolTipTitle, out string toolTipText)
        {
            base.OnGetToolTip(item, out toolTipTitle, out toolTipText);

            if (SelectedItem != null)
            {
                toolTipTitle = SelectedItem.FullName;
                toolTipText =
                    $"Region: {CountryLocationHelper.Name(SelectedItem.Location)}\n" +
                    $"Alpha3: {SelectedItem.Alpha3}\n" +
                    $"Alpha2: {SelectedItem.Alpha2}\n" +
                    $"ISO: {SelectedItem.ISO}\n";
            }
        }

        public void LoadCounties()
        {
            Items.Clear();

            foreach (Country country in CountryList.Countries)
                Items.Add(country);
        }

        protected override Bitmap? OnGetPicture(Country item) => 
            base.OnGetPicture(item) ?? item.Flag;
    }
}