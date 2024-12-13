using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxOneBorderPanel : OxPanel
    {
        protected override OxBool GetBorderVisible() =>
            Border.Visible;

        public OxOneBorderPanel(short height) : base(new(1, height)) { }
        public OxOneBorderPanel() : this(1) { }

        public override OxBool HandleParentPadding => OxB.F;

        public OxBorder Border =>
            Borders[OxDockHelper.Opposite(Dock)];

        public override void OnDockChanged(OxDockChangedEventArgs e)
        {
            base.OnDockChanged(e);

            if (!e.IsChanged)
                return;
            
            Borders[OxDockHelper.Opposite(e.OldValue)].SetVisible(false);
            Borders[OxDockHelper.Opposite(e.NewValue)].SetVisible(true);
        }
    }
}