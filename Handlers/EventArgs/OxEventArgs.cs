namespace OxLibrary.Handlers;

public class OxEventArgs : EventArgs
{
    public OxBool Changed => OxB.B(IsChanged);
    public virtual bool IsChanged => true;
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

    public override bool IsChanged =>
        OxHelper.Changed(OldValue, NewValue);

    public static readonly new OxEventArgs<TValue> Empty = new(default, default);
}