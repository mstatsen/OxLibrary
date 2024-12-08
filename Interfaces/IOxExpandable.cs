using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    public interface IOxExpandable
    {
        bool Expanded { get; set; }
        void Expand();
        void Collapse();
        event ExpandChanged ExpandChanged;
    }
}