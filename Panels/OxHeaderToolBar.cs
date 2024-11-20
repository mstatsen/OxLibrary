using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxHeaderToolBar : OxToolBar<OxIconButton>
    {
        public override Color GetBordersColor() =>
            Color.Transparent;

        protected override void SetToolBarPaddings() =>
            Padding.Size = OxWh.W1;
    }
}