namespace OxLibrary.Panels
{
    public class OxClickFrameList : List<OxClickFrame>
    {
        public OxClickFrame? Last => Count > 0 ? this[Count - 1] : null;

        public int Right => Last != null ? Last.Right : 0;

        public int Width()
        {
            int calcedRight = 0;
            int calcedLeft = -1;

            foreach (OxClickFrame frame in this)
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

            foreach (OxClickFrame frame in this)
                maxHeight = Math.Max(maxHeight, frame.Height);

            return maxHeight;
        }

        private OxClickFrame? Default()
        {
            foreach (OxClickFrame button in this)
                if (button.Visible && button.Enabled && button.Default)
                    return button;

            return null;
        }

        public bool ExecuteDefault()
        {
            OxClickFrame? defultButton = Default();
            defultButton?.PerformClick();
            return defultButton != null;
        }
    }

}