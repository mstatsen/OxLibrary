using OxLibrary.Controls;
using OxLibrary.Handlers;
using OxLibrary.Panels;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public class OxDialogPanel<TDialog, TDialogPanel> : OxFormPanel<TDialog,TDialogPanel>
    where TDialog : OxDialog<TDialog, TDialogPanel>
    where TDialogPanel: OxDialogPanel<TDialog, TDialogPanel>, new()
{
    public OxWidth DialogButtonSpace = OxWh.W6;
    public OxWidth DialogButtonStartSpace = OxWh.W10;
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
            OxDialogButtonsHelper.Name(dialogButton),
            OxDialogButtonsHelper.Icon(dialogButton)
        )
        {
            Parent = Footer,
            Top = FooterButtonVerticalMargin,
            Font = OxStyles.Font(OxStyles.DefaultFontSize + 0.5f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Visible = (dialogButtons & dialogButton).Equals(dialogButton),
            Size = new(
                OxDialogButtonsHelper.Width(dialogButton),
                OxWh.Sub(
                    FooterButtonHeight,
                    OxWh.Mul(FooterButtonVerticalMargin, 2)
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

        foreach (OxDialogButton button in OxDialogButtonsHelper.List())
            CreateButton(button);

        PlaceButtons();
        Footer.Size = new(Footer.Width, FooterButtonHeight);
        Footer.Borders[OxDock.Left].Visible = false;
        Footer.Borders[OxDock.Right].Visible = false;
        Footer.Borders[OxDock.Bottom].Visible = false;
    }

    protected virtual OxWidth FooterButtonHeight => OxWh.W36;

    protected virtual OxWidth FooterButtonVerticalMargin => OxWh.W4;

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
            Form.DialogResult = OxDialogButtonsHelper.Result(dialogButton);
    }

    protected virtual void PlaceButtons()
    {
        Dictionary<OxDialogButton, OxButton> realButtons = new();

        OxWidth fullButtonsWidth = OxWh.W0;

        foreach (var item in buttonsDictionary)
            if ((dialogButtons & item.Key).Equals(item.Key))
            {
                realButtons.Add(item.Key, item.Value);
                fullButtonsWidth |= OxDialogButtonsHelper.Width(item.Key) | DialogButtonSpace;
            }

        fullButtonsWidth = OxWh.Sub(fullButtonsWidth, DialogButtonSpace);

        OxWidth rightOffset =
            FooterButtonsAlign switch
            {
                HorizontalAlign.Left =>
                    fullButtonsWidth,
                HorizontalAlign.Center =>
                    OxWh.Sub(
                        Footer.Width,
                        OxWh.Div(
                            OxWh.Sub(Footer.Width, fullButtonsWidth),
                            OxWh.W2
                        )
                    ),
                _ =>
                    OxWh.Sub(Footer.Width, DialogButtonStartSpace)
            };

        int buttonIndex = 0;

        foreach (var item in realButtons)
        {
            item.Value.Left = rightOffset - OxDialogButtonsHelper.Z_Width(item.Key);
            item.Value.Size = new(
                OxDialogButtonsHelper.Width(item.Key),
                OxWh.Sub(
                    FooterButtonHeight,
                    OxWh.Mul(FooterButtonVerticalMargin, 2)
                )
            );
            rightOffset = OxWh.Sub(item.Value.Left, DialogButtonSpace);
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

public class OxDialogPanel : OxDialogPanel<OxDialog, OxDialogPanel>
{ 
}