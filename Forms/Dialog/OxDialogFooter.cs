using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Panels;
using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Forms;

public delegate void OnDialogResult(DialogResult dialogResult);

public class OxDialogFooter : OxOneBorderPanel
{
    public readonly short DialogButtonSpace = 6;
    public readonly short DialogButtonStartSpace = 10;

    private HorizontalAlign buttonsAlign = HorizontalAlign.Right;
    private short buttonHeight = 36;
    private short buttonVerticalMargin = 4;

    public OnDialogResult? SetDialogResult;
    public event EventHandler? ButtonsPlacing;

    public OxDialogFooter() : base()
    {
        BlurredBorder = true;

        foreach (OxDialogButton button in OxDialogButtonHelper.List())
            CreateButton(button);

        PlaceButtons();
        Size = new(Width, ButtonHeight);
    }

    public override void PrepareColors()
    {
        base.PrepareColors();

        //TODO: try drop this loop - it alredy exists in OxPanel.PrepareColors
        foreach (var item in buttonsDictionary)
            item.Value.BaseColor = BaseColor;
    }

    public string FooterButtonText(OxDialogButton dialogButton) =>
        buttonsDictionary.TryGetValue(dialogButton, out var button)
            ? button.Text
            : string.Empty;

    public void SetFooterButtonText(OxDialogButton dialogButton, string text)
    {
        if (buttonsDictionary.TryGetValue(dialogButton, out var button))
            button.Text = text;
    }

    public override void OnDockChanged(OxDockChangedEventArgs e)
    {
        base.OnDockChanged(e);

        if (e.Changed)
            PlaceButtons();
    }

    public override void OnSizeChanged(OxSizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        if (e.Changed)
            PlaceButtons();
    }

    public short ButtonVerticalMargin
    {
        get => buttonVerticalMargin;
        set
        {
            if (buttonVerticalMargin.Equals(value))
                return;

            buttonVerticalMargin = value;
            PlaceButtons();
        }
    }
    public short ButtonHeight {
        get => buttonHeight;
        set
        {
            if (buttonHeight.Equals(value))
                return;

            buttonHeight = value;
            PlaceButtons();
        }
    }

    public HorizontalAlign ButtonsAlign
    {
        get => buttonsAlign;
        set
        {
            if (buttonsAlign.Equals(value))
                return;

            buttonsAlign = value;
            PlaceButtons();
        }
    }

    public bool ButtonVisible(OxDialogButton button) =>
        (DialogButtons & button).Equals(button);

    public void SetButtonEnabled(OxDialogButton dialogButton, bool enabled)
    {
        if (buttonsDictionary.TryGetValue(dialogButton, out var button))
            button.Enabled = enabled;
    }

    public short ButtonsWidth
    {
        get
        {
            short calcedWidth = 0;

            foreach (OxDialogButton button in buttonsDictionary.Keys)
                if (ButtonVisible(button))
                    calcedWidth += OxSH.Add(
                        OxDialogButtonHelper.Width(button), 
                        DialogButtonSpace
                    );

            return calcedWidth;
        }
    }

    private OxDialogButton dialogButtons = OxDialogButton.OK | OxDialogButton.Cancel;
    private readonly Dictionary<OxDialogButton, OxButton> buttonsDictionary = new();

    public OxDialogButton DialogButtons
    {
        get => dialogButtons;
        set
        {
            if (dialogButtons.Equals(value))
                return;

            dialogButtons = value;

            foreach (var item in buttonsDictionary)
                item.Value.Visible = (dialogButtons & item.Key).Equals(item.Key);

            PlaceButtons();
        }
    }

    private void CreateButton(OxDialogButton dialogButton)
    {
        OxButton button = new(
            OxDialogButtonHelper.Name(dialogButton),
            OxDialogButtonHelper.Icon(dialogButton)
        )
        {
            Parent = this,
            Top = buttonVerticalMargin,
            Font = OxStyles.Font(OxStyles.DefaultFontSize + 0.5f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Visible = ButtonVisible(dialogButton),
            Size = new(
                OxDialogButtonHelper.Width(dialogButton),
                OxSH.Sub(
                    ButtonHeight,
                    OxSH.X2(buttonVerticalMargin)
                )
            )
        };
        button.Click += ButtonClickHandler;
        buttonsDictionary.Add(dialogButton, button);
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
            buttonsAlign switch
            {
                HorizontalAlign.Left =>
                    fullButtonsWidth,
                HorizontalAlign.Center =>
                        OxSH.Sub(
                            Width,
                            OxSH.CenterOffset(Width, fullButtonsWidth)
                        ),
                _ =>
                    OxSH.Sub(Width, DialogButtonStartSpace)
            };

        int buttonIndex = 0;

        foreach (var item in realButtons)
        {
            item.Value.Left = OxSH.Sub(rightOffset, OxDialogButtonHelper.Width(item.Key));
            item.Value.Size = new(
                OxDialogButtonHelper.Width(item.Key),
                OxSH.Sub(
                    ButtonHeight,
                    OxSH.X2(buttonVerticalMargin)
                )
            );
            rightOffset = OxSH.Sub(item.Value.Left, DialogButtonSpace);
            buttonIndex++;
        }

        OnButtonPlacing();
    }

    private void OnButtonPlacing()
    {
        ButtonsPlacing?.Invoke(this, EventArgs.Empty);
    }

    private void ButtonClickHandler(object? sender, EventArgs e)
    {
        if (sender is null)
            return;

        OxButton button = (OxButton)sender;
        OxDialogButton dialogButton = OxDialogButton.OK;

        foreach (var item in buttonsDictionary)
            if (item.Value.Equals(button))
                dialogButton = item.Key;

        SetDialogResult?.Invoke(OxDialogButtonHelper.Result(dialogButton));
    }
}
