namespace OxLibrary.Handlers
{
    public class OxDockChangedEventArgs : OxNotNullEventArgs<OxDock>
    {
        public OxDockChangedEventArgs(OxDock oldDock, OxDock newDock) :
            base(oldDock, newDock) { }

        public static readonly new OxDockChangedEventArgs Empty = new(OxDock.None, OxDock.None);
    }
}