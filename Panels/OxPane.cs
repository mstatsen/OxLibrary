using OxLibrary.Controls;
using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPane : Panel, IOxPane, IOxControl<Panel>
    {
        private readonly OxControlManager<Panel> manager;
        public OxControlManager<Panel> Manager => manager;
        public OxPane() : this(OxSize.Empty) { }
        public OxPane(OxSize size)
        {
            manager = OxControlManager.RegisterControl<Panel>(this,OnSizeChanged);
            BorderVisible = false;
            colors = new OxColorHelper(DefaultColor);
            Initialized = false;
            StartSizeChanging();
            try
            {
                DoubleBuffered = true;

                if (!size.Equals(OxSize.Empty))
                    Size = size;

                SetBordersHandlers();
                PrepareInnerControls();
                PrepareColors();
                SetHandlers();
                ReAlign();
                AfterCreated();
            }
            finally
            {
                EndSizeChanging();
            }

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

        public virtual bool OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!SizeChanging && e.Changed)
                base.OnSizeChanged(e);

            return e.Changed;
        }

        protected override sealed void OnSizeChanged(EventArgs e) =>
            base.OnSizeChanged(e);

        private void SetBordersHandlers()
        {
            padding.SizeChanged += BordersSizeChangedHandler;
            borders.SizeChanged += BordersSizeChangedHandler;
            margin.SizeChanged += BordersSizeChangedHandler;
        }

        private void BordersSizeChangedHandler(object sender, BorderEventArgs e)
        {
            if (FullClientOffset.Equals(base.Padding))
                base.Padding = FullClientOffset.AsPadding;

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

        public Color BorderColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        public OxWidth BorderWidth
        {
            get => Borders.Size;
            set => Borders.Size = value;
        }

        public bool BorderVisible
        {
            get => Borders.AllVisible;
            set => Borders.AllVisible = value;
        }

        private readonly OxBorders margin = new();
        private bool blurredBorder;

        private Color GetMarginsColor() =>
            Parent is null
            || blurredBorder
                ? Colors.Lighter(7)
                : Parent.BackColor;

        public new OxBorders Margin => margin;

        public bool BlurredBorder
        {
            get => blurredBorder;
            set
            {
                blurredBorder = value;
                ApplyMarginsColor();
            }
        }

        private void ApplyMarginsColor() =>
            BackColor = GetMarginsColor();

        protected override void OnParentBackColorChanged(EventArgs e) =>
            ApplyMarginsColor();

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            ApplyMarginsColor();
        }

        public virtual Color GetBordersColor() =>
            Enabled
            || !UseDisabledStyles
                ? BaseColor
                : Colors.Lighter(2);

        public void StartSizeChanging() =>
            SizeChanging = true;

        public void EndSizeChanging() => 
            SizeChanging = false;

        public Color BaseColor
        {
            get => colors.BaseColor;
            set
            {
                BaseColorChanging = true;

                try
                {
                    colors.BaseColor = value;
                    PrepareColors();

                    if (!BaseColorChanging)
                        Update();
                }
                finally
                { 
                    BaseColorChanging = false; 
                }
            }
        }

        /*
        public new OxPane? Parent 
        { 
            get => (OxPane)base.Parent;
            set => base.Parent = value; 
        }
        */

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
            ApplyMarginsColor();
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

        private OxBorders FullClientOffset =>
            Padding + Borders + Margin;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Pen pen = new(Color.Red, OxWh.Int(BorderWidth)))
            {
                foreach (var border in Borders)
                {
                    if (border.Value.Size.Equals(OxWh.W0))
                        continue;

                    /*                    
                    e.Graphics.DrawLine(
                        pen,
                        border.Key is OxDock.Right ? ClientRectangle.Width - 1 : ClientRectangle.Left + 1,
                        border.Key is OxDock.Bottom ? ClientRectangle.Height - 1 : ClientRectangle.Top + 1,
                        border.Key is OxDock.Left ? ClientRectangle.Left + 1 : ClientRectangle.Width - 1,
                        border.Key is OxDock.Top ? ClientRectangle.Top + 1 : ClientRectangle.Height - 1
                    );
                    */

                    e.Graphics.DrawLine(
                        pen,
                        border.Key is OxDock.Right ? ClientRectangle.Width - 1 : 1,
                        border.Key is OxDock.Bottom ? ClientRectangle.Height - 1 : 1,
                        border.Key is OxDock.Left ? 1 : ClientRectangle.Width - 1,
                        border.Key is OxDock.Top ? 1 : ClientRectangle.Height - 1
                    );
                }
            };
        }

        protected virtual void SetHandlers() { }

        protected virtual void AfterCreated() { }

        protected bool Initialized { get; set; } = false;

        public virtual void ReAlignControls() { }

        public void ReAlign()
        {
            ReAlignControls();
            SendToBack();
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            ReAlign();
        }

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

        protected bool SizeChanging = false;

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