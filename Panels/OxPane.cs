using OxLibrary.Dialogs;

namespace OxLibrary.Panels
{
    public class OxPane : Panel, IOxPane
    {
        private readonly OxColorHelper colors;

        //TODO: link on base.Padding with OxBorders.SizeChanged

        private readonly OxBorders padding = new();
        public new OxBorders Padding => padding;

        private readonly OxBorders borders =
            new()
            {
                Size = OxSize.XXS
            };

        public OxBorders Borders => borders;

        public Color BorderColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        public int BorderWidth
        {
            get => Borders.SizeInt;
            set => Borders.SizeInt = value;
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

        protected bool SizeRecalcing { get; private set; } = false;

        public void StartSizeRecalcing() =>
            SizeRecalcing = true;

        public void EndSizeRecalcing() =>
            SizeRecalcing = false;

        protected virtual int GetCalcedHeight() =>
            Height;

        protected virtual int GetCalcedWidth() =>
            Width;

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
            || Dock is DockStyle.Left
            || Dock is DockStyle.Right
            || Dock is DockStyle.None;

        protected bool IsVariableHeight =>
            Parent is null
            || Dock is DockStyle.Top
            || Dock is DockStyle.Bottom
            || Dock is DockStyle.None;

        public void RecalcSize()
        {
            if (SizeRecalcing)
                return;

            int calcedWidth = CalcedWidth;
            int calcedHeight = CalcedHeight;

            StartSizeRecalcing();

            try
            {
                if (IsVariableWidth
                    && !Width.Equals(calcedWidth))
                    SetWidth(calcedWidth);

                if (IsVariableHeight
                    && !Height.Equals(calcedHeight))
                    SetHeight(calcedHeight);
            }
            finally
            {
                PrepareColors();
                EndSizeRecalcing();
            }
        }

        protected virtual void SetHeight(int value) => Height = value;

        protected virtual void SetWidth(int value) => Width = value;

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using Pen pen = new(BaseColor, 1);
            e.Graphics.DrawRectangle(pen, new Rectangle(1, 1, Width - 1, Height - 1));
        }

        public OxPane() : this(Size.Empty) { }
        public OxPane(Size contentSize)
        {
            colors = new OxColorHelper(DefaultColor);
            Initialized = false;
            StartSizeRecalcing();
            try
            {
                DoubleBuffered = true;

                if (!contentSize.Equals(Size.Empty))
                    Size = contentSize;

                PrepareInnerControls();
                PrepareColors();
                RecalcSize();
                SetHandlers();
                ReAlign();
                AfterCreated();
            }
            finally
            {
                EndSizeRecalcing();
            }

            Initialized = true;
            Visible = true;
        }

        protected virtual void SetHandlers() { }

        private void RecalcSizeHandler(object? sender, EventArgs e)
        {
            if (Initialized 
                && !SizeRecalcing 
                && (IsVariableHeight || IsVariableWidth))
                RecalcSize();
        }

        public int CalcedWidth => GetCalcedWidth();
        public int CalcedHeight => GetCalcedHeight();
        protected void ApplyVisibleChangedHandler(Control control)
        {
            if (control is null)
                return;

            control.VisibleChanged += RecalcSizeHandler;
        }

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

        public new string? Text
        {
            get => GetText();
            set => SetText(value);
        }

        protected virtual void SetText(string? value) =>
            base.Text = value;

        protected virtual string? GetText() =>
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
                    (thisPoint.X <= Width) && 
                    (thisPoint.Y >= 0) && 
                    (thisPoint.Y <= Height);
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