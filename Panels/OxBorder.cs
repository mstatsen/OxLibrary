using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OxLibrary.Panels
{
    public class OxBorder : OxPane
    {
        public OxDock OxDock { get; internal set; }

        public override void ReAlignControls()
        {
            switch (OxDock)
            {
                case OxDock.Left:
                    Dock = DockStyle.Left;
                    break;
                case OxDock.Right:
                    Dock = DockStyle.Right;
                    break;
                case OxDock.Top:
                    Dock = DockStyle.Top;
                    break;
                case OxDock.Bottom:
                    Dock = DockStyle.Bottom;
                    break;
            }

            SendToBack();
        }

        public bool SetSize(int size)
        {
            if (GetSize() == size)
                return false;

            switch (OxDock)
            {
                case OxDock.Left:
                    Width = size;
                    break;
                case OxDock.Right:
                    Width = size;
                    break;
                case OxDock.Top:
                    Height = size;
                    break;
                case OxDock.Bottom:
                    Height = size;
                    break;
            }

            return true;
        }

        public int GetSize() =>
            OxDock switch
            {
                OxDock.Left => Width,
                OxDock.Right => Width,
                OxDock.Top => Height,
                OxDock.Bottom => Height,
                _ => 0,
            };

        public int CalcedSize => Visible ? GetSize() : 0;

        public void SetSize(OxSize size) =>
            SetSize((int)size);

        public OxBorder(Control parentControl, OxDock dock, int size)
        {
            Parent = parentControl;
            OxDock = dock;
            ReAlign();
            SetSize(size);
        }

        public OxBorder(
            Control parentControl,
            OxDock dock,
            Color backColor,
            int size = (int)OxSize.Small) : this(parentControl, dock, size)
            => BackColor = backColor;

        public OxBorder(
            Control parentControl,
            OxDock dock,
            Color backColor,
            OxSize size) : this(parentControl, dock, backColor, (int)size)
        { }

        public static OxBorder NewLeft(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Left, backColor, size);

        public static OxBorder NewLeft(Control parentControl, Color backColor, OxSize size = OxSize.Small) =>
            NewLeft(parentControl, backColor, (int)size);

        public static OxBorder NewRight(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Right, backColor, size);

        public static OxBorder NewRight(Control parentControl, Color backColor, OxSize size = OxSize.Small) =>
            NewRight(parentControl, backColor, (int)size);

        public static OxBorder NewTop(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Top, backColor, size);

        public static OxBorder NewTop(Control parentControl, Color backColor, OxSize size = OxSize.Small) =>
            NewTop(parentControl, backColor, (int)size);

        public static OxBorder NewBottom(Control parentControl, Color backColor, int size) =>
            new(parentControl, OxDock.Bottom, backColor, size);

        public static OxBorder NewBottom(Control parentControl, Color backColor, OxSize size = OxSize.Small) =>
            NewBottom(parentControl, backColor, (int)size);

        public static Dictionary<OxDock, OxBorder> NewFull(Control parentControl, Color backColor, int size) =>
            new()
                {
                    { OxDock.Left, NewLeft(parentControl, backColor, size) },
                    { OxDock.Top, NewTop(parentControl, backColor, size) },
                    { OxDock.Right, NewRight(parentControl, backColor, size) },
                    { OxDock.Bottom, NewBottom(parentControl, backColor, size) }
                };

        public static Dictionary<OxDock, OxBorder> NewFull(
            Control parentControl,
            Color backColor,
            OxSize size = OxSize.Small) =>
            NewFull(parentControl, backColor, (int)size);
    }
}