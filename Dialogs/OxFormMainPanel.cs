using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary.Dialogs
{
    public partial class OxFormMainPanel : OxFrameWithHeader
    {
        public OxForm Form { get; internal set; }

        private OxFormMover formMover = default!;

        public OxFormMainPanel(OxForm form) : base()
        {
            Form = form;
            Form.SizeChanged += FormSizeChanged;
            SetTitleButtonsVisible();
            SetHeaderContentSize(34);
            SetRestoreButtonIconAndTooltip();
            SetBordersSize();
            SetHeaderFont();
            SetContentSize(Width, Height);
            SetMarginsSize();
            CreateFormMover();
            SetAnchors();
            BlurredBorder = true;
        }

        private void FormSizeChanged(object? sender, EventArgs e) => SetRestoreButtonIconAndTooltip();

        private void SetHeaderFont() => 
            Header.Label.Font = 
                new(Header.Label.Font.FontFamily, Header.Label.Font.Size + 1, FontStyle.Bold);

        private void SetAnchors() => 
            Anchor = AnchorStyles.Left | AnchorStyles.Top;

        private void SetBordersSize() => 
            Borders.SetSize(OxSize.Small);

        private void CreateFormMover() => 
            formMover = new OxFormMover(Form, Header.Label);

        public void SetHeaderContentSize(int height)
        {
            Header.SetContentSize(Width, height);
            SetButtonsSize();
        }

        public void SetIcon()
        {
            Header.Icon = Form.FormIcon;

            if (Form.FormIcon is Bitmap bitmap)
                Form.Icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        }

        internal void SetTitleButtonsVisible()
        {
            minimizeButton.Visible = Form.CanMinimize;
            restoreButton.Visible = Form.CanMaximize;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            SetButtonsHandlers();
            SetButtonsSize();
            PlaceButtons();
        }

        private void PlaceButtons()
        {
            Header.AddToolButton(closeButton);
            Header.AddToolButton(restoreButton);
            Header.AddToolButton(minimizeButton);
        }

        private void SetButtonsSize()
        {
            foreach (OxClickFrame button in Header.Buttons)
                button.SetContentSize(36, 28);
        }

        private void SetButtonsHandlers()
        {
            closeButton.Click += CloseButtonClickHandler;
            closeButton.GetHoveredColor += CloseButtonGetHoveredColorHandler;
            restoreButton.Click += RestoreButtonClickHandler;
            minimizeButton.Click += MinimizeButtonClickHandler;
        }

        private Color CloseButtonGetHoveredColorHandler() => Color.Red;

        private void MinimizeButtonClickHandler(object? sender, EventArgs e) => 
            SetFormState(FormWindowState.Minimized);

        private void RestoreButtonClickHandler(object? sender, EventArgs e) => 
            SetFormState(FormIsMaximized ? FormWindowState.Normal : FormWindowState.Maximized);

        private readonly OxIconButton closeButton = new(OxIcons.Close, 28)
        {
            IconPadding = 5,
            ToolTipText = "Close"
        };
        private readonly OxIconButton restoreButton = new(OxIcons.Restore, 28)
        {
            IconPadding = 5,
            ToolTipText = "Restore window",
            Default = true
        };
        private readonly OxIconButton minimizeButton = new(OxIcons.Minimize, 28)
        {
            IconPadding = 5,
            ToolTipText = "Minimize window"
        };

        private Bitmap GetRestoreIcon() =>
            Form != null && FormIsMaximized
                ? OxIcons.Restore
                : OxIcons.Maximize;

        private string GetRestoreToopTip() =>
            Form != null && FormIsMaximized
                ? "Restore window"
                : "Maximize window";

        private void SetRestoreButtonIconAndTooltip()
        {
            restoreButton.Icon = GetRestoreIcon();
            restoreButton.ToolTipText = GetRestoreToopTip();
        }

        public void SetFormState(FormWindowState state)
        {
            ContentContainer.Visible = false;

            try
            {
                Form.SetUpSizes();
                Form.WindowState = state;
            }
            finally
            {
                ContentContainer.Visible = true;
            }
        }

        private void CloseButtonClickHandler(object? sender, EventArgs e)
        {
            Form.DialogResult = DialogResult.Cancel;
            Form.Close();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            BorderColor = BaseColor;
            Header.BaseColor = Colors.Darker();
            Header.UnderlineColor = BaseColor;
        }

        public override Color DefaultColor => Color.FromArgb(146, 143, 140);
        public bool FormIsMaximized => Form.WindowState == FormWindowState.Maximized;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            Form.Width = Width;
            Form.Height = Height;

            StartSizeRecalcing();

            try
            {
                SetMarginsSize();
            }
            finally
            {
                EndSizeRecalcing();
            }
        }

        internal void SetMarginsSize() => 
            Margins.SetSize(Form.Sizeble ? OxSize.Medium : OxSize.None);

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (formMover.Processing)
                return;

            Form.Left += Left;
            Form.Top += Top;
            SetMarginsSize();
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();

            foreach (OxBorder margin in Margins.Borders.Values)
                SetMarginHandlers(margin);

            ContentContainer.VisibleChanged += ContentContainerVisibleChanged;
        }

        private void SetMarginHandlers(OxBorder margin)
        {
            margin.MouseDown += ResizerMouseDown;
            margin.MouseUp += MarginMouseUpHandler;
            margin.MouseMove += ResizeHandler;
            margin.MouseLeave += MarginMouseLeaveHandler;
        }

        private void MarginMouseLeaveHandler(object? sender, EventArgs e) =>
            Cursor = Cursors.Default;

        private void MarginMouseUpHandler(object? sender, MouseEventArgs e) =>
            LastDirection = OxDirection.None;

        private void ContentContainerVisibleChanged(object? sender, EventArgs e) => 
            Update();

        private void ResizerMouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not OxBorder border)
                return;

            LastMousePosition = border.PointToScreen(e.Location);
            LastDirection = OxDirectionHelper.GetDirection(border, e.Location);
        }

        private void SetSizerCursor(OxDirection direction) => 
            Cursor = OxDirectionHelper.GetSizerCursor(direction);

        private Point LastMousePosition = new(-1, -1);
        private OxDirection LastDirection = OxDirection.None;
        private bool ResizeProcessing = false;

        private void ResizeHandler(object? sender, MouseEventArgs e)
        {
            if (!Form.Sizeble)
                return;

            if (ResizeProcessing)
                return;

            OxBorder? border = (OxBorder?)sender;

            if (border == null)
                return;

            if (LastDirection == OxDirection.None)
            {
                SetSizerCursor(
                    OxDirectionHelper.GetDirection(border, e.Location)
                );
                return;
            }

            if (LastMousePosition.Equals(e.Location))
                return;

            Point newLastMousePosition = border.PointToScreen(e.Location);
            Point oldSize = new(Width, Height);
            Point newSize = new(oldSize.X, oldSize.Y);
            Point delta = new(
                newLastMousePosition.X - LastMousePosition.X,
                newLastMousePosition.Y - LastMousePosition.Y
            );

            if (OxDirectionHelper.ContainsRight(LastDirection))
                newSize.X += delta.X;

            if (OxDirectionHelper.ContainsBottom(LastDirection))
                newSize.Y += delta.Y;

            if (OxDirectionHelper.ContainsLeft(LastDirection))
                newSize.X -= delta.X;

            if (OxDirectionHelper.ContainsTop(LastDirection))
                newSize.Y -= delta.Y;

            LastMousePosition = newLastMousePosition;

            if (newSize.X <= Form.MinimumSize.Width)
                newSize.X = Form.MinimumSize.Width;

            if (newSize.Y <= Form.MinimumSize.Height)
                newSize.Y = Form.MinimumSize.Height;

            List<Point> sizePoints = OxFormMover.WayPoints(oldSize, newSize, 30);

            ResizeProcessing = true;
            Form.SuspendLayout();
            SuspendLayout();

            foreach (Point point in sizePoints)
            {
                Point newLocationStep = new(Form.Left, Form.Top);

                if (OxDirectionHelper.ContainsLeft(LastDirection))
                    newLocationStep.X -= point.X - Width;

                if (OxDirectionHelper.ContainsTop(LastDirection))
                    newLocationStep.Y -= point.Y - Height;

                if (!Form.Location.Equals(newLocationStep))
                    Form.Location = newLocationStep;

                Size newSizeStep = new(point);

                if (!Size.Equals(newSizeStep))
                    Size = newSizeStep;
            }

            ResumeLayout();
            Form.ResumeLayout();
            ResizeProcessing = false;
        }
    }
}