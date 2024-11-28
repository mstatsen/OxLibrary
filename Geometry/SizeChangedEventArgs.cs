namespace OxLibrary
{
    public class SizeChangedEventArgs : EventArgs
    {
        public SizeChangedEventArgs(OxWidth oldWidth, OxWidth oldHeight, OxWidth newWidth, OxWidth newHeight)
        {
            OldSize = new(oldWidth, oldHeight);
            NewSize = new(newWidth, newHeight);
        }

        public SizeChangedEventArgs(OxSize oldSize, OxSize newSize) :
            this(oldSize.Width, oldSize.Height, newSize.Width, newSize.Height) { }

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