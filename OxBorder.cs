namespace OxLibrary
{
    public class OxBorder
    {
        public OxBorder() { }

        private OxSize size = OxSize.None;
        private bool visible = true;

        public OxSize Size 
        { 
            get => visible ? size : OxSize.None; 
            set => size = value; 
        }

        public int IntSize
        {
            get => (int)Size;
            set => Size = (OxSize)value;
        }

        public bool Visible
        { 
            get => visible;
            set => visible = value;
        }
    }
}