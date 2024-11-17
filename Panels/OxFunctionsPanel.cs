namespace OxLibrary.Panels
{
    public class OxFunctionsPanel : OxFrameWithHeader, IOxFrameWithHeader
    {
        public OxFunctionsPanel() : base() { }

        public OxFunctionsPanel(Size contentSize) : base(contentSize) { }

        protected virtual void SetTitleAlign() =>
            Header.TitleAlign = ContentAlignment.MiddleCenter;

        protected override void AfterCreated()
        {
            base.AfterCreated();
            SetTitleAlign();
        }
    }
}