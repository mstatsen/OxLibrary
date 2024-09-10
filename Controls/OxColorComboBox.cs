namespace OxLibrary.Controls
{
    public class OxColorComboBox : OxComboBox
    {
        public OxColorComboBox() : base()
        {
            Items.Add("Black");
            Items.Add("White");
            Items.Add("DarkGray");
            Items.Add("Silver");
            Items.Add("DarkBlue");
            Items.Add("Blue");
            Items.Add("LightBlue");
            Items.Add("Violet");
            Items.Add("Pink");
            Items.Add("DarkGreen");
            Items.Add("Green");
            Items.Add("LightGreen");
            Items.Add("Yellow");
            Items.Add("Brown");
            Items.Add("DarkRed");
            Items.Add("Red");
            Items.Add("Orange");
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                string? colorName = Items[e.Index].ToString();

                try
                { 
                    if (colorName != null)
                        BackColor = Color.FromName(colorName);
                }
                catch 
                {
                    BackColor = Color.Black;
                }
            }

            base.OnDrawItem(e);
        }
    }
}