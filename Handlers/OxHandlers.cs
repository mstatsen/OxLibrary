using OxLibrary.Interfaces;

namespace OxLibrary.Handlers
{
    public delegate void GetToolTip<T>(T item, out string toolTipTitle, out string toolTipText);
    public delegate void OxActionClick<TAction>(
        object sender,
        OxActionEventArgs<TAction> e)
        where TAction : notnull, Enum;
    public delegate void OxBordersChangedEvent(object sender, OxBordersChangedEventArgs e);
    public delegate void OxControlEvent(IOxBox sender, OxControlEventArgs e);
    public delegate void OxDockChangedEvent(object sender, OxDockChangedEventArgs e);
    public delegate void ExpandChanged(IOxExpandable sender, OxBoolEventArgs e);
    public delegate void OxLocationChangedEvent(object sender, OxLocationChangedEventArgs e);
    public delegate void OxParentChangedEvent(object sender, OxParentChangedEventArgs e);
    public delegate void OxSizeChangedEvent(object sender, OxSizeChangedEventArgs e);

    public class OxHandlers : Dictionary<OxHandlerType, List<Delegate>>
    {
        public IOxControl Owner { get; }
        public OxHandlers(IOxControl owner) =>
            Owner = owner;

        public void Add(OxHandlerType type, Delegate handler)
        {
            if (OxHandlerTypeHelper.UseDependedFromBox(type)
                && Owner is IOxDependedBox dependedBox)
            { 
                dependedBox.DependedFrom.AddHandler(type, handler);
                return;
            }

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
            if (OxHandlerTypeHelper.UseDependedFromBox(type)
                && Owner is IOxDependedBox dependedBox)
            {
                dependedBox.DependedFrom.RemoveHandler(type, handler);
                return;
            }

            if (!TryGetValue(type, out List<Delegate>? list))
                return;

            list.Remove(handler);
        }

        public void Invoke(OxHandlerType type, object sender, OxEventArgs args)
        {
            if (OxHandlerTypeHelper.UseDependedFromBox(type)
                && Owner is IOxDependedBox dependedBox)
            {
                dependedBox.DependedFrom.InvokeHandlers(type, args);
                return;
            }

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