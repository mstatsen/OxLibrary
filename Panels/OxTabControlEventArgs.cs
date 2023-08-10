namespace OxLibrary.Panels
{
    public class OxTabControlEventArgs
    {
        public IOxPane? OldPage { get; private set; }
        public IOxPane? Page { get; private set; }
        public OxTabControlEventArgs(IOxPane? oldPage, IOxPane? page)
        {
            OldPage = oldPage;
            Page = page;
        }
    }

    public delegate void OxTabControlEvent(object sender, OxTabControlEventArgs e);
}