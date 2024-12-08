using OxLibrary.Controls;
using OxLibrary.Handlers;

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

            if (SquareButtons)
                button.Width = button.Height;

            AddButton(button, startGroup, true);
            return button;
        }

        private bool squareButtons = true;
        public bool SquareButtons 
        {
            get => squareButtons;
            set
            {
                squareButtons = value;
                PlaceButtons();
            }
        }

        protected override void PrepareButtonsSizes()
        {
            if (!SquareButtons)
                return;

            foreach (OxIconButton button in Buttons)
            {
                button.Height = Height;
                button.Width = Height;
            }
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
                PlaceButtons();
        }
    }
}