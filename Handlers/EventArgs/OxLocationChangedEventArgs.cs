namespace OxLibrary.Handlers
{
    public class OxLocationChangedEventArgs : OxNotNullEventArgs<OxPoint>
    {
        public OxLocationChangedEventArgs(OxPoint oldLocation, OxPoint newLocation) :
            base(new(oldLocation), new(newLocation))
        { }

        public bool XChanged =>
            !OldValue.X.Equals(NewValue!.X);

        public bool YChanged =>
            !OldValue.Y.Equals(NewValue!.Y);
    }
}