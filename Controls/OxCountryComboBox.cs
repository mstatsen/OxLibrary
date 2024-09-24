using OxLibrary.Data.Countries;

namespace OxLibrary.Controls
{
    public class OxCountryComboBox : OxComboBox<Country>
    {
        public OxCountryComboBox() 
        {
            HoverItemChanged += OxCountryComboBox_ListItemSelectionChanged;
            /*
            MouseLeave += OxCountryComboBox_MouseLeave;
            SelectedIndexChanged += OxCountryComboBox_SelectedIndexChanged;
            LostFocus += OxCountryComboBox_LostFocus;
            MouseHover += OxCountryComboBox_MouseHover;
            */
            
        }

        /*
        private void OxCountryComboBox_MouseHover(object? sender, EventArgs e)
        {
            ToolTip.Hide(this);
        }

        private void OxCountryComboBox_LostFocus(object? sender, EventArgs e)
        {
            ToolTip.Hide(this);
        }

        private void OxCountryComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ToolTip.Hide(this);
        }

        private void OxCountryComboBox_MouseLeave(object? sender, EventArgs e)
        {
            ToolTip.Hide(this);
        }
        */

        private void OxCountryComboBox_ListItemSelectionChanged(object? sender, HoverItemEventArgs e)
        {
            if (e.HoveredItem != null)
            {
                ToolTip.ToolTipTitle = e.HoveredItem.FullName;
                ToolTip.Show( $"Region: {CountryLocationHelper.Name(e.HoveredItem.Location)}\n" +
                    $"Alpha3: {e.HoveredItem.Alpha3}\n" +
                    $"Alpha2: {e.HoveredItem.Alpha2}\n" +
                    $"ISO: {e.HoveredItem.ISO}\n", this, PointToClient(Cursor.Position));
            }
            else
            {
                ToolTip.Hide(this);
            }
        }

        public Country? SelectedCountry =>
            SelectedItem is Country country ? country : null;

        public void LoadCounties()
        {
            Items.Clear();

            foreach (Country country in CountryList.Countries)
                Items.Add(country);
        }

        private static readonly int FlagSize = 24;
        private static readonly int FlagSpace = 2;
        private static readonly int FlagLeft = 2;

        private readonly ToolTip ToolTip = new()
        {
            AutomaticDelay = 500,
            ShowAlways = true
        };

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