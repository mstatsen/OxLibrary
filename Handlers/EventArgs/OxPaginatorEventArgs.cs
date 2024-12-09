namespace OxLibrary.Handlers
{
    public class OxPaginatorEventArgs
    {
        public short CurrentPageIndex { get; set; }
        public int StartObjectIndex { get; set; }
        public int EndObjectIndex { get; set; }

        public OxPaginatorEventArgs(short currentPageIndex, int startObjectIndex, int endObjectIndex)
        {
            CurrentPageIndex = currentPageIndex;
            StartObjectIndex = startObjectIndex;
            EndObjectIndex = endObjectIndex;
        }
    }
}