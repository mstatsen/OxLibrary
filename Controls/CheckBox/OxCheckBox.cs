using OxLibrary.Handlers;

namespace OxLibrary.Controls
{
    public class OxCheckBox :
        CheckBox, 
        IOxControl,
        IOxManagingControl<OxControlManager<OxCheckBox>>,
        IOxManagingControl<IOxControlManager>
    {
        public OxControlManager<OxCheckBox> Manager { get; }

        public OxCheckBox()
        {
            Manager = OxControlManagers.RegisterControl<OxCheckBox>(this);
            DoubleBuffered = true;
        }

        public new OxWidth Width
        {
            get => Manager.Width;
            set => Manager.Width = value;
        }

        public new OxWidth Height
        {
            get => Manager.Height;
            set => Manager.Height = value;
        }

        public new OxWidth Top
        {
            get => Manager.Top;
            set => Manager.Top = value;
        }

        public new OxWidth Left
        {
            get => Manager.Left;
            set => Manager.Left = value;
        }

        public new OxWidth Bottom =>
            Manager.Bottom;

        public new OxWidth Right =>
            Manager.Right;

        public new OxSize Size
        {
            get => Manager.Size;
            set => Manager.Size = value;
        }

        public new OxSize ClientSize
        {
            get => Manager.ClientSize;
            set => Manager.ClientSize = value;
        }

        public new OxPoint Location
        {
            get => Manager.Location;
            set => Manager.Location = value;
        }

        public new OxSize MinimumSize
        {
            get => Manager.MinimumSize;
            set => Manager.MinimumSize = value;
        }

        public new OxSize MaximumSize
        {
            get => Manager.MaximumSize;
            set => Manager.MaximumSize = value;
        }

        public new virtual OxDock Dock
        {
            get => Manager.Dock;
            set => Manager.Dock = value;
        }

        public new virtual IOxBox? Parent
        {
            get => Manager.Parent;
            set => Manager.Parent = value;
        }

        public new OxRectangle ClientRectangle =>
            Manager.ClientRectangle;

        public new virtual OxRectangle DisplayRectangle =>
            Manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => Manager.Bounds;
            set => Manager.Bounds = value;
        }

        public new OxSize PreferredSize =>
            Manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => Manager.AutoScrollOffset;
            set => Manager.AutoScrollOffset = value;
        }
        public void DoWithSuspendedLayout(Action method) =>
            Manager.DoWithSuspendedLayout(method);

        public Control GetChildAtPoint(OxPoint pt, GetChildAtPointSkip skipValue) =>
            Manager.GetChildAtPoint(pt, skipValue);

        public Control GetChildAtPoint(OxPoint pt) =>
            Manager.GetChildAtPoint(pt);

        public OxSize GetPreferredSize(OxSize proposedSize) =>
            Manager.GetPreferredSize(proposedSize);

        public void Invalidate(OxRectangle rc) =>
            Manager.Invalidate(rc);

        public void Invalidate(OxRectangle rc, bool invalidateChildren) =>
            Manager.Invalidate(rc, invalidateChildren);

        public OxSize LogicalToDeviceUnits(OxSize value) =>
            Manager.LogicalToDeviceUnits(value);

        public OxPoint PointToClient(OxPoint p) =>
            Manager.PointToClient(p);

        public OxPoint PointToScreen(OxPoint p) =>
            Manager.PointToScreen(p);

        public OxRectangle RectangleToClient(OxRectangle r) =>
            Manager.RectangleToClient(r);

        public OxRectangle RectangleToScreen(OxRectangle r) =>
            Manager.RectangleToScreen(r);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height, BoundsSpecified specified) =>
            Manager.SetBounds(x, y, width, height, specified);

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            Manager.SetBounds(x, y, width, height);

        public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
        public new event OxDockChangedEvent DockChanged
        {
            add => Manager.DockChanged += value;
            remove => Manager.DockChanged -= value;
        }

        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }

        public new event OxLocationChangedEvent LocationChanged
        {
            add => Manager.LocationChanged += value;
            remove => Manager.LocationChanged -= value;
        }

        public virtual void OnParentChanged(OxParentChangedEventArgs e) { }

        public new event OxParentChangedEvent ParentChanged
        {
            add => Manager.ParentChanged += value;
            remove => Manager.ParentChanged -= value;
        }

        public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }

        public new event OxSizeChangedEvent SizeChanged
        {
            add => Manager.SizeChanged += value;
            remove => Manager.SizeChanged -= value;
        }

        private bool readOnly = false;

        public bool ReadOnly 
        { 
            get => readOnly; 
            set => readOnly = value; 
        }

        IOxControlManager IOxManagingControl<IOxControlManager>.Manager => throw new NotImplementedException();

        protected override void OnCheckedChanged(EventArgs e)
        {
            if (!readOnly)
                base.OnCheckedChanged(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (readOnly)
                Checked = !Checked;

            base.OnClick(e);
        }

        #region Hidden base methods
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
        #endregion
    }
}