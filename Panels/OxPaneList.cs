namespace OxLibrary.Panels
{
    public class OxPaneList<TPane> : List<TPane>
        where TPane : IOxPane, new()
    {
        public TPane? Last =>
            Count > 0
                ? this[Count - 1]
                : default;

        public TPane? First =>
            Count > 0
                ? this[0]
                : default;

        public int Bottom
        {
            get
            {
                TPane? last = Last;

                return last is null
                    ? 0
                    : last.Bottom + 24;
            }
        }

        public new OxPaneList<TPane> AddRange(IEnumerable<TPane> collection)
        {
            base.AddRange(collection);
            return this;
        }
    }

    public class OxPaneList : OxPaneList<OxPane>
    { 
    }
}