namespace OxLibrary.Panels
{
    public class OxFunctionsPanel : OxFrameWithHeader, IOxFrameWithHeader
    {
        public OxFunctionsPanel() : base() { }

        public OxFunctionsPanel(Size contentSize) : base(contentSize) { }

        private void SetTitleAlign() =>
            Header.TitleAlign = ContentAlignment.MiddleCenter;

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetTitleAlign();
        }
    }
}