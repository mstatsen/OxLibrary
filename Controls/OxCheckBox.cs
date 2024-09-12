namespace OxLibrary.Controls
{
    public class OxCheckBox : CheckBox
    {
        public OxCheckBox() => 
            DoubleBuffered = true;

        private bool readOnly = false;

        public bool ReadOnly 
        { 
            get => readOnly; 
            set => readOnly = value; 
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            if (!readOnly)
                base.OnCheckedChanged(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (readOnly)
                Checked = !Checked;

            base.OnClick(e);
        }
    }
}