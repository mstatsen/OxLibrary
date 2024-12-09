using static System.Windows.Forms.Control;

namespace OxLibrary.Interfaces
{
    public interface IOxBox :
        IOxControlWithManager<IOxBoxManager>,
        IOxBoxManager
    {
        ControlCollection Controls { get; }
    }
}