using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxHeaderToolBar : OxToolBar<OxIconButton>
    {
        protected override Color GetBorderColor() =>
            Color.Transparent;

        protected override void SetToolBarPaddings() =>
            Padding.Size = OxWh.W0;


        private new OxIconButton AddButton(OxIconButton button, bool? startGroup = null, bool InsertAsFirst = false) =>
            base.AddButton(button, startGroup, InsertAsFirst);

        public OxIconButton AddButton(OxIconButton button, bool? startGroup = null)
        {
            button.BorderVisible = false;
            button.BlurredBorder = false;
            button.Dock = OxDock.Right;
            AddButton(button, startGroup, true);
            return button;
        }
    }
}