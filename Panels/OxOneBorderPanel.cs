using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxOneBorderPanel : OxPanel
    {
        protected override bool GetBorderVisible() =>
            Border.Visible;

        public OxOneBorderPanel(short height) : base(new(1, height)) { }
        public OxOneBorderPanel() : this(1) { }

        public override bool HandleParentPadding => false;

        public OxBorder Border =>
            Borders[OxDockHelper.Opposite(Dock)];

        public override void OnDockChanged(OxDockChangedEventArgs e)
        {
            base.OnDockChanged(e);

            if (e.Changed)
            {
                Borders[OxDockHelper.Opposite(e.OldValue)].Visible = false;
                Borders[OxDockHelper.Opposite(e.NewValue)].Visible = true;
            }
        }
    }
}