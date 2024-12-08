namespace OxLibrary.Handlers
{
    public class OxNotNullEventArgs<TValue> : OxEventArgs<TValue>
        where TValue : notnull
    {
        public OxNotNullEventArgs(TValue oldValue, TValue newValue) : base(oldValue, newValue)
        { }

        public new TValue OldValue => base.OldValue!;
        public new TValue NewValue => base.NewValue!;
    }
}