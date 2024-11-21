using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public interface IOxControlManager
    {
        OxWidth Width { get; set; }
        OxWidth Height { get; set; }
        OxWidth Top { get; set; }
        OxWidth Left { get; set; }
        OxWidth Bottom { get; }
        OxWidth Right { get; }
        OxSize Size { get; set; }
        OxSize MinimumSize { get; set; }
        OxSize MaximumSize { get; set; }
        OxDock Dock { get; set; }
        
        //OxPane? Parent { get; set; }
        //Control? Parent { get; set; }
        bool OnSizeChanged(SizeChangedEventArgs e);
    }
}
