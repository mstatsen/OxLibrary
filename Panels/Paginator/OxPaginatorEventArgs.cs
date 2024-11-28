namespace OxLibrary.Panels
{
    public class OxPaginatorEventArgs
    {
        public int CurrentPageIndex { get; set; }
        public int StartObjectIndex { get; set; }
        public int EndObjectIndex { get; set; }

        public OxPaginatorEventArgs(int currentPageIndex, int startObjectIndex, int endObjectIndex)
        {
            CurrentPageIndex = currentPageIndex;
            StartObjectIndex = startObjectIndex;
            EndObjectIndex = endObjectIndex;
        }
    }

    public delegate void OxPaginatorEventHandler(object sender, OxPaginatorEventArgs e);
}