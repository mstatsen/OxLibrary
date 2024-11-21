using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Dialogs
{
    public class OxForm : Form, IOxControl<Form>
    {
        private readonly bool Initialized = false;
        public OxFormMainPanel MainPanel { get; internal set; }

        private readonly OxControlManager<Form> manager;

        public OxControlManager<Form> Manager => manager;

        public virtual bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (SizeChanging || !e.Changed)
                return false;

            SizeChanging = true;
            try
            {
                base.OnSizeChanged(e);

                if (MainPanel is not null)
                    MainPanel.Size = new(Size);
            }
            finally
            {
                SizeChanging = false;
            }

            return true;
        }

        private readonly OxControls oxControls = new();
        public OxControls OxControls => oxControls;

        public OxForm()
        {
            manager = OxControlManager.RegisterControl<Form>(this, OnSizeChanged);
            DoubleBuffered = true;
            AutoSize = true;
            MainPanel = CreateMainPanel();
            PlaceMainPanel();
            SetUpForm();
            Initialized = true;
        }

        protected virtual void SetUpForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            SetUpSizes();
            StartPosition = FormStartPosition.CenterParent;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MainPanel.SetIcon();
        }

        public void SetUpSizes()
        {
            MaximumSize = new(Screen.GetWorkingArea(this).Size);
            OxSize wantedMinimumSize = WantedMinimumSize;
            MinimumSize = new(
                OxWh.Min(wantedMinimumSize.Width, MaximumSize.Width),
                OxWh.Min(wantedMinimumSize.Height, MaximumSize.Height)
            );
        }

        public virtual OxSize WantedMinimumSize => new(OxWh.W640, OxWh.W480);

        protected virtual OxFormMainPanel CreateMainPanel() => new(this);

        private void PlaceMainPanel()
        {
            MainPanel.Parent = this;
            MainPanel.StartSizeChanging();

            try
            {
                MainPanel.Left = OxWh.W0;
                MainPanel.Top = OxWh.W0;
                MainPanel.Size = new(Width, Height);
                MainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
            }
            finally
            {
                MainPanel.EndSizeChanging();
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (Initialized)
                MainPanel.Controls.Add(e.Control);
            else base.OnControlAdded(e);
        }

        protected override void OnTextChanged(EventArgs e) =>
            MainPanel.Text = Text;

        protected bool SizeChanging = false;

        private bool canMaximize = true;
        private bool canMinimize = true;

        public bool CanMaximize
        {
            get => canMaximize;
            set
            {
                canMaximize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }

        public bool CanMinimize
        {
            get => canMinimize;
            set
            {
                canMinimize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }


        private bool sizeble = true;
        public bool Sizeble
        {
            get => sizeble;
            set
            {
                sizeble = value;
                MainPanel.SetMarginsSize();
            }
        }

        public virtual Bitmap? FormIcon => null;

        public void ClearConstraints()
        {
            MinimumSize = OxSize.Empty;
            MaximumSize = MinimumSize;
        }

        public void FreezeSize()
        {
            if (MainPanel is null)
                return;

            MinimumSize = MainPanel.Size;
            MaximumSize = MinimumSize;
        }

        public Color BaseColor
        {
            get => MainPanel.BaseColor;
            set => MainPanel.BaseColor = value;
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

        public new OxWidth Bottom => manager.Bottom;

        public new OxWidth Right => manager.Right;

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

        public new OxDock Dock
        {
            get => manager.Dock;
            set => manager.Dock = value;
        }

        public new IOxControl? Parent
        {
            get => manager.Parent;
            set => manager.Parent = value;
        }
        public bool HasOxChildren => manager.HasOxChildren;

        public new OxRectangle ClientRectangle => manager.ClientRectangle;

        public new OxRectangle DisplayRectangle => manager.DisplayRectangle;

        public new OxRectangle Bounds
        {
            get => manager.Bounds;
            set => manager.Bounds = value;
        }

        public new OxSize PreferredSize => manager.PreferredSize;

        public new OxPoint AutoScrollOffset
        {
            get => manager.AutoScrollOffset;
            set => manager.AutoScrollOffset = value;
        }

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

        protected override sealed void OnSizeChanged(EventArgs e) =>
            base.OnSizeChanged(e);
    }
}