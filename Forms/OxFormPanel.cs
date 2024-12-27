using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public partial class OxFormPanel :
    OxFrameWithHeader,
    IOxFormPanel
{
    private IOxForm? form;
    public IOxForm? Form 
    { 
        get => form;
        set
        {
            if (!OxHelper.Changed(form, value))
                return;

            if (form is not null)
                form.SizeChanged -= FormSizeChanged;

            form = value;

            if (form is not null)
            {
                form.SizeChanged += FormSizeChanged;
                formMover = new(form, Header.Title);
                form.Shown += FormShownHandler;
                Size = form.Size;
            }
        }
    }

    private void FormShownHandler(object? sender, EventArgs e) =>
        OnFormShown(e);

    protected virtual void OnFormShown(EventArgs e) { }

    private OxMover? formMover;
    
    public OxFormPanel() : base()
    {
        Dock = OxDock.Fill;
        ApplyRestoreButtonIconAndToolTip();
        SetBordersSize();
        SetHeaderFont();
        BlurredBorder = OxB.T;
    }

    public override OxDock Dock 
    { 
        get => OxDock.Fill; 
        set => base.Dock = OxDock.Fill;
    }

    private void FormSizeChanged(object sender, OxSizeChangedEventArgs args) =>
        ApplyRestoreButtonIconAndToolTip();

    private void SetHeaderFont() => 
        Header.TitleFont = 
            new(Header.TitleFont.FontFamily, Header.TitleFont.Size + 1, FontStyle.Bold);

    private void SetBordersSize() => 
        Borders.Size = 1;

    protected new virtual short HeaderHeight => OxSh.ToDPI(this, 34);

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        SetHeaderButtonsSize();
        PlaceHeaderButtons();
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        Form?.PrepareColors();
    }

    private void PlaceHeaderButtons()
    {
        Header.SquareToolBarButtons = false;
        Header.AddButton(minimizeButton);
        Header.AddButton(restoreButton);
        Header.AddButton(closeButton);
    }

    private void SetHeaderButtonsSize()
    {
        foreach (OxIconButton button in Header.Buttons)
        {
            button.Parent = null;
            button.Size = new(button.ToDPI(44), button.ToDPI(28));
        }

        Header.ToolBar.Buttons.Clear();
        PlaceHeaderButtons();
    }

    private readonly OxIconButton closeButton = new(OxIcons.Close, 28)
    {
        ToolTipText = "Close",
        HoveredColor = Color.Red,
        Name = "FormCloseButton"
    };
    private readonly OxIconButton restoreButton = new(OxIcons.Restore, 28)
    {
        ToolTipText = "Restore window",
        Default = true,
        Name = "FormRestoreButton"
    };
    private readonly OxIconButton minimizeButton = new(OxIcons.Minimize, 28)
    {
        ToolTipText = "Minimize window",
        Name = "FormMinimizeButton"
    };

    public OxIconButton CloseButton => closeButton;
    public OxIconButton RestoreButton => restoreButton;
    public OxIconButton MinimizeButton => minimizeButton;

    public void ApplyRestoreButtonIconAndToolTip()
    {
        if (Form is null)
        {
            if (RestoreButton.Icon is null 
                || !RestoreButton.Icon.Equals(OxIcons.Maximize))
                RestoreButton.Icon = OxIcons.Maximize;

            RestoreButton.ToolTipText = "Maximize window";
        }
        else
            Form.ApplyRestoreButtonIconAndToolTip();
    }

    public override Color DefaultColor =>
        Color.FromArgb(146, 143, 140);

    public bool FormIsMaximized => 
        Form?.WindowState is FormWindowState.Maximized;

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.IsChanged
            || !Initialized)
            return;

        WithSuspendedLayout(() =>
            {
                if (!base.HeaderHeight.Equals(HeaderHeight))
                {
                    base.HeaderHeight = HeaderHeight;
                    SetHeaderButtonsSize();
                }

                if (Form is not null)
                    Form.Size = new(Size);
            }
        );
    }

    public override void OnLocationChanged(OxLocationChangedEventArgs e)
    {
        base.OnLocationChanged(e);

        if (!e.IsChanged
            || Form is null
            || formMover is not null
                && formMover.Processing)
            return;

        if (e.IsXChanged)
        {
            if (e.NewValue.X > e.OldValue.X)
                Form.Left += Left;
            else Form.Left -= Left;
        }

        if (e.IsYChanged)
        {
            if (e.NewValue.Y > e.OldValue.Y)
                Form.Top += Top;
            else Form.Top -= Top;
        }
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
        LastMousePosition = new(PointToScreen(e.Location));
        LastDirection = OxDirectionHelper.GetDirection(this, new(e.Location));
    }

    private void SetSizerCursor(OxDirection direction) => 
        Cursor = OxDirectionHelper.GetSizerCursor(direction);

    private OxPoint LastMousePosition = new(-1, -1);
    private OxDirection LastDirection = OxDirection.None;
    private bool ResizeProcessing = false;

    private void ResizeHandler(object? sender, MouseEventArgs e)
    {
        if (Form is null
            || !Form.IsSizable
            || ResizeProcessing)
            return;

        if (LastDirection.Equals(OxDirection.None))
        {
            SetSizerCursor(
                OxDirectionHelper.GetDirection(this, new(e.Location))
            );
            return;
        }

        if (LastMousePosition.Equals(e.Location))
            return;

        OxPoint newLastMousePosition = new(PointToScreen(e.Location));
        OxPoint oldSize = new(Width, Height);
        OxPoint newSize = new(oldSize.X, oldSize.Y);
        OxPoint delta = new(
            OxSh.Sub(newLastMousePosition.X, LastMousePosition.X),
            OxSh.Sub(newLastMousePosition.Y, LastMousePosition.Y)
        );

        if (OxDirectionHelper.ContainsRight(LastDirection))
            newSize.X += delta.X;
        else
        if (OxDirectionHelper.ContainsLeft(LastDirection))
            newSize.X -= delta.X;

        if (OxDirectionHelper.ContainsBottom(LastDirection))
            newSize.Y += delta.Y;
        else
        if (OxDirectionHelper.ContainsTop(LastDirection))
            newSize.X -= delta.Y;

        LastMousePosition = newLastMousePosition;
        newSize.X = OxSh.Max(newSize.X, Form.MinimumSize.Width);
        newSize.Y = OxSh.Max(newSize.Y, Form.MinimumSize.Height);
        List<Point> sizePoints = OxMover.WayPoints(oldSize, newSize, 30);
        ResizeProcessing = true;

        try
        {
            Form.WithSuspendedLayout(
                () =>
                {
                    foreach (Point point in sizePoints)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        Point newLocationStep = new(Form.ZBounds.Left, Form.ZBounds.Top);
#pragma warning restore CS0618 // Type or member is obsolete

                        if (OxDirectionHelper.ContainsLeft(LastDirection))
                            newLocationStep.X -= point.X - Width;

                        if (OxDirectionHelper.ContainsTop(LastDirection))
                            newLocationStep.Y -= point.Y - Height;

                        if (!Form.Location.Equals(newLocationStep))
                            Form.Location = new(newLocationStep);

                        OxSize newSizeStep = new(point);

                        if (!Size.Equals(newSizeStep))
                            Size = newSizeStep;
                    }
                }
            );
        }
        finally
        { 
            ResizeProcessing = false;
        }
    }
}