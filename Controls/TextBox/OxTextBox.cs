using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls
{
    public class OxTextBox : TextBox, IOxControlWithManager
    {
        public IOxControlManager Manager { get; }

        public OxTextBox() : base()
        {
            Manager = OxControlManagers.RegisterControl(this);
            DoubleBuffered = true;
            AutoSize = false;
            Height = 24;
        }

        #region Implemention of IOxControl using IOxControlManager
        public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
        public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
        public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
        public new IOxBox? Parent
        {
            get => Manager.Parent;
            set => Manager.Parent = value;
        }

        public new short Width
        {
            get => Manager.Width;
            set => Manager.Width = value;
        }

        public new short Height
        {
            get => Manager.Height;
            set => Manager.Height = value;
        }

        public new short Top
        {
            get => Manager.Top;
            set => Manager.Top = value;
        }

        public new short Left
        {
            get => Manager.Left;
            set => Manager.Left = value;
        }

        public new short Bottom =>
            Manager.Bottom;

        public new short Right =>
            Manager.Right;

        public new OxSize Size
        {
            get => Manager.Size;
            set => Manager.Size = value;
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

        public void DoWithSuspendedLayout(Action method) =>
            Manager.DoWithSuspendedLayout(method);


        public new event OxDockChangedEvent DockChanged
        {
            add => Manager.DockChanged += value;
            remove => Manager.DockChanged -= value;
        }

        public new event OxLocationChangedEvent LocationChanged
        {
            add => Manager.LocationChanged += value;
            remove => Manager.LocationChanged -= value;
        }

        public new event OxParentChangedEvent ParentChanged
        {
            add => Manager.ParentChanged += value;
            remove => Manager.ParentChanged -= value;
        }

        public new event OxSizeChangedEvent SizeChanged
        {
            add => Manager.SizeChanged += value;
            remove => Manager.SizeChanged -= value;
        }

        public void AddHandler(OxHandlerType type, Delegate handler) =>
            Manager.AddHandler(type, handler);

        public void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
            Manager.InvokeHandlers(type, args);

        public void RemoveHandler(OxHandlerType type, Delegate handler) =>
            Manager.RemoveHandler(type, handler);

        #region Internal used properties and methods
        [Obsolete("ZBounds it is used only for internal needs")]
        public OxZBounds ZBounds =>
            Manager.ZBounds;
        #endregion

        #endregion

        #region Hidden base methods
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
        #endregion
    }
}