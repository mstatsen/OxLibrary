namespace OxLibrary.Handlers
{
    public class OxSizeChangedEventArgs : OxEventArgs
    {
        public OxSizeChangedEventArgs(OxWidth oldWidth, OxWidth oldHeight, OxWidth newWidth, OxWidth newHeight)
        {
            OldSize = new(oldWidth, oldHeight);
            NewSize = new(newWidth, newHeight);
        }

        public OxSizeChangedEventArgs(OxSize oldSize, OxSize newSize) :
            this(oldSize.Width, oldSize.Height, newSize.Width, newSize.Height)
        { }

        public readonly OxSize OldSize;
        public readonly OxSize NewSize;

        public bool Changed =>
            !OldSize.Equals(NewSize);

        public bool WidthChanged =>
            !OldSize.Width.Equals(NewSize.Width);

        public bool HeightChanged =>
            !OldSize.Height.Equals(NewSize.Height);
    }
}
