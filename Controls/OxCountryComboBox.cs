using OxLibrary.Data.Countries;

namespace OxLibrary.Controls
{
    public class OxCountryComboBox : OxComboBox<Country>
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

        private static readonly int FlagSize = 24;
        private static readonly int FlagSpace = 2;
        private static readonly int FlagLeft = 2;

        protected override Point GetTextStartPosition(Rectangle bounds) => 
            new(bounds.X + FlagLeft + FlagSize + FlagSpace, bounds.Y);

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (DrawStrings && e.Index > -1)
                e.Graphics.DrawImage(
                    OxImageBoxer.BoxingImage(TItems[e.Index].Flag, new Size(FlagSize, FlagSize)),
                    e.Bounds.X + FlagLeft,
                    e.Bounds.Y - 2
                );

        }
    }
}