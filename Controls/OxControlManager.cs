using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxControlManager<TBaseControl> : IOxControlManager
        where TBaseControl : notnull, Control
    {
        private readonly TBaseControl managingControl;
        public TBaseControl ManagingControl => managingControl;
        private readonly Func<SizeChangedEventArgs, bool> managingOnSizeChanged;
        internal OxControlManager(TBaseControl managingControl, Func<SizeChangedEventArgs, bool> onSizeChanged)
        {
            this.managingControl = managingControl;
            this.managingControl.Disposed += ControlDisposedHandler;
            managingOnSizeChanged = onSizeChanged;
        }

        private void ControlDisposedHandler(object? sender, EventArgs e)
        {
            OxControlManager.UnRegisterControl(managingControl);
        }

        private bool SizeChanging = false;

        private void StartSizeChanging() =>
            SizeChanging = true;

        private void EndSizeChanging() =>
            SizeChanging = false;

        public OxWidth Width
        {
            get => OxWh.W(managingControl.Width);
            set
            {
                OxWidth oldWidth = Width;

                if (oldWidth.Equals(value))
                    return;

                managingControl.Width = OxWh.Int(value);
                OnSizeChanged(new SizeChangedEventArgs(oldWidth, Height, value, Height));
            }
        }

        public OxWidth Height
        {
            get => OxWh.W(managingControl.Height);
            set
            {
                OxWidth oldHeight = Height;

                if (oldHeight.Equals(value))
                    return;

                managingControl.Height = OxWh.Int(value);
                OnSizeChanged(new SizeChangedEventArgs(Width, oldHeight, Width, value));
            }
        }
        public OxWidth Bottom => OxWh.W(managingControl.Bottom);

        public OxWidth Right => OxWh.W(managingControl.Right);

        public OxWidth Top
        {
            get => OxWh.W(managingControl.Top);
            set => managingControl.Top = OxWh.Int(value);
        }

        public OxWidth Left
        {
            get => OxWh.W(managingControl.Left);
            set => managingControl.Left = OxWh.Int(value);
        }

        public OxDock Dock
        {
            get => OxDockHelper.Dock(managingControl.Dock);
            set => managingControl.Dock = OxDockHelper.Dock(value);
        }

        public OxSize Size
        {
            get => new(Width, Height);
            set
            {
                bool changed = false;
                OxWidth oldWidth = Width;
                OxWidth oldHeight = Height;

                StartSizeChanging();
                try
                {
                    if (!oldWidth.Equals(value.Width))
                    {
                        changed = true;
                        Width = value.Width;
                    }

                    if (!oldHeight.Equals(value.Height))
                    {
                        Height = value.Height;
                        changed = true;
                    }
                }
                finally
                {
                    EndSizeChanging();

                    if (changed)
                        OnSizeChanged(new SizeChangedEventArgs(oldWidth, oldHeight, Width, Height));
                }
            }
        }

        public OxSize ClientSize 
        { 
            get => new(managingControl.ClientSize);
            set => managingControl.ClientSize = value.Size;
        }

        public OxPoint Location
        {
            get => new(managingControl.Location);
            set => managingControl.Location = value.Point;
        }

        public OxSize MinimumSize
        {
            get => new(managingControl.MinimumSize);
            set => managingControl.MinimumSize = value.Size;
        }
        public OxSize MaximumSize
        {
            get => new(managingControl.MaximumSize);
            set => managingControl.MaximumSize = value.Size;
        }

        //public Control? Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!SizeChanging || !e.Changed)
                return false;

            managingOnSizeChanged(e);
            return true;
        }

        public void SetBounds(OxWidth x, OxWidth y, OxWidth width, OxWidth height) =>
            managingControl.SetBounds(
                OxWh.Int(x),
                OxWh.Int(y),
                OxWh.Int(width),
                OxWh.Int(height)
            );
    }

    public static class OxControlManager
    {
        private static readonly Dictionary<Control, IOxControlManager> Controls = new();

        public static OxControlManager<TBaseControl> RegisterControl<TBaseControl>(
            TBaseControl managingControl,
            Func<SizeChangedEventArgs, bool> onSizeChanged)
            where TBaseControl : Control
        {
            OxControlManager<TBaseControl> oxControlManager = new(managingControl, onSizeChanged);
            Controls.Add(managingControl, oxControlManager);
            return oxControlManager;
        }

        public static void UnRegisterControl(Control control) =>
            Controls.Remove(control);
    }
}