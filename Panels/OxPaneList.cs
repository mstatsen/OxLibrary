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

        public OxWidth Bottom
        {
            get
            {
                TPane? last = Last;

                return last is null
                    ? OxWh.W0
                    : last.Bottom | OxWh.W24;
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