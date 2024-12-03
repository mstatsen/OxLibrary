namespace OxLibrary.Handlers
{
    public class OxEventArgs : EventArgs
    {
        public virtual bool Changed => true;
        public new static readonly OxEventArgs Empty = new();
    }
}