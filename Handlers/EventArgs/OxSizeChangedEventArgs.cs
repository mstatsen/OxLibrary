namespace OxLibrary.Handlers;

public class OxSizeChangedEventArgs : OxNotNullEventArgs<OxSize>
{
    public OxSizeChangedEventArgs(OxSize oldSize, OxSize newSize) :
        base(new(oldSize), new(newSize))
    { }

    public bool WidthChanged =>
        !OldValue!.Width.Equals(NewValue!.Width);

    public bool HeightChanged =>
        !OldValue!.Height.Equals(NewValue!.Height);

    public static readonly new OxSizeChangedEventArgs Empty = new(OxSize.Empty, OxSize.Empty);
}
