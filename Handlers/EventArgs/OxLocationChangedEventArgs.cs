namespace OxLibrary.Handlers;

public class OxLocationChangedEventArgs : OxNotNullEventArgs<OxPoint>
{
    public OxLocationChangedEventArgs(OxPoint oldLocation, OxPoint newLocation) :
        base(new(oldLocation), new(newLocation))
    { }

    public OxBool XChanged =>
        OxB.B(IsXChanged);

    public bool IsXChanged =>
        !OldValue.X.Equals(NewValue!.X);

    public OxBool YChanged =>
        OxB.B(IsYChanged);

    public bool IsYChanged =>
        !OldValue.Y.Equals(NewValue!.Y);
}