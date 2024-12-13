namespace OxLibrary.Handlers
{
    public class OxBoolChangedEventArgs : OxEventArgs<OxBool>
    {
        public OxBoolChangedEventArgs(OxBool oldValue, OxBool newValue) : base(oldValue, newValue)
        { }

        public OxBoolChangedEventArgs(bool oldValue, bool newValue) : this(OxB.B(oldValue), OxB.B(newValue))
        { }

        public OxBoolChangedEventArgs(OxBool newValue) : this(OxB.Not(newValue), newValue)
        { }

        public OxBoolChangedEventArgs(bool newValue) : this(OxB.B(!newValue), OxB.B(newValue))
        { }

        public bool IsOldValue => OxB.B(OldValue);
        public bool IsNewValue => OxB.B(NewValue);
    }
}