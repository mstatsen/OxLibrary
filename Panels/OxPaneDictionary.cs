namespace OxLibrary.Panels
{
    public class OxPaneDictionary : Dictionary<IOxPane, OxPaneList>
    {
        public void Add(IOxPane pane) =>
            Add(pane, new OxPaneList());
    }
}