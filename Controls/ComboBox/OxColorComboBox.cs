namespace OxLibrary.Controls;

public class OxColorComboBox : OxComboBox<string>
{
    public OxColorComboBox() : base()
    {
        Items.Add("Black");
        Items.Add("White");
        Items.Add("DarkGray");
        Items.Add("Silver");
        Items.Add("DarkBlue");
        Items.Add("Blue");
        Items.Add("DeepSkyBlue");
        Items.Add("BlueViolet");
        Items.Add("HotPink");
        Items.Add("SeaGreen");
        Items.Add("Green");
        Items.Add("LightGreen");
        Items.Add("Olive");
        Items.Add("Gold");
        Items.Add("Yellow");
        Items.Add("SaddleBrown");
        Items.Add("FireBrick");
        Items.Add("Red");
        Items.Add("Orange");
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index > -1)
        {
            string? colorName = TItems[e.Index].ToString();

            try
            { 
                if (colorName is not null)
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