using OxLibrary.Controls;

namespace OxLibrary.Handlers
{
    public delegate void OxActionClick<TAction>(
        object sender,
        OxActionEventArgs<TAction> EventArgs)
        where TAction : notnull, Enum;
    public delegate void OxBordersChangedEvent(object sender, OxBordersChangedEventArgs e);
    public delegate void OxControlEvent(IOxBox sender, OxControlEventArgs EventArgs);
    public delegate void OxDockChangedEvent(object sender, OxDockChangedEventArgs args);
    public delegate void OxLocationChangedEvent(object sender, OxLocationChangedEventArgs args);
    public delegate void OxParentChangedEvent(object sender, OxParentChangedEventArgs args);
    public delegate void OxSizeChangedEvent(object sender, OxSizeChangedEventArgs args);

    public class OxHandlers : Dictionary<OxHandlerType, List<Delegate>>
    {

        public OxHandlers() { }

        public void Add(OxHandlerType type, Delegate handler)
        {

            if (!TryGetValue(type, out List<Delegate>? list))
            {
                list = new List<Delegate>();
                Add(type, list);
            }

            if (list.Find(d => d.Equals(handler)) is null)
                list.Add(handler);
        }

        public void Remove(OxHandlerType type, Delegate handler)
        {
            if (!TryGetValue(type, out List<Delegate>? list))
                return;

            list.Remove(handler);
        }

        public void Invoke(OxHandlerType type, object sender, OxEventArgs args)
        {
            if (!TryGetValue(type, out List<Delegate>? list))
                return;

            foreach (Delegate handler in list)
            {
                if (args is OxChangingEventArgs changingEventArgs
                    && changingEventArgs.Cancel)
                    return;

                handler.DynamicInvoke(sender, args);
            }
        }
    }
}