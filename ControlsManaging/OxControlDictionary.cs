using OxLibrary.Dock;
using OxLibrary.Interfaces;

namespace OxLibrary.ControlsManaging;

public class OxControlDictionary<T> : Dictionary<IOxControl, T>
{
    public new virtual KeyValuePair<IOxControl, T> Add(IOxControl control, T value)
    {
        if (!ContainsKey(control))
            base.Add(control, value);

        return new(control, value);
    }
}