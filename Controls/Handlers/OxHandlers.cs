namespace OxLibrary.Controls.Handlers
{
    public enum OxHandlerType
    { 
        SizeChanged
    }

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

        public void Invoke(OxHandlerType type, object sender, EventArgs args)
        {
            if (!TryGetValue(type, out List<Delegate>? list))
                return;

            foreach (Delegate handler in list)
                handler.DynamicInvoke(sender, args);
        }
    }
}
