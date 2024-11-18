namespace OxLibrary.Panels
{
    public class OxPane : Panel, IOxPane
    {
        private int savedWidth = 0;
        private int savedHeight = 0;
        private readonly OxColorHelper colors;

        private bool sizeRecalcing = false;

        protected bool SizeRecalcing => sizeRecalcing;

        public void StartSizeRecalcing() =>
            sizeRecalcing = true;

        public void EndSizeRecalcing() =>
            sizeRecalcing = false;

        protected virtual int GetCalcedHeight() =>
            savedHeight is 0 
                ? Height 
                : savedHeight;

        protected virtual int GetCalcedWidth() =>
            savedWidth is 0 
                ? Width 
                : savedWidth;

        public Color BaseColor
        {
            get => colors.BaseColor;
            set
            {
                colors.BaseColor = value;
                PrepareColors();
                Update();
            }
        }

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
            if (sizeRecalcing)
                return;

            int calcedWidth = CalcedWidth;
            int calcedHeight = CalcedHeight;

            StartSizeRecalcing();

            try
            {
                if (IsVariableWidth 
                    && !Width.Equals(calcedWidth))
                    SetWidth(calcedWidth);

                if (IsVariableHeight && !Height.Equals(calcedHeight))
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

        protected static void SetPaneBaseColor(OxPane pane, Color color)
        {
            if (pane is not null)
                pane.BaseColor = color;
        }

        protected static void SetControlBackColor(Control? control, Color color)
        {
            if (control is not null)
                control.BackColor = color;
        }

        protected virtual void PrepareInnerControls() { }

        public OxPane() : this(Size.Empty) { }
        public OxPane(Size contentSize)
        {
            colors = new OxColorHelper(DefaultColor);
            Initialized = false;
            StartSizeRecalcing();
            try
            {
                DoubleBuffered = true;
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

            if (!contentSize.Equals(Size.Empty))
                SetContentSize(contentSize);
        }

        protected virtual void SetHandlers() { }

        private void RecalcSizeHandler(object? sender, EventArgs e)
        {
            if (Initialized && !sizeRecalcing && (IsVariableHeight || IsVariableWidth))
                RecalcSize();
        }

        public int CalcedWidth => GetCalcedWidth();
        public int CalcedHeight => GetCalcedHeight();

        protected void ApplyRecalcSizeHandler(Control control, bool forSizeChange = true, bool forVisibleChange = true)
        {
            if (control is null)
                return;

            if (forVisibleChange)
                control.VisibleChanged += RecalcSizeHandler;

            if (forSizeChange)
                control.SizeChanged += RecalcSizeHandler;
        }

        protected virtual void AfterCreated() { }

        public int SavedWidth => savedWidth;
        public int SavedHeight => savedHeight;

        protected bool Initialized { get; set; } = false;

        public virtual void SetContentSize(int width, int height)
        {
            savedWidth = width;
            savedHeight = height;
            RecalcSize();
        }

        public void SetContentSize(Size size) =>
            SetContentSize(size.Width, size.Height);

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
                Point thisPoint = this.PointToClient(Cursor.Position);

                return (thisPoint.X >= 0) && 
                    (thisPoint.X <= Width) && 
                    (thisPoint.Y >= 0) && 
                    (thisPoint.Y <= Height);

            }
        }

        protected readonly ToolTip ToolTip = new()
        {
            AutomaticDelay = 500,
            InitialDelay = 100,
            ShowAlways = true,
            IsBalloon = true
        };

        public string ToolTipText
        {
            get => ToolTip.GetToolTip(this);
            set => SetToolTipText(value);
        }

        protected virtual void SetToolTipText(string value) =>
            ToolTip.SetToolTip(this, value);
    }
}