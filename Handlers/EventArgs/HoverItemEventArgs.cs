namespace OxLibrary.Handlers;

public partial class HoverItemEventArgs<T> : EventArgs
{
    public HoverItemEventArgs(int idx, T? hoveredItem)
    {
        HoveredItemIndex = idx;
        HoveredItem = hoveredItem;
    }
    public int HoveredItemIndex { get; }
    public T? HoveredItem { get; }
}