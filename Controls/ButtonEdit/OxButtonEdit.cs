using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Panels;

namespace OxLibrary.Controls;

public class OxButtonEdit : OxPanel
{
    public readonly OxTextBox TextBox = new()
    {
        ReadOnly = true
    };

    public readonly OxIconButton Button = 
        new (OxIcons.Elipsis, 16)
        {
            Dock = OxDock.Right,
            Cursor = Cursors.Default
        };

    public OxButtonEdit() : base(new(120, 22)) { }

    protected override void PrepareInnerComponents() 
    {
        base.PrepareInnerComponents();
        PrepareTextBox();
        PrepareButton();
    }

    private void PrepareButton()
    {
        Button.Parent = this;
        Button.BaseColor = BaseColor;
        Button.Size = new(
            OxSH.Min(26, TextBox.Height - 4),
            TextBox.Height
        );
        Button.Borders.Left = 0;
        Button.FixedBorderColor = true;
    }

    public event EventHandler OnButtonClick
    {
        add => Button.Click += value;
        remove => Button.Click -= value;
    }

    public string? Value
    {
        get => TextBox.Text;
        set => TextBox.Text = value;
    }

    private void PrepareTextBox()
    {
        TextBox.Parent = this;
        TextBox.Dock = OxDock.Fill;
        TextBox.Multiline = true;
        TextBox.WordWrap = true;
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (e.Changed)
        {
            short calcedButtonWidth = OxSH.Min(22, TextBox.Height - 4);
            Button.Size = new(calcedButtonWidth, OxSH.Sub(Height, 2));
            Button.Width = calcedButtonWidth;
        }
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        TextBox.BackColor = Colors.Lighter(6);
        Button.BaseColor = Colors.Darker();

        if (TextBox.Focused)
            Button.BorderColor = Color.LightBlue;
    }

    public bool IsEmpty => 
        TextBox.Text.Trim().Equals(string.Empty);

    public bool ReadOnly
    {
        get => Button.Visible;
        set => Button.Visible = value;
    }
}
