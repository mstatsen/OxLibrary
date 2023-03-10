using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxSidePanel : OxFrame
    {
        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            BorderVisible = false;
            Sider.Parent = this;
            SiderButton.Parent = Sider;
            SiderButton.Click += ExpandHandler;
        }

        private readonly OxPanel Sider = new(new Size(16, 1));
        private readonly OxIconButton SiderButton = new(OxIcons.left, 16)
        {
            Dock = DockStyle.Fill,
            HiddenBorder = false
        };

        public OxSidePanel(Size contentSize) : base(contentSize) { }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            SiderButton.BaseColor = BaseColor;
        }

        protected override int GetCalcedWidth()
        {
            if (OxDockHelper.IsVertical(OxDockHelper.Dock(Dock)))
                return base.GetCalcedWidth();

            int calcedWidth = base.GetCalcedWidth() + Sider.CalcedWidth;

            if (!Expanded)
                calcedWidth -= SavedWidth;

            return calcedWidth;
        }

        protected override int GetCalcedHeight()
        {
            if (!OxDockHelper.IsVertical(OxDockHelper.Dock(Dock)))
                return base.GetCalcedHeight();

            int calcedHeight = base.GetCalcedHeight() + Sider.CalcedHeight;

            if (!Expanded)
                calcedHeight -= SavedHeight;

            return calcedHeight;
        }

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            Sider.ReAlign();
            Borders.ReAlign();
            Margins.ReAlign();
            SendToBack();
        }

        protected override void OnDockChanged(EventArgs e)
        {
            OxDock oxDock = OxDockHelper.Dock(Dock);
            Sider.Dock = OxDockHelper.Dock(OxDockHelper.Opposite(oxDock));

            if (OxDockHelper.IsVertical(oxDock))
                Sider.SetContentSize(1, 16);
            else Sider.SetContentSize(16, 1);

            base.OnDockChanged(e);
        }

        private bool expanded = true;

        private void ExpandHandler(object? sender, EventArgs e) =>
            Expanded = !Expanded;

        private void SetExpanded(bool value)
        {
            expanded = value;

            if (!Expandable)
                return;

            OnExpandedChanged?.Invoke(this, EventArgs.Empty);

            ContentContainer.StartSizeRecalcing();
            try
            {
                Paddings[OxDockHelper.Dock(Dock)].Visible = value;
                Paddings[OxDockHelper.Dock(Sider.Dock)].Visible = value;
                ContentContainer.Visible = value;
                SiderButton.Icon = SiderButtonIcon;
            }
            finally
            {
                Update();
                ContentContainer.EndSizeRecalcing();
            }

            if (expanded)
                OnAfterExpand?.Invoke(this, EventArgs.Empty);
            else OnAfterCollapse?.Invoke(this, EventArgs.Empty);
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            ApplyRecalcSizeHandler(ContentContainer, false, true);
        }

        public bool Expanded
        {
            get => expanded;
            set => SetExpanded(value);
        }

        private bool Expandable => IsVariableWidth || IsVariableHeight;

        public Bitmap? SiderButtonIcon =>
            Dock switch
            {
                DockStyle.Left => expanded ? OxIcons.left : OxIcons.right,
                DockStyle.Right => expanded ? OxIcons.right : OxIcons.left,
                DockStyle.Top => expanded ? OxIcons.up : OxIcons.down,
                DockStyle.Bottom => expanded ? OxIcons.down : OxIcons.up,
                _ => null,
            };

        public OxBorders SiderButtonBorders => SiderButton.Borders;

        public Color SiderButtonColor
        {
            get => SiderButton.BaseColor;
            set => SiderButton.BaseColor = value;
        }

        public void Expand() => Expanded = true;
        public void Collapse() => Expanded = false;

        public EventHandler? OnExpandedChanged;
        public EventHandler? OnAfterExpand;
        public EventHandler? OnAfterCollapse;
    }
}