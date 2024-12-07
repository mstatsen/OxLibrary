namespace OxLibrary.Interfaces
{
    public interface IOxDependedBox : IOxBox
    {
        public IOxBox DependedFrom { get; }
    }
}