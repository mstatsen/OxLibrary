using OxLibrary.Controls;
using OxLibrary.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Forms
{
    public partial class OxFormMainPanel : OxFrameWithHeader
    {
        public OxForm Form { get; internal set; }

        private OxFormMover formMover = default!;

        public OxFormMainPanel(OxForm form) : base()
        {
            Form = form;
            Form.SizeChanged += FormSizeChanged;
            Dock = OxDock.Fill;
            SetTitleButtonsVisible();
            SetHeaderHeight(OxWh.W34);
            SetRestoreButtonIconAndTooltip();
            SetBordersSize();
            SetHeaderFont();
            SetMarginsSize();
            CreateFormMover();
            BlurredBorder = true;
        }

        public override OxDock Dock 
        { 
            get => OxDock.Fill; 
            set => base.Dock = OxDock.Fill;
        }

        private void FormSizeChanged(object sender, OxSizeChangedEventArgs args) => 
            SetRestoreButtonIconAndTooltip();

        private void SetHeaderFont() => 
            Header.TitleFont = 
                new(Header.TitleFont.FontFamily, Header.TitleFont.Size + 1, FontStyle.Bold);

        private void SetBordersSize() => 
            Borders.Size = OxWh.W1;

        private void CreateFormMover() => 
            formMover = new OxFormMover(Form, Header.Label);

        public void SetHeaderHeight(OxWidth height)
        {
            HeaderHeight = height;
            SetButtonsSize();
        }

        public void SetIcon()
        {
            Header.Icon = Form.FormIcon;

            if (Form.FormIcon is not null)
                Form.Icon = System.Drawing.Icon.FromHandle(Form.FormIcon.GetHicon());
        }

        internal void SetTitleButtonsVisible()
        {
            minimizeButton.Visible = Form.CanMinimize;
            restoreButton.Visible = Form.CanMaximize;
        }

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
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
                button.Size = new(OxWh.W36, OxWh.W28);
        }

        private void SetButtonsHandlers()
        {
            closeButton.Click += CloseButtonClickHandler;
            restoreButton.Click += RestoreButtonClickHandler;
            minimizeButton.Click += MinimizeButtonClickHandler;
        }

        private void MinimizeButtonClickHandler(object? sender, EventArgs e) => 
            SetFormState(FormWindowState.Minimized);

        private void RestoreButtonClickHandler(object? sender, EventArgs e) => 
            SetFormState(FormIsMaximized ? FormWindowState.Normal : FormWindowState.Maximized);

        private readonly OxIconButton closeButton = new(OxIcons.Close, OxWh.W28)
        {
            IconPadding = OxWh.W5,
            ToolTipText = "Close",
            HoveredColor = Color.Red,
            Name = "FormCloseButton"
        };
        private readonly OxIconButton restoreButton = new(OxIcons.Restore, OxWh.W28)
        {
            IconPadding = OxWh.W5,
            ToolTipText = "Restore window",
            Default = true,
            Name = "FormRestoreButton"
        };
        private readonly OxIconButton minimizeButton = new(OxIcons.Minimize, OxWh.W28)
        {
            IconPadding = OxWh.W5,
            ToolTipText = "Minimize window",
            Name = "FormMinimizeButton"
        };

        private Bitmap GetRestoreIcon() =>
            Form is not null 
            && FormIsMaximized
                ? OxIcons.Restore
                : OxIcons.Maximize;

        private string GetRestoreToopTip() =>
            Form is not null
            && FormIsMaximized
                ? "Restore window"
                : "Maximize window";

        private void SetRestoreButtonIconAndTooltip()
        {
            if (!Initialized)
                return;

            restoreButton.Icon = GetRestoreIcon();
            restoreButton.ToolTipText = GetRestoreToopTip();
        }

        public void SetFormState(FormWindowState state)
        {
            //ContentBox.Visible = false;

            try
            {
                Form.SetUpSizes(state);
            }
            finally
            {
                //ContentBox.Visible = true;
            }
        }

        private void CloseButtonClickHandler(object? sender, EventArgs e)
        {
            Form.DialogResult = DialogResult.Cancel;
            Form.Close();
        }

        public override Color DefaultColor =>
            Color.FromArgb(146, 143, 140);

        public bool FormIsMaximized => 
            Form.WindowState is FormWindowState.Maximized;

        public override bool OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!e.Changed
                || !Initialized)
                return false;

            DoWithSuspendedLayout(() =>
                {
                    if (e.Changed &&
                        Form is not null)
                        Form.Size = new(Size);
                }
            );

            return true;
        }

        internal void SetMarginsSize() => 
            Margin.Size = 
                Form.Sizeble 
                    ? OxWh.W2
                    : OxWh.W0;

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (formMover.Processing)
                return;

            Form.Left |= Left;
            Form.Top |= Top;
        }
                                             
        protected override void SetHandlers()
        {
            base.SetHandlers();
            MouseDown += ResizerMouseDown;
            MouseUp += MarginMouseUpHandler;
            MouseMove += ResizeHandler;
            MouseLeave += MarginMouseLeaveHandler;
        }

        private void MarginMouseLeaveHandler(object? sender, EventArgs e) =>
            Cursor = Cursors.Default;

        private void MarginMouseUpHandler(object? sender, MouseEventArgs e) =>
            LastDirection = OxDirection.None;

        private void ResizerMouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not OxPanel border)
                return;

            LastMousePosition = new(border.PointToScreen(e.Location));
            LastDirection = OxDirectionHelper.GetDirection(border, new(e.Location));
        }

        private void SetSizerCursor(OxDirection direction) => 
            Cursor = OxDirectionHelper.GetSizerCursor(direction);

        private OxPoint LastMousePosition = new(-1, -1);
        private OxDirection LastDirection = OxDirection.None;
        private bool ResizeProcessing = false;

        private void ResizeHandler(object? sender, MouseEventArgs e)
        {
            if (!Form.Sizeble)
                return;

            if (ResizeProcessing)
                return;

            OxPanel? border = (OxPanel?)sender;

            if (border is null)
                return;

            if (LastDirection.Equals(OxDirection.None))
            {
                SetSizerCursor(
                    OxDirectionHelper.GetDirection(border, new(e.Location))
                );
                return;
            }

            if (LastMousePosition.Equals(e.Location))
                return;

            OxPoint newLastMousePosition = new(border.PointToScreen(e.Location));
            OxPoint oldSize = new(Width, Height);
            OxPoint newSize = new(oldSize.X, oldSize.Y);
            OxPoint delta = new(
                newLastMousePosition.X - LastMousePosition.X,
                newLastMousePosition.Y - LastMousePosition.Y
            );

            if (OxDirectionHelper.ContainsRight(LastDirection))
                newSize.X |= delta.X;

            if (OxDirectionHelper.ContainsBottom(LastDirection))
                newSize.Y |= delta.Y;

            if (OxDirectionHelper.ContainsLeft(LastDirection))
                newSize.X -= delta.X;

            if (OxDirectionHelper.ContainsTop(LastDirection))
                newSize.Y -= delta.Y;

            LastMousePosition = newLastMousePosition;

            if (OxWh.LessOrEquals(newSize.X, Form.MinimumSize.Width))
                newSize.X = Form.MinimumSize.Width;

            if (OxWh.LessOrEquals(newSize.Y, Form.MinimumSize.Height))
                newSize.Y = Form.MinimumSize.Height;

            List<OxPoint> sizePoints = OxFormMover.WayPoints(oldSize, newSize, 30);

            ResizeProcessing = true;
            Form.SuspendLayout();
            SuspendLayout();

            foreach (OxPoint point in sizePoints)
            {
                OxPoint newLocationStep = new(Form.Left, Form.Top);

                if (OxDirectionHelper.ContainsLeft(LastDirection))
                    newLocationStep.X =
                        OxWh.Sub(
                            newLocationStep.X, 
                            OxWh.Sub(point.X, Width)
                        );

                if (OxDirectionHelper.ContainsTop(LastDirection))
                    newLocationStep.Y =
                        OxWh.Sub(
                            newLocationStep.Y,
                            OxWh.Sub(point.Y, Height)
                        );

                if (!Form.Location.Equals(newLocationStep))
                    Form.Location = newLocationStep;

                OxSize newSizeStep = new(point);

                if (!Size.Equals(newSizeStep))
                    Size = newSizeStep;
            }

            ResumeLayout();
            Form.ResumeLayout();
            ResizeProcessing = false;
        }
    }
}