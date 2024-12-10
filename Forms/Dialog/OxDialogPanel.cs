using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Panels;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public class OxDialogPanel : OxFormPanel
{
    public short DialogButtonSpace = 6;
    public short DialogButtonStartSpace = 10;
    protected readonly Dictionary<OxDialogButton, OxButton> buttonsDictionary = new();
    protected virtual HorizontalAlign FooterButtonsAlign => HorizontalAlign.Right;

    public void SetFooterButtonText(OxDialogButton dialogButton, string text)
    {
        if (buttonsDictionary.TryGetValue(dialogButton, out var button))
            button.Text = text;
    }

    private OxDialogButton dialogButtons = OxDialogButton.OK | OxDialogButton.Cancel;

    public OxDialogButton DialogButtons
    {
        get => dialogButtons;
        set
        {
            dialogButtons = value;

            foreach (var item in buttonsDictionary)
                item.Value.Visible = (dialogButtons & item.Key).Equals(item.Key);

            PlaceButtons();
        }
    }

    public OxDialogPanel() : base() =>
        Size = new(480, 360);

    public readonly OxFrame Footer = new();

    protected override void PrepareInnerComponents()
    {
        PrepareFooter();
        base.PrepareInnerComponents();
    }

    private void CreateButton(OxDialogButton dialogButton)
    {
        OxButton button = new(
            OxDialogButtonHelper.Name(dialogButton),
            OxDialogButtonHelper.Icon(dialogButton)
        )
        {
            Parent = Footer,
            Top = FooterButtonVerticalMargin,
            Font = OxStyles.Font(OxStyles.DefaultFontSize + 0.5f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Visible = (dialogButtons & dialogButton).Equals(dialogButton),
            Size = new(
                OxDialogButtonHelper.Width(dialogButton),
                OxSH.Sub(
                    FooterButtonHeight, 
                    OxSH.X2(FooterButtonVerticalMargin)
                )
            )
        };
        button.Click += DialogButtonClickHandler;
        buttonsDictionary.Add(dialogButton, button);
    }

    private void PrepareFooter()
    {
        Footer.Dock = OxDock.Bottom;
        Footer.Parent = this;
        Footer.BlurredBorder = true;

        foreach (OxDialogButton button in OxDialogButtonHelper.List())
            CreateButton(button);

        PlaceButtons();
        Footer.Size = new(Footer.Width, FooterButtonHeight);
        Footer.Borders[OxDock.Left].Visible = false;
        Footer.Borders[OxDock.Right].Visible = false;
        Footer.Borders[OxDock.Bottom].Visible = false;
    }

    protected virtual short FooterButtonHeight => 36;

    protected virtual short FooterButtonVerticalMargin => 4;

    private void DialogButtonClickHandler(object? sender, EventArgs e)
    {
        if (sender is null)
            return;

        OxButton button = (OxButton)sender;
        OxDialogButton dialogButton = OxDialogButton.OK;

        foreach (var item in buttonsDictionary)
            if (item.Value.Equals(button))
                dialogButton = item.Key;

        if (Form is not null)
            Form.DialogResult = OxDialogButtonHelper.Result(dialogButton);
    }

    protected virtual void PlaceButtons()
    {
        Dictionary<OxDialogButton, OxButton> realButtons = new();

        short fullButtonsWidth = 0;

        foreach (var item in buttonsDictionary)
            if ((dialogButtons & item.Key).Equals(item.Key))
            {
                realButtons.Add(item.Key, item.Value);
                fullButtonsWidth += OxDialogButtonHelper.Width(item.Key);
                fullButtonsWidth += DialogButtonSpace;
            }

        fullButtonsWidth -= DialogButtonSpace;

        short rightOffset =
            FooterButtonsAlign switch
            {
                HorizontalAlign.Left =>
                    fullButtonsWidth,
                HorizontalAlign.Center =>
                        OxSH.Sub(
                            Footer.Width,
                            OxSH.CenterOffset(Footer.Width, fullButtonsWidth)
                        ),
                _ =>
                    OxSH.Sub(Footer.Width, DialogButtonStartSpace)
            };

        int buttonIndex = 0;

        foreach (var item in realButtons)
        {
            item.Value.Left = OxSH.Sub(rightOffset, OxDialogButtonHelper.Width(item.Key));
            item.Value.Size = new(
                OxDialogButtonHelper.Width(item.Key),
                OxSH.Sub(
                    FooterButtonHeight, 
                    OxSH.X2(FooterButtonVerticalMargin)
                )
            );
            rightOffset = OxSH.Sub(item.Value.Left, DialogButtonSpace);
            buttonIndex++;
        }
    }

    public override void PrepareColors()
    {
        base.PrepareColors();
        Footer.BaseColor = BaseColor;

        foreach (var item in buttonsDictionary)
            item.Value.BaseColor = BaseColor;
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        if (!e.Changed)
            return;

        Form?.ClearConstraints();
        base.OnSizeChanged(e);
        PlaceButtons();
        Form?.FreezeSize();
    }

    /*
    public new void Size = new(int width, int height)
    {
        base.Size = new(width, height);
        Form.ClientSize = new(Width, Height);
    };
    */
}