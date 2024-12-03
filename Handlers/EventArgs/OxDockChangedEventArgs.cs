using OxLibrary.Controls;

namespace OxLibrary.Handlers
{
    public class OxParentChangedEventArgs : OxEventArgs
    {
        public OxParentChangedEventArgs(IOxControlContainer? oldParent, IOxControlContainer? newParent)
        {
            OldParent = oldParent;
            NewParent = newParent;
        }

        public readonly IOxControlContainer? OldParent;
        public readonly IOxControlContainer? NewParent;

        public override bool Changed =>
            (OldParent is null
                && NewParent is not null)
            || (OldParent is not null
                && !OldParent.Equals(NewParent));

        public static readonly new OxParentChangedEventArgs Empty = new(null, null);
    }
}