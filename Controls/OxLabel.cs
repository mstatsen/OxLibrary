namespace OxLibrary.Controls
{
    public class OxLabel : Label
    {
        public OxLabel()
        {
            DoubleBuffered = true;
            AutoSize = true;
        }

        public bool ReadOnly
        {
            get => !Enabled;
            set => Enabled = !value;
        }
    }
}