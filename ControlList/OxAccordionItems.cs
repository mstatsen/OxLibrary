using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.ControlList;

public class OxAccordionItems : List<IOxExpandable>
{
    public new void Add(IOxExpandable item)
    {
        item.ExpandChanged += ItemExpandChanedHandler;
        base.Add(item);
    }

    public new void Remove(IOxExpandable item)
    {
        item.ExpandChanged -= ItemExpandChanedHandler;
        base.Remove(item);
    }

    private void ItemExpandChanedHandler(IOxExpandable sender, OxBoolEventArgs e)
    {
        if (!e.Changed)
            return;

        if (e.NewValue)
            foreach (IOxExpandable item in this)
                if (item.Expanded
                    && !item.Equals(sender))
                    item.Collapse();

        if (sender is IOxWithColorHelper withColorHelper)
            withColorHelper.BaseColor = e.NewValue
                ? withColorHelper.Colors.HBluer(-2).Browner(1)
                : withColorHelper.Colors.HBluer(2).Browner(-1);
    }
}