using OxLibrary.Handlers;

namespace OxLibrary.Controls
{
    public class OxPictureBox : PictureBox, IOxControl<PictureBox>
    {
        private readonly OxControlManager<PictureBox> manager;
        public IOxControlManager<PictureBox> Manager => manager;

        public OxPictureBox()
        {
            manager = OxControlManager.RegisterControl<PictureBox>(this);
            DoubleBuffered = true;
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

        public new virtual IOxContainer? Parent
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
        public void DoWithSuspendedLayout(Action method) =>
            manager.DoWithSuspendedLayout(method);

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

        public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
        public new event OxDockChangedEvent DockChanged
        {
            add => manager.DockChanged += value;
            remove => manager.DockChanged -= value;
        }

        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
        public new event OxLocationChangedEvent LocationChanged
        {
            add => manager.LocationChanged += value;
            remove => manager.LocationChanged -= value;
        }

        public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
        public new event OxParentChangedEvent ParentChanged
        {
            add => manager.ParentChanged += value;
            remove => manager.ParentChanged -= value;
        }

        public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
        public new event OxSizeChangedEvent SizeChanged
        {
            add => manager.SizeChanged += value;
            remove => manager.SizeChanged -= value;
        }

        #region Hidden base methods
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
        #endregion
    }
}