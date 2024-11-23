using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPane : Panel, IOxPane, IOxControlContainer<Panel>
    {
        private readonly OxControlManager<Panel> manager;
        public IOxControlManager Manager => manager;
        public OxPane() : this(OxSize.Empty) { }
        public OxPane(OxSize size)
        {
            manager = OxControlManager.RegisterControl<Panel>(this, OnSizeChanged);
            BorderVisible = false;
            colors = new(DefaultColor);
            Initialized = false;

            SilentSizeChange(
                () =>
                {
                    DoubleBuffered = true;

                    if (!size.Equals(OxSize.Empty))
                        Size = new(size);

                    SetBordersHandlers();
                    PrepareInnerControls();
                    PrepareColors();
                    SetHandlers();
                    AfterCreated();
                },
                Size
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

        public new virtual OxRectangle DisplayRectangle => manager.DisplayRectangle;

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

        public void RealignControls() => 
            manager.RealignControls();

        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (SizeChanging)
                return;

            base.OnSizeChanged(e);
        }

        private void SetBordersHandlers()
        {
            padding.SizeChanged += BordersSizeChangedHandler;
            borders.SizeChanged += BordersSizeChangedHandler;
            margin.SizeChanged += BordersSizeChangedHandler;
        }

        private void BordersSizeChangedHandler(object sender, BorderEventArgs e)
        {
            //if (FullControlZone.Equals(base.Padding))
                //base.Padding = FullControlZone.AsPadding;

            Invalidate();
        }

        private readonly OxColorHelper colors;

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

        private Color borderColor = Color.Black;
        public Color BorderColor
        {
            get =>
                useDefaultBorderColor
                ? BaseColor
                : borderColor;
            set
            {
                useDefaultBorderColor = false;
                borderColor = value;
                Invalidate();
            }
        }

        private OxWidth borderWidth = OxWh.W0;

        public OxWidth BorderWidth
        {
            get => borderWidth;
            set => borderWidth = value;
        }

        public bool BorderVisible
        {
            get => Borders.AllVisible;
            set => Borders.AllVisible = value;
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

        public virtual Color GetBordersColor() =>
            Enabled
            || !UseDisabledStyles
                ? BaseColor
                : Colors.Lighter(2);

        public Color BaseColor
        {
            get => colors.BaseColor;
            set
            {
                if (BaseColorChanging)
                    return;

                BaseColorChanging = true;

                try
                {
                    colors.BaseColor = value;
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

        protected virtual void PrepareColors()
        {
            BackColor = Colors.Lighter(Enabled || !useDisabledStyles ? 7 : 8);
            ForeColor = Colors.Darker(7);
        }

        private bool useDisabledStyles = true;

        public bool UseDisabledStyles
        {
            get => useDisabledStyles;
            set => SetUseDisabledStyles(value);
        }

        protected virtual void SetUseDisabledStyles(bool value) =>
            useDisabledStyles = value;

        protected virtual void PrepareInnerControls() { }

        public OxRectangle FullControlZone =>
            ClientRectangle - (Padding + Borders + Margin);

        public OxRectangle ControlZone =>
            manager.ControlZone;

        private OxRectangle BorderRectangle
        {
            get
            {
                OxRectangle bordersRectangle = ClientRectangle - Margin;
                bordersRectangle.X |= OxWh.W1;
                bordersRectangle.Y |= OxWh.W1;
                bordersRectangle.Width -= OxWh.W2;
                bordersRectangle.Height -= OxWh.W2;
                return bordersRectangle;
            }
        }

        private void DrawBorders(Graphics g)
        {
            if (Borders.IsEmpty)
                return;

            using (Pen pen = new(BorderColor, OxWh.Int(BorderWidth)))
            //using (Pen pen = new(Color.Red, OxWh.Int(BorderWidth)))
            {
                OxRectangle rect = BorderRectangle;
                int X1 = OxWh.Int(rect.X);
                int Y1 = OxWh.Int(rect.Y);
                int X2 = OxWh.Int(OxWh.Add(rect.X, rect.Width));
                int Y2 = OxWh.Int(OxWh.Add(rect.Y, rect.Height));

                foreach (var border in Borders)
                {
                    if (border.Value.IsEmpty)
                        continue;

                    switch (border.Key)
                    {
                        case OxDock.Left:
                            g.DrawLine(pen, X1, Y1, X1, Y2);
                            break;
                        case OxDock.Right:
                            g.DrawLine(pen, X2, Y1, X2, Y2);
                            break;
                        case OxDock.Top:
                            g.DrawLine(pen, X1, Y1, X2, Y1);
                            break;
                        case OxDock.Bottom:
                            g.DrawLine(pen, X1, Y2, X2, Y2);
                            break;
                    }
                }
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBorders(e.Graphics);
        }

        protected virtual void SetHandlers() { }

        protected virtual void AfterCreated() { }

        protected bool Initialized { get; set; } = false;

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            PrepareColors();
        }

        public OxColorHelper Colors => colors;
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

        private readonly OxControls oxControls = new();
        public OxControls OxControls => oxControls;

        private readonly OxDockedControls oxDockedControls = new();
        public OxDockedControls OxDockedControls => oxDockedControls;

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
            set => SetIcon(value);
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
    }
}