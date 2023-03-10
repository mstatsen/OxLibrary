using System.Windows.Forms;

namespace OxLibrary.Controls
{
    public class OxTextBox : TextBox
    {
        public OxTextBox() : base()
        {
            DoubleBuffered = true;
            AutoSize = false;
            Height = 28;
        }
    }
}
