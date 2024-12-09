using OxLibrary.Interfaces;

namespace OxLibrary.Handlers
{
    public class OxTabControlEventArgs
    {
        public IOxPanel? OldPage { get; private set; }
        public IOxPanel? Page { get; private set; }
        public OxTabControlEventArgs(IOxPanel? oldPage, IOxPanel? page)
        {
            OldPage = oldPage;
            Page = page;
        }
    }
}