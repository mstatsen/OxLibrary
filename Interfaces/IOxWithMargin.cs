namespace OxLibrary.Interfaces
{
    public interface IOxWithMargin
    {
        OxBorders Margin { get; }
        OxBool BlurredBorder { get; set; }
        bool IsBlurredBorder { get; }
        void SetBlurredBorder(bool value);
    }
}