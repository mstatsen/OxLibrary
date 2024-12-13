using OxLibrary.Controls;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using OxLibrary.ControlList;

namespace OxLibrary.Panels;

public class OxHeader : OxOneBorderPanel, IOxHeader
{
    private readonly OxPicture icon = new()
    {
        Dock = OxDock.Left,
        Visible = OxB.F
    };

    private readonly OxLabel title = new()
    {
        AutoSize = OxB.F,
        Dock = OxDock.Fill,
        Text = string.Empty,
        Font = new(
            OxStyles.FontFamily,
            OxStyles.DefaultFontSize + 0.5f, 
            FontStyle.Bold | FontStyle.Italic
        ),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private readonly OxHeaderToolBar toolBar = new()
    {
        Dock = OxDock.Right,
        Width = 1
    };

    public OxHeaderToolBar ToolBar => toolBar;

    public OxClickFrameList<OxIconButton> Buttons
    {
        get => ToolBar.Buttons;
        set => ToolBar.Buttons = value;
    }

    public OxHeader(string title = "") : base(26)
    {
        Dock = OxDock.Top;
        this.title.Text = title;
        this.title.DoubleClick += TitleDoubleClickHandler;
        this.title.Click += LabelClickHandler;
    }

    private void TitleDoubleClickHandler(object? sender, EventArgs e) =>
        ToolBar.ExecuteDefault();

    public override OxDock Dock 
    { 
        get => base.Dock; 
        set => base.Dock = OxDock.Top; 
    }

    private void LabelClickHandler(object? sender, EventArgs e) => 
        Click?.Invoke(sender, e);

    public new EventHandler? Click { get; set; }

    public OxLabel Title => title;

    protected override string GetText() =>
        title.Text;

    protected override void SetText(string? value)
    {
        title.Text = 
            value is not null 
                ? value.Trim() 
                : string.Empty;
        title.SetVisible(!title.Text.Equals(string.Empty));
    }

    public ContentAlignment TitleAlign
    {
        get => title.TextAlign;
        set => title.TextAlign = value;
    }

    public Font TitleFont 
    { 
        get => title.Font; 
        set => title.Font = value;
    }

    protected override Bitmap? GetIcon() => 
        (Bitmap?)icon.Image;

    protected override void SetIcon(Bitmap? value)
    {
        icon.Image = value;
        icon.SetVisible(icon.Image is not null);
    }

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        icon.Parent = this;
        ToolBar.Parent = this;
        title.Parent = this;
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        title.ForeColor = Colors.Darker(6);
        ToolBar.BaseColor = BaseColor;
        icon.BaseColor = BaseColor;
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (!e.IsChanged)
            return;

        icon.Width = icon.Height;
        ToolBar.PlaceButtons();
    }

    public void AddButton(OxIconButton button, bool startGroup = false) =>
        ToolBar.AddButton(button, startGroup);

    public bool SquareToolBarButtons
    {
        get => ToolBar.SquareButtons;
        set => ToolBar.SquareButtons = value;
    }
}