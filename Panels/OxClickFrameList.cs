namespace OxLibrary.Panels
{
    public class OxClickFrameList<TClickFrame> : List<TClickFrame>
        where TClickFrame : OxClickFrame, new()
    {
        public TClickFrame? First =>
            Count > 0
                ? this[0]
                : null;

        public TClickFrame? Last =>
            Count > 0
                ? this[Count - 1]
                : null;

        public TClickFrame? FirstVisible =>
            Find(f => f.Visible);

        public TClickFrame? LastVisible
        {
            get
            {
                TClickFrame? visibleFrame = null;

                foreach (TClickFrame frame in this)
                    if (frame.Visible)
                        visibleFrame = frame;

                return visibleFrame;
            }
        }

        public OxWidth Right => 
            Last is not null 
                ? Last.Right 
                : OxWh.W0;

        public OxWidth Width()
        {
            TClickFrame? firstFisibleFrame = FirstVisible;
            TClickFrame? lastVisibleFrame = LastVisible;

            return OxWh.Sub(
                lastVisibleFrame is null
                    ? OxWh.W0
                    : lastVisibleFrame.Right,
                OxWh.Max(
                    firstFisibleFrame is null 
                        ? OxWh.W0 
                        : firstFisibleFrame.Right, 
                    OxWh.W0
                )
            ); 
        }

        public OxWidth Height()
        {
            OxWidth maxHeight = OxWh.W0;

            foreach (TClickFrame frame in this)
                maxHeight = OxWh.Max(maxHeight, frame.HeightInt);

            return maxHeight;
        }

        private TClickFrame? Default()
        {
            foreach (TClickFrame button in this)
                if (button.Visible && button.Enabled && button.Default)
                    return button;

            return null;
        }

        public bool ExecuteDefault()
        {
            TClickFrame? defultButton = Default();
            defultButton?.PerformClick();
            return defultButton is not null;
        }
    }

    public class OxClickFrameList : OxClickFrameList<OxClickFrame>
    { 
    }
}