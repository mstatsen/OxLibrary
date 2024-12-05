using OxLibrary.Dock;
using OxLibrary.Interfaces;

namespace OxLibrary.ControlsManaging;

public class OxControlList : List<IOxControl>
{
    public new OxControlList FindAll(Predicate<IOxControl> match)
    {
        OxControlList result = new();
        result.AddRange(base.FindAll(match));
        return result;
    }

    public new virtual IOxControl Add(IOxControl control)
    {
        if (!Contains(control))
            base.Add(control);

        return control;
    }

    public new virtual IOxControl Remove(IOxControl control)
    {
        if (Contains(control))
            base.Remove(control);

        return control;
    }

    public OxControlList this[OxDockType dockType]
    {
        get
        {
            OxControlList result =
            FindAll(c =>
                c.Dock is not OxDock.Fill
                && OxDockTypeHelper.ContainsIn(
                        OxDockHelper.DockType(c),
                        dockType
                   )
            );

            if (OxDockTypeHelper.ContainsIn(OxDockType.Docked, dockType))
                result.AddRange(FindAll(c => c.Dock is OxDock.Fill));

            return result;
        }
    }
}
