namespace OxLibrary.Panels
{
    public class OxPaneList : List<IOxPane>
    {
        public IOxPane? Last =>
            Count > 0
                ? this[Count - 1]
                : null;

        public IOxPane? First =>
            Count > 0
                ? this[0]
                : null;

        public int Bottom
        {
            get
            {
                IOxPane? last = Last;

                return last == null
                    ? 0
                    : last.Bottom + 24;
            }
        }

        public new OxPaneList AddRange(IEnumerable<IOxPane> collection)
        {
            base.AddRange(collection);
            return this;
        }
    }
}