using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxLabel : Label, IOxControl<Label>
    {
        private readonly OxControlManager<Label> manager;
        public IOxControlManager Manager => manager;

        public OxLabel()
        {
            manager = OxControlManager.RegisterControl<Label>(this, OnSizeChanged);
            DoubleBuffered = true;
            AutoSize = true;
        }

        public bool ReadOnly
        {
            get => !Enabled;
            set => Enabled = !value;
        }

        public new OxWidth Width
        {
            get => manager.Width;
            set => manager.Width = value;
        }

        public new OxWidth Height
        {
            get => manager.Height;
            set => manager.Height = value;
        }

        public new OxWidth Top
        {
            get => manager.Top;
            set => manager.Top = value;
        }

        public new OxWidth Left
        {
            get => manager.Left;
            set => manager.Left = value;
        }

        public new OxWidth Bottom =>
            manager.Bottom;

        public new OxWidth Right =>
            manager.Right;

        public new OxSize Size
        {
            get => manager.Size;
            set => manager.Size = value;
        }

        public new OxSize ClientSize
        {
            get => manager.ClientSize;
            set => manager.ClientSize = value;
        }

        public new OxPoint Location
        {
            get => manager.Location;
            set => manager.Location = value;
        }

        public new OxSize MinimumSize
        {
            get => manager.MinimumSize;
            set => manager.MinimumSize = value;
        }

        public new OxSize MaximumSize
        {
            get => manager.MaximumSize;
            set => manager.MaximumSize = value;
        }

        public new virtual OxDock Dock
        {
            get => manager.Dock;
            set => manager.Dock = value;
        }

        public new virtual IOxControlContainer? Parent
        {
            get => manager.Parent;
            set => manager.Parent = value;
        }

        public new OxRectangle ClientRectangle =>
            manager.ClientRectangle;

        public new virtual OxRectangle DisplayRectangle =>
            manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => manager.Bounds;
            set => manager.Bounds = value;
        }

        public new OxSize PreferredSize =>
            manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => manager.AutoScrollOffset;
            set => manager.AutoScrollOffset = value;
        }
        public bool SizeChanging =>
            manager.SizeChanging;

        public bool SilentSizeChange(Action method, OxSize oldSize) =>
            manager.SilentSizeChange(method, oldSize);

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            manager.GetChildAtPoint(pt, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            manager.GetChildAtPoint(pt);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            manager.GetPreferredSize(proposedSize);

        public void Invalidate(OxRectangle rc) =>
            manager.Invalidate(rc);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            manager.Invalidate(rc, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            manager.LogicalToDeviceUnits(value);

        public OxPoint PointToClient(OxPoint p) =>
            manager.PointToClient(p);

        public OxPoint PointToScreen(OxPoint p) =>
            manager.PointToScreen(p);

        public OxRectangle RectangleToClient(OxRectangle r) =>
            manager.RectangleToClient(r);

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            manager.RectangleToScreen(r);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            manager.SetBounds(x, y, width, height, specified);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            manager.SetBounds(x, y, width, height);

        public virtual bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!e.Changed)
                return false;

            base.OnSizeChanged(e);
            return true;
        }

        public void RealignControls(OxControlDockType dockType = OxControlDockType.Unknown) =>
            manager.RealignControls(dockType);

        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (SizeChanging)
                return;

            base.OnSizeChanged(e);
        }
    }
}