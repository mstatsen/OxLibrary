namespace OxLibrary.Controls
{
    public class OxTaggedButton : OxButton
    {
        public OxTaggedButton(int tag) : base(tag.ToString(), null) { }

        private int tag = 0;
        public new int Tag 
        { 
            get => tag; 
            set => SetTag(value); 
        }

        private void SetTag(int value)
        {
            tag = value;
            Text = tag.ToString();
        }
    }
}
