namespace OxLibrary.Panels
{
    public class OxPaneList: List<OxPane>
    {
        public OxPane? Last =>
            Count > 0
                ? this[Count - 1]
                : default;

        public OxPane? First =>
            Count > 0
                ? this[0]
                : default;

        public OxWidth Bottom
        {
            get
            {
                OxPane? last = Last;

                return last is null
                    ? OxWh.W0
                    : last.Bottom | OxWh.W24;
            }
        }

        public new OxPaneList AddRange(IEnumerable<OxPane> collection)
        {
            base.AddRange(collection);
            return this;
        }
    }
}