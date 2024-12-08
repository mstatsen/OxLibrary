namespace OxLibrary.Interfaces
{
    public interface IOxCard : IOxFrameWithHeader
    {
        bool Accordion { get; set; }
        bool Expanded { get; set; }

        void Expand();
        void Collapse();

        event EventHandler ExpandHandler;

        bool ExpandButtonVisible { get; set; }
    }
}