using OxLibrary.Handlers;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public class OxDialogPanel : OxFormPanel
{
    protected HorizontalAlign FooterButtonsAlign
    {
        get => Footer.ButtonsAlign;
        set => Footer.ButtonsAlign = value;
    }

    public void SetFooterButtonText(OxDialogButton dialogButton, string text) =>
        Footer.SetFooterButtonText(dialogButton, text);

    public OxDialogButton DialogButtons
    {
        get => Footer.DialogButtons;
        set => Footer.DialogButtons = value;
    }

    public OxDialogPanel() : base() => 
        Size = new OxSize(480, 360).ToDPI(this);

    protected override void PrepareInnerComponents()
    {
        base.PrepareInnerComponents();
        PrepareFooter();
    }

    public readonly OxDialogFooter Footer = new();

    private void PrepareFooter()
    {
        Footer.Dock = OxDock.Bottom;
        Footer.Parent = this;
        Footer.SetDialogResult += SetDialogResultHandler;
    }

    public short FooterHeight
    { 
        get => Footer.Height; 
        set => Footer.Height = value;
    }

    protected virtual void SetDialogResultHandler(DialogResult dialogResult)
    {
        if (Form is not null)
            Form.DialogResult = dialogResult;
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        Footer.BaseColor = BaseColor;
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.IsChanged)
            return;

        Form?.ClearConstraints();
        base.OnSizeChanged(e);
        Form?.FreezeSize();
    }
}