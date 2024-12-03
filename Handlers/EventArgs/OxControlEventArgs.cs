using OxLibrary.Controls;

namespace OxLibrary.Handlers
{
    public class OxControlEventArgs : OxEventArgs
    {
        public readonly IOxControl Control;

        public OxControlEventArgs(IOxControl control) =>
            Control = control;
    }
}
