using OxLibrary.Interfaces;

namespace OxLibrary;

public static class OxControlManagers
{
    private class OxControlManagerCache : Dictionary<Control, IOxControlManager>
    {
        public TManager AddManager<TManager>(Control control, TManager manager)
            where TManager : OxControlManager
        {
            if (!ContainsKey(control))
                Add(
                    control, manager
                );

            return (TManager)this[control];
        }
    }

    private static readonly OxControlManagerCache Controls = new();

    public static IOxControlManager RegisterControl(Control baseControl) =>
        Controls.AddManager(
            baseControl,
            new OxControlManager(baseControl)
        );

    public static IOxBoxManager RegisterBox(Control baseBox) =>
        Controls.AddManager(
            baseBox,
                new OxBoxManager(baseBox)
            );

    public static void UnRegisterControl(Control control) =>
        Controls.Remove(control);
}