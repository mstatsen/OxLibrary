namespace OxLibrary.Handlers
{
    public class OxDockChangedEventArgs : OxEventArgs
    {
        public OxDockChangedEventArgs(OxDock oldDock, OxDock newDock)
        {
            OldDock = oldDock;
            NewDock = newDock;
        }

        public readonly OxDock OldDock;
        public readonly OxDock NewDock;

        public override bool Changed =>
            !OldDock.Equals(NewDock);

        public static readonly new OxDockChangedEventArgs Empty = new(OxDock.None, OxDock.None);
    }
}