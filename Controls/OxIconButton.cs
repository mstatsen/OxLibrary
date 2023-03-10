using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxIconButton : OxClickFrame
    {
        protected readonly OxPicture picture = new()
        {
            Dock = DockStyle.Fill
        };

        public OxIconButton(Bitmap? icon, int Size) : base(new Size(Size, Size))
        {
            MinimumSize = new Size(Width, Height);
            MinimumSize = MaximumSize;
            Icon = icon;
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            picture.Parent = ContentContainer;
            picture.UseDisabledStyles = UseDisabledStyles;
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            picture.BaseColor = ContentContainer.BaseColor;
        }

        public Bitmap? Icon
        {
            get => (Bitmap?)picture.Image;
            set => SetIcon(value);
        }

        protected virtual void SetIcon(Bitmap? value) => 
            picture.Image = value;

        public int IconPadding
        {
            get => picture.PicturePadding;
            set => picture.PicturePadding = value;
        }

        protected override void SetUseDisabledStyles(bool value)
        {
            base.SetUseDisabledStyles(value);
            picture.UseDisabledStyles = UseDisabledStyles;
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            SetHoverHandlers(picture);
            SetClickHandler(picture);
        }

        protected override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            picture.Enabled = value;
            PrepareColors();
        }
    }
}