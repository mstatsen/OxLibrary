using OxLibrary.Handlers;

namespace OxLibrary.Interfaces
{
    public interface IOxExpandable
    {
        OxBool Expanded { get; set; }
        bool IsExpanded => OxB.B(Expanded);
        void SetExpanded(bool value) => Expanded = OxB.B(value);
        void Expand();
        void Collapse();

        event ExpandChanged ExpandChanged;
    }
}