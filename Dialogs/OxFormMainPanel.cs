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
            Form.SizeChanged += (s, e) => SetRestoreButtonIcon();
            SetTitleButtonsVisible();
            SetHeaderContentSize();
            SetIcon();
            SetRestoreButtonIcon();
            SetBordersSize();
            SetHeaderFont();
            SetContentSize(Width, Height);
            SetMarginsSize();
            CreateFormMover();
            SetAnchors();
            BlurredBorder = true;
        }

        private void SetHeaderFont() => 
            Header.Label.Font = 
                new Font(Header.Label.Font.FontFamily, Header.Label.Font.Size + 1, FontStyle.Bold);

        private void SetAnchors() => 
            Anchor = AnchorStyles.Left | AnchorStyles.Top;

        private void SetBordersSize() => 
            Borders.SetSize(OxSize.Small);

        private void CreateFormMover() => 
            formMover = new OxFormMover(Form, Header.Label);

        private void SetHeaderContentSize() => 
            Header.SetContentSize(Width, 34);

        private void SetIcon()
        {
            Header.Icon = Form.FormIcon;

            if (Form.FormIcon != null)
                Form.Icon = Icon.FromHandle(Form.FormIcon.GetHicon());
        }

        internal void SetTitleButtonsVisible()
        {
            minimizeButton.Visible = Form.CanMinimize;
            restoreButton.Visible = Form.CanMaximize;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();

            closeButton.Click += CloseButtonClickHandler;
            closeButton.GetHoveredColor += () => Color.Red;
            restoreButton.Click += (s, e) => SetFormState(FormIsMaximized ? FormWindowState.Normal : FormWindowState.Maximized);
            minimizeButton.Click += (s, e) => SetFormState(FormWindowState.Minimized);

            closeButton.SetContentSize(36, 28);
            restoreButton.SetContentSize(36, 28);
            minimizeButton.SetContentSize(36, 28);

            Header.AddToolButton(closeButton);
            Header.AddToolButton(restoreButton);
            Header.AddToolButton(minimizeButton);

        }

        private readonly OxIconButton closeButton = new(OxIcons.close, 28)
        {
            IconPadding = 5
        };
        private readonly OxIconButton restoreButton = new(OxIcons.restore, 28)
        {
            IconPadding = 5,
            Default = true
        };
        private readonly OxIconButton minimizeButton = new(OxIcons.minimize, 28)
        {
            IconPadding = 5
        };

        private Bitmap GetRestoreIcon() =>
            Form != null && FormIsMaximized
                ? OxIcons.restore
                : OxIcons.maximize;

        private void SetRestoreButtonIcon() =>
            restoreButton.Icon = GetRestoreIcon();

        public void SetFormState(FormWindowState state)
        {
            ContentContainer.Visible = false;
            Form.SetUpSizes();
            Form.WindowState = state;
            ContentContainer.Visible = true;
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
            Header.BaseColor = Colors.Darker(1);
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

            foreach (OxBorder border in Margins.Borders.Values)
            {
                border.MouseDown += ResizerMouseDown;
                border.MouseUp += (s, e) => LastDirection = OxDirection.None;
                border.MouseMove += ResizeHandler;
                border.MouseLeave += (s, e) => Cursor = Cursors.Default;
            }

            ContentContainer.VisibleChanged += (s, e) => Update();
        }

        private void ResizerMouseDown(object? sender, MouseEventArgs e)
        {
            OxBorder? border = (OxBorder?)sender;

            if (border == null)
                return;

            LastMousePosition = border.PointToScreen(e.Location);
            LastDirection = OxDirectionHelper.GetDirection(border, e.Location);
        }

        private void SetSizerCursor(OxDirection direction)
        {
            if (OxDirectionHelper.IsHorizontal(direction))
                Cursor = Cursors.SizeWE;
            else
            if (OxDirectionHelper.IsVertical(direction))
                Cursor = Cursors.SizeNS;
            else
            if (OxDirectionHelper.IsLeftTop(direction)
                || OxDirectionHelper.IsRightBottom(direction))
                Cursor = Cursors.SizeNWSE;
            else
            if (OxDirectionHelper.IsRightTop(direction)
                || OxDirectionHelper.IsLeftBottom(direction))
                Cursor = Cursors.SizeNESW;
        }

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