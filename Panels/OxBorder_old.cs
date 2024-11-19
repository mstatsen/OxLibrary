using System.Diagnostics.Eventing.Reader;

namespace OxLibrary.Panels
{
    public class OxBorder_old : OxPane
    {
        public OxDock OxDock { get; internal set; }

        public override void ReAlignControls()
        {
            Dock = OxDockHelper.Dock(OxDock);
            SendToBack();
        }

        public bool SetSize(int size)
        {
            if (GetSize().Equals(size))
                return false;

            if (OxDockHelper.IsVertical(OxDock))
                Height = size;
            else
                Width = size;

            return true;
        }

        public int GetSize() =>
            OxDock switch
            {
                OxDock.Left or OxDock.Right => Width,
                OxDock.Top or OxDock.Bottom => Height,
                _ => 0,
            };

        public int CalcedSize => Visible ? GetSize() : 0;

        public void SetSize(OxSize size) =>
            SetSize((int)size);

        public OxBorder_old(Control parentControl, OxDock dock, int size, bool visible)
        {
            Visible = visible;
            Parent = parentControl;
            OxDock = dock;
            ReAlign();
            SetSize(size);
        }

        public OxBorder_old(Control parentControl, OxDock dock, Color backColor, int size = (int)OxSize.XXS, bool visible = true) 
            : this(parentControl, dock, size, visible)
            => BackColor = backColor;

        public OxBorder_old(Control parentControl, OxDock dock, Color backColor, OxSize size, bool visible = true) 
            : this(parentControl, dock, backColor, (int)size, visible)
        { }

        public static OxBorder_old NewLeft(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Left, backColor, size);

        public static OxBorder_old NewLeft(Control parentControl, Color backColor, OxSize size = OxSize.XXS) =>
            NewLeft(parentControl, backColor, (int)size);

        public static OxBorder_old NewRight(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Right, backColor, size);

        public static OxBorder_old NewRight(Control parentControl, Color backColor, OxSize size = OxSize.XXS) =>
            NewRight(parentControl, backColor, (int)size);

        public static OxBorder_old NewTop(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Top, backColor, size);

        public static OxBorder_old NewTop(Control parentControl, Color backColor, OxSize size = OxSize.XXS) =>
            NewTop(parentControl, backColor, (int)size);

        public static OxBorder_old NewBottom(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Bottom, backColor, size);

        public static OxBorder_old NewBottom(Control parentControl, Color backColor, OxSize size = OxSize.XXS) =>
            NewBottom(parentControl, backColor, (int)size);

        public static OxBorder_old New(Control parentControl, DockStyle dock, Color backColor, int size, bool visible = true) =>
            new(parentControl, OxDockHelper.Dock(dock), backColor, size, visible);

        public static OxBorder_old New(Control parentControl, DockStyle dock, Color backColor, OxSize size = OxSize.XXS, bool visible = true) =>
            New(parentControl, dock, backColor, (int)size, visible);

        public static Dictionary<OxDock, OxBorder_old> NewFull(Control parentControl, Color backColor, int size) =>
            new()
                {
                    { OxDock.Left, NewLeft(parentControl, backColor, size) },
                    { OxDock.Top, NewTop(parentControl, backColor, size) },
                    { OxDock.Right, NewRight(parentControl, backColor, size) },
                    { OxDock.Bottom, NewBottom(parentControl, backColor, size) }
                };

        public static Dictionary<OxDock, OxBorder_old> NewFull(
            Control parentControl,
            Color backColor,
            OxSize size = OxSize.XXS) =>
            NewFull(parentControl, backColor, (int)size);
    }
}