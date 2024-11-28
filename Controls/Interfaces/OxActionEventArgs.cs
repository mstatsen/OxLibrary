namespace OxLibrary.Controls
{
    public class OxActionEventArgs<TAction>
        where TAction : notnull, Enum
    {
        public TAction Action { get; internal set; }
        public OxActionEventArgs(TAction action) : base() =>
            Action = action;
    }

    public delegate void OxActionClick<TAction>(object? sender, 
        OxActionEventArgs<TAction> EventArgs)
        where TAction : notnull, Enum;
}