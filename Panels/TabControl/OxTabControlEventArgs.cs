using OxLibrary.Interfaces;

namespace OxLibrary.Panels
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

    public delegate void OxTabControlEvent(object sender, OxTabControlEventArgs e);
}