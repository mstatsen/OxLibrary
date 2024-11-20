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

        public int Width()
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

            return calcedRight - Math.Max(calcedLeft, 0);
        }

        public int Height()
        {
            int maxHeight = 0;

            foreach (TClickFrame frame in this)
                maxHeight = Math.Max(maxHeight, frame.Height);

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