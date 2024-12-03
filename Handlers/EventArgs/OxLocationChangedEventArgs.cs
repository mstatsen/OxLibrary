namespace OxLibrary.Handlers
{
    public class OxLocationChangedEventArgs : OxEventArgs
    {
        public OxLocationChangedEventArgs(OxWidth oldX, OxWidth oldY, OxWidth newX, OxWidth newY)
        {
            OldLocation = new(oldX, oldY);
            NewLocation = new(newX, newY);
        }

        public OxLocationChangedEventArgs(OxPoint oldLocation, OxPoint newLocation) :
            this(oldLocation.X, oldLocation.Y, newLocation.X, newLocation.Y)
        { }

        public readonly OxPoint OldLocation;
        public readonly OxPoint NewLocation;

        public override bool Changed =>
            !OldLocation.Equals(NewLocation);

        public bool XChanged =>
            !OldLocation.X.Equals(NewLocation.X);

        public bool YChanged =>
            !OldLocation.Y.Equals(NewLocation.Y);
    }
}
