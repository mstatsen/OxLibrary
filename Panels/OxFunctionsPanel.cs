
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetTitleAlign();
        }
    }
}