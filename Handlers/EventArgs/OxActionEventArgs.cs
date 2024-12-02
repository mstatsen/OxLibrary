namespace OxLibrary.Handlers
{
    public class OxActionEventArgs<TAction> : OxEventArgs
        where TAction : notnull, Enum
    {
        public TAction Action { get; internal set; }
        public OxActionEventArgs(TAction action) : base() =>
            Action = action;
    }
}