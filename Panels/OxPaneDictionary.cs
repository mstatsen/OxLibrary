namespace OxLibrary.Panels
{
    public class OxPaneDictionary : Dictionary<OxPane, OxPaneList>
    {
        public void Add(OxPane pane) =>
            Add(pane, new OxPaneList());
    }
}