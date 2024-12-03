using OxLibrary.Controls;
using OxLibrary.Forms;
using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxPanel : Panel, IOxPanel
    {
        public IOxContainerManager Manager { get; }
        public OxPanel() : this(OxSize.Empty) { }
        public OxPanel(OxSize size)
        {
            Manager = OxControlManagers.RegisterContainer(this);
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

        protected bool IsVariableWidth =>
            Parent is null
            || OxDockHelper.IsVariableWidth(Dock);

        protected bool IsVariableHeight =>
            Parent is null
            || OxDockHelper.IsVariableHeight(Dock);


        private bool useDisabledStyles = true;

        public bool UseDisabledStyles
        {
            get => useDisabledStyles;
            set => SetUseDisabledStyles(value);
        }

        protected virtual void SetUseDisabledStyles(bool value) =>
            useDisabledStyles = value;

        protected virtual void PrepareInnerComponents() { }

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

        public bool IsHovered
        {
            get
            {
                Point thisPoint = PointToClient(Cursor.Position);
                return (thisPoint.X >= 0)
                    && (thisPoint.X <= Size.WidthInt)
                    && (thisPoint.Y >= 0)
                    && (thisPoint.Y <= Size.HeightInt);
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

        public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
        public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
        public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
        public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }

        public virtual bool HandleParentPadding => true;

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

        public new virtual IOxContainer? Parent
        {
            get => Manager.Parent;
            set
            {
                Manager.Parent = value;
                PrepareColors();
            }
        }

        #region IOxWithPadding implementaion
        private readonly OxBorders padding = new();
        public new OxBorders Padding => padding;
        #endregion

        #region IOxWithBorder implementation
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

        protected virtual Color GetBorderColor() =>
            Enabled
            || !UseDisabledStyles
                ? BaseColor
                : Colors.Lighter(2);
        #endregion

        #region IOxWithMargin implementation
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

        private Color MarginColor =>
            !BlurredBorder
            && Parent is not null
                ? Parent.BackColor
                : BackColor;
        #endregion

        #region IOxWithColorHelper implementation
        public OxColorHelper Colors { get; }
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

        private bool BaseColorChanging = false;

        protected virtual Color GetBackColor() =>
            Colors.Lighter(Enabled || !useDisabledStyles ? 7 : 8);

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
        #endregion

        #region IWithIcon implementation
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
        #endregion

        #region Implemention of IOxContainer using IOxContainerManager
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

        public OxRectangle OuterControlZone =>
            Manager.OuterControlZone;

        public void RealignControls(OxDockType dockType = OxDockType.Unknown) =>
            Manager.RealignControls(dockType);

        public bool Realigning =>
            Manager.Realigning;

        public virtual OxRectangle ControlZone =>
            Manager.ControlZone;

        public OxControls OxControls =>
            Manager.OxControls;
        #endregion

        #region Hidden base methods
        protected sealed override void OnDockChanged(EventArgs e) { }
        protected sealed override void OnLocationChanged(EventArgs e) { }
        protected sealed override void OnParentChanged(EventArgs e) { }
        protected sealed override void OnSizeChanged(EventArgs e) { }
#endregion
    }
}