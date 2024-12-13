using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using System.Runtime.InteropServices;

namespace OxLibrary.Controls
{
    public class OxTextBox : TextBox, IOxControlWithManager
    {
        public IOxControlManager Manager { get; }

        public OxTextBox() : base()
        {
            Manager = OxControlManagers.RegisterControl(this);
            DoubleBuffered = true;
            AutoSize = OxB.F;
            Height = 24;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (ReadOnly)
            {
                Cursor = Cursors.Arrow;
                OxControlHelper.HideTextCursor(Handle);
            }
            else
                Cursor = Cursors.Default;
        }

        public new string Text
        {
            get => base.Text;
            set => base.Text = value.Replace("\r\n", "\n").Replace("\n", "\r\n");
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

        public void WithSuspendedLayout(Action method) =>
            Manager.WithSuspendedLayout(method);


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

        public new event OxBoolChangedEvent AutoSizeChanged
        {
            add => Manager.AutoSizeChanged += value;
            remove => Manager.AutoSizeChanged -= value;
        }

        public new event OxBoolChangedEvent EnabledChanged
        {
            add => Manager.EnabledChanged += value;
            remove => Manager.EnabledChanged -= value;
        }

        public new event OxBoolChangedEvent VisibleChanged
        {
            add => Manager.VisibleChanged += value;
            remove => Manager.VisibleChanged -= value;
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

        public new OxBool AutoSize
        { 
            get => Manager.AutoSize;
            set => Manager.AutoSize = value;
        }

        public bool IsAutoSize =>
            Manager.IsAutoSize;

        public new OxBool Enabled 
        { 
            get => Manager.Enabled; 
            set => Manager.Enabled = value;
        }

        public bool IsEnabled =>
            Manager.IsEnabled;

        public new OxBool Visible 
        { 
            get => Manager.Visible;
            set => Manager.Visible = value;
        }

        public bool IsVisible =>
            Manager.IsVisible;
        #endregion

        #endregion

        #region Hidden base methods
        protected sealed override void OnAutoSizeChanged(EventArgs e) { }
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnEnabledChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
        protected sealed override void OnVisibleChanged(EventArgs e) { }

        public virtual void OnAutoSizeChanged(OxBoolChangedEventArgs e) { }

        public virtual void OnEnabledChanged(OxBoolChangedEventArgs e) { }

        public virtual void OnVisibleChanged(OxBoolChangedEventArgs e) { }

        public void SetAutoSize(bool value) =>
            Manager.SetAutoSize(value);

        public void SetEnabled(bool value) =>
            Manager.SetEnabled(value);

        public void SetVisible(bool value) =>
            Manager.SetVisible(value);
        #endregion
    }
}