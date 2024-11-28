namespace OxLibrary.Panels
{
    public class OxFunctionsPanel : OxFrameWithHeader, IOxFrameWithHeader
    {
        public OxFunctionsPanel() : base() { }

        public OxFunctionsPanel(OxSize size) : base(size) { }

        private void SetTitleAlign() =>
            Header.TitleAlign = Height > Width
                ? ContentAlignment.MiddleCenter
                : ContentAlignment.MiddleLeft;

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetTitleAlign();
        }

        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            SetTitleAlign();
            return e.Changed;
        }
    }
}