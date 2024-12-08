using OxLibrary.Controls;
using OxLibrary.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Forms;

public partial class OxFormPanel<TForm, TFormPanel> :
    OxFrameWithHeader,
    IOxFormPanel<TForm, TFormPanel>
    where TForm : IOxForm<TForm, TFormPanel>
    where TFormPanel : IOxFormPanel<TForm, TFormPanel>, new()
{
    private TForm? form;
    public TForm? Form 
    { 
        get => form;
        set
        { 
            form = value;

            if (form is not null)
            {
                form.SizeChanged += FormSizeChanged;
                formMover = new(form, Header.Label);
            }
        }
    }

    private OxBoxMover? formMover;

    
    public OxFormPanel() : base()
    {
        Dock = OxDock.Fill;
        SetHeaderHeight(OxWh.W34);
        SetRestoreButtonIconAndTooltip();
        SetBordersSize();
        SetHeaderFont();
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

    public void SetHeaderHeight(OxWidth height)
    {
        HeaderHeight = height;
        SetButtonsSize();
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        SetButtonsSize();
        PlaceButtons();
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        Form?.PrepareColors();
    }

    private void PlaceButtons()
    {
        Header.SquareToolBarButtons = false;
        Header.AddToolButton(minimizeButton);
        Header.AddToolButton(restoreButton);
        Header.AddToolButton(closeButton);
    }

    private void SetButtonsSize()
    {
        foreach (OxIconButton button in Header.Buttons)
        {
            button.Parent = null;
            button.Size = new(OxWh.W44, OxWh.W28);
        }

        Header.ToolBar.Buttons.Clear();
        PlaceButtons();
    }

    private readonly OxIconButton closeButton = new(OxIcons.Close, OxWh.W28)
    {
        ToolTipText = "Close",
        HoveredColor = Color.Red,
        Name = "FormCloseButton"
    };
    private readonly OxIconButton restoreButton = new(OxIcons.Restore, OxWh.W28)
    {
        ToolTipText = "Restore window",
        Default = true,
        Name = "FormRestoreButton"
    };
    private readonly OxIconButton minimizeButton = new(OxIcons.Minimize, OxWh.W28)
    {
        ToolTipText = "Minimize window",
        Name = "FormMinimizeButton"
    };

    public OxIconButton CloseButton => closeButton;
    public OxIconButton RestoreButton => restoreButton;
    public OxIconButton MinimizeButton => minimizeButton;

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

    public void SetFormState(FormWindowState state) => 
        Form?.SetState(state);

    public override Color DefaultColor =>
        Color.FromArgb(146, 143, 140);

    public bool FormIsMaximized => 
        Form?.WindowState is FormWindowState.Maximized;

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.Changed
            || !Initialized)
            return;

        DoWithSuspendedLayout(() =>
            {
                if (e.Changed &&
                    Form is not null)
                    Form.Size = new(Size);
            }
        );
    }

    public override void OnLocationChanged(OxLocationChangedEventArgs e)
    {
        base.OnLocationChanged(e);

        if (!e.Changed
            || Form is null
            || formMover is not null
                && formMover.Processing)
            return;

        if (e.XChanged)
        {
            if (OxWh.Greater(e.NewValue.X, e.OldValue.X))
                Form.Left = OxWh.A(Form.Left, Left);
            else Form.Left = OxWh.S(Form.Left, Left);
        }

        if (e.YChanged)
        {
            if (OxWh.Greater(e.NewValue.Y, e.OldValue.Y))
                Form.Left = OxWh.A(Form.Top, Top);
            else Form.Left = OxWh.S(Form.Top, Top);
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
        // TODO: change logic from OxPanel? border to OxBorder border
        if (Form is null
            || !Form.Sizable
            || ResizeProcessing)
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

        List<OxPoint> sizePoints = OxBoxMover.WayPoints(oldSize, newSize, 30);

        ResizeProcessing = true;

        try
        {
            Form.DoWithSuspendedLayout(
                () =>
                {
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
                }
            );
        }
        finally
        { 
            ResizeProcessing = false;
        }
    }
}

public class OxFormPanel : OxFormPanel<OxForm, OxFormPanel>
{ 
}