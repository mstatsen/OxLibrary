namespace OxLibrary.Handlers
{
    public class OxEventArgs : EventArgs
    {
        public virtual bool Changed => true;
        public new static readonly OxEventArgs Empty = new();
    }

    public class OxEventArgs<TValue> : OxEventArgs
    {
        public TValue? OldValue { get; }
        public TValue? NewValue { get; }

        public OxEventArgs(TValue? oldValue, TValue? newValue)
        { 
            OldValue = oldValue;
            NewValue = newValue;
        }

        public override bool Changed =>
            (OldValue is null
                && NewValue is not null)
            || (OldValue is not null
                && !OldValue.Equals(NewValue));

        public static readonly new OxEventArgs<TValue> Empty = new(default, default);
    }

    public class OxNotNullEventArgs<TValue> : OxEventArgs<TValue>
        where TValue : notnull
    {
        public OxNotNullEventArgs(TValue oldValue, TValue newValue) : base(oldValue, newValue)
        { }

        public new TValue OldValue => base.OldValue!;
        public new TValue NewValue => base.NewValue!;
    }
}