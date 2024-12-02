using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxPanel : Panel, IOxPanel
    {
        private readonly OxControlManager<Panel> manager;
        public IOxControlManager Manager => manager;
        public OxPanel() : this(OxSize.Empty) { }
        public OxPanel(OxSize size)
        {
            oxControls = new(this);
            manager = OxControlManager.RegisterControl<Panel>(this);
            BorderVisible = false;
            Colors = new(DefaultColor);
            Initialized = false;

            DoWithSuspendedLayout(
                () =>
                {
                    DoubleBuffered = true;

                    if (!size.Equals(OxSize.Empty))
                        Size = new(size);

                    PrepareInnerComponents();
                    SetHandlers();
                    AfterCreated();
                }
            );

            Initialized = true;
            Visible = true;
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
            set
            {
                manager.Parent = value;
                PrepareColors();
            }
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

        public new event OxLocationChanged LocationChanged
        {
            add => manager.LocationChanged += value;
            remove => manager.LocationChanged -= value;
        }
        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }

        public new event OxSizeChanged SizeChanged
        {
            add => manager.SizeChanged += value;
            remove => manager.SizeChanged -= value;
        }

        public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }

        public void RealignControls(OxDockType dockType = OxDockType.Unknown) =>
            manager.RealignControls(dockType);

        public bool Realigning => 
            manager.Realigning;

        public virtual bool HandleParentPadding => true;

        private readonly OxBorders padding = new();
        public new OxBorders Padding => padding;

        private readonly OxBorders borders =
            new()
            {
                Size = OxWh.W1
            };

        public OxBorders Borders => borders;

        private bool useDefaultBorderColor = true;
        public bool UseDefaultBorderColor
        {
            get => useDefaultBorderColor;
            set
            {
                useDefaultBorderColor = value;
                Invalidate();
            }
        }

        private Color borderColor = Color.Transparent;
        public Color BorderColor
        {
            get =>
                useDefaultBorderColor
                ? GetBorderColor()
                : borderColor;
            set
            {
                useDefaultBorderColor = false;
                borderColor = value;
                Invalidate();
            }
        }

        public void SetBorderWidth(OxWidth value) =>
            Borders.Size = value;

        public void SetBorderWidth(OxDock dock, OxWidth value) =>
            Borders[dock].Size = value;

        public bool BorderVisible
        {
            get => Borders.GetVisible();
            set => Borders.SetVisible(value);
        }

        private readonly OxBorders margin = new();

        public new OxBorders Margin => margin;

        private bool blurredBorder = false;
        public bool BlurredBorder
        {
            get => blurredBorder;
            set
            {
                blurredBorder = value;

                if (!margin.IsEmpty)
                    Invalidate();
            }
        }

        protected virtual Color GetBorderColor() =>
            Enabled
            || !UseDisabledStyles
                ? BaseColor
                : Colors.Lighter(2);

        private Color MarginColor =>
            !BlurredBorder
            && Parent is not null
                ? Parent.BackColor
                : BackColor;

        public Color BaseColor
        {
            get => Colors.BaseColor;
            set
            {
                if (BaseColorChanging)
                    return;

                BaseColorChanging = true;

                try
                {
                    Colors.BaseColor = value;
                    PrepareColors();
                }
                finally
                { 
                    BaseColorChanging = false;
                    Invalidate();
                }
            }
        }

        protected bool IsVariableWidth =>
            Parent is null
            || OxDockHelper.IsVariableWidth(Dock);

        protected bool IsVariableHeight =>
            Parent is null
            || OxDockHelper.IsVariableHeight(Dock);

        private bool BaseColorChanging = false;

        protected virtual Color GetBackColor() =>
            Colors.Lighter(Enabled || !useDisabledStyles? 7 : 8);

        protected virtual Color GetForeColor() =>
            Colors.Darker(Enabled || !useDisabledStyles ? 7 : -3);

        public virtual void PrepareColors()
        {
            BackColor = GetBackColor();
            ForeColor = GetForeColor();
            ColorizeControls();
        }

        private void ColorizeControls()
        {
            foreach (IOxControl control in OxControls)
            {
                if (control is IOxPanel panel)
                    panel.BaseColor = BaseColor;
                else control.BackColor = BackColor;
            }
        }

        private bool useDisabledStyles = true;

        public bool UseDisabledStyles
        {
            get => useDisabledStyles;
            set => SetUseDisabledStyles(value);
        }

        protected virtual void SetUseDisabledStyles(bool value) =>
            useDisabledStyles = value;

        protected virtual void PrepareInnerComponents() { }

        public virtual OxRectangle ControlZone =>
            manager.ControlZone;

        private OxRectangle BorderRectangle => 
            ClientRectangle - Margin;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!BlurredBorder)
                Margin.Draw(e.Graphics, ClientRectangle, MarginColor);

            Borders.Draw(e.Graphics, BorderRectangle, BorderColor);
        }

        protected virtual void SetHandlers() { }

        protected virtual void AfterCreated() { }

        protected bool Initialized { get; set; } = false;

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            PrepareColors();
        }

        public OxColorHelper Colors { get; }
        public virtual Color DefaultColor => Color.FromArgb(142, 142, 138);

        public new string Text
        {
            get
            {
                string text = base.Text;
                return text is null ? string.Empty : text;
            }
            set => SetText(value is null ? string.Empty : value);
        }

        protected virtual void SetText(string value) =>
            base.Text = value;

        protected virtual string GetText() =>
            base.Text;

        public new bool Visible
        {
            get => base.Visible;
            set => SetVisible(value);
        }

        protected virtual void SetVisible(bool value) =>
            base.Visible = value;

        private readonly OxControls oxControls;
        public OxControls OxControls => oxControls;

        public bool IsHovered
        {
            get 
            {
                Point thisPoint = PointToClient(Cursor.Position);
                return (thisPoint.X >= 0) && 
                    (thisPoint.X <= Size.WidthInt) && 
                    (thisPoint.Y >= 0) && 
                    (thisPoint.Y <= Size.HeightInt);
            }
        }

        protected readonly ToolTip ToolTip =
            new()
            {
                AutomaticDelay = 500,
                InitialDelay = 100,
                ShowAlways = true
            };

        public string ToolTipText
        {
            get => ToolTip.GetToolTip(this);
            set => SetToolTipText(value);
        }

        protected virtual void SetToolTipText(string value) => 
            ToolTip.SetToolTip(this, value);

        public Bitmap? Icon
        {
            get => GetIcon();
            set
            {
                switch (Icon)
                {
                    case null when value is null:
                    case not null when Icon.Equals(value):
                        return;
                }

                SetIcon(value);
            }
        }

        protected virtual void SetIcon(Bitmap? value) { }
        protected virtual Bitmap? GetIcon() => null;

        public virtual void PutBack(OxPanelViewer viewer)
        {
            foreach (Form form in viewer.OwnedForms)
                viewer.RemoveOwnedForm(form);

            Initialized = false;
            Initialized = true;
        }

        public DialogResult ShowAsDialog(Control owner, OxDialogButton buttons = OxDialogButton.OK)
        {
            DialogResult result = AsDialog(buttons).ShowDialog(owner);
            PanelViewer?.Dispose();
            PanelViewer = null;
            return result;
        }

        protected OxPanelViewer? PanelViewer;

        public OxPanelViewer AsDialog(OxDialogButton buttons = OxDialogButton.OK)
        {
            PrepareDialogCaption(out string? dialogCaption);
            PanelViewer = new OxPanelViewer(this, buttons)
            {
                Text = dialogCaption
            };
            PanelViewer.ButtonsWithBorders.Clear();
            PrepareDialog(PanelViewer);
            return PanelViewer;
        }

        protected virtual void PrepareDialogCaption(out string? dialogCaption) =>
            dialogCaption = Text;

        protected virtual void PrepareDialog(OxPanelViewer dialog) { }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members
        private static new void OnLocationChanged(EventArgs e) { }
        private static new void OnSizeChanged(EventArgs e) { }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0060 // Remove unused parameter
    }
}