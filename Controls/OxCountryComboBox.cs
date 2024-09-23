using OxLibrary.Data.Countries;

namespace OxLibrary.Controls
{
    public class OxCountryComboBox : OxComboBox
    {
        public OxCountryComboBox() { }

        public void LoadCounties()
        {
            Items.Clear();

            foreach (Country country in CountryList.Countries)
                Items.Add(country);
        }

        private static readonly int FlagSize = 24;
        private static readonly int FlagSpace = 2;
        private static readonly int FlagLeft = 2;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Color BrushColor =
                ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    ? new OxColorHelper(BackColor).Darker(2)
                    : BackColor;

            e.Graphics.DrawRectangle(new Pen(BrushColor), e.Bounds);
            e.Graphics.FillRectangle(new SolidBrush(BrushColor), e.Bounds);

            if (DrawStrings && e.Index > -1)
            {
                Country country = (Country)Items[e.Index];

                e.Graphics.DrawImage(
                    OxImageBoxer.BoxingImage(country.Flag!, new Size(FlagSize, FlagSize)),
                    e.Bounds.X + FlagLeft,
                    e.Bounds.Y - 2
                );

                e.Graphics.DrawString(country.ToString(),
                    e.Font ?? new Font("Calibri Light", 10),
                    new SolidBrush(Color.Black),
                    new Point(e.Bounds.X + FlagLeft + FlagSize + FlagSpace, e.Bounds.Y));
            }
        }
    }
}