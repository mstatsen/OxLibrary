namespace OxLibrary.Panels
{
    public class OxClickFrameList<TClickFrame> : List<TClickFrame>
        where TClickFrame : OxClickFrame, new()
    {
        public TClickFrame? Last => 
            Count > 0 
                ? this[Count - 1] 
                : null;

        public int Right => 
            Last is not null 
                ? Last.Right 
            : 0;

        public OxWidth Width()
        {
            int calcedRight = 0;
            int calcedLeft = -1;

            foreach (TClickFrame frame in this)
                if (frame.Visible)
                {
                    if (calcedLeft < 0)
                        calcedLeft = frame.Left;

                    calcedRight = frame.Right;
                }

            return OxWh.Sub(calcedRight, OxWh.Max(calcedLeft, OxWh.W0));
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