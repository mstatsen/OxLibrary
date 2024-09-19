using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxIconButton : OxClickFrame
    {
        public readonly OxPicture Picture = new()
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
            Picture.Parent = ContentContainer;
            Picture.UseDisabledStyles = UseDisabledStyles;
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            Picture.BaseColor = ContentContainer.BaseColor;
        }

        public Bitmap? Icon
        {
            get => (Bitmap?)Picture.Image;
            set => SetIcon(value);
        }

        protected virtual void SetIcon(Bitmap? value) => 
            Picture.Image = value;

        public int IconPadding
        {
            get => Picture.PicturePadding;
            set => Picture.PicturePadding = value;
        }

        protected override void SetUseDisabledStyles(bool value)
        {
            base.SetUseDisabledStyles(value);
            Picture.UseDisabledStyles = UseDisabledStyles;
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            SetHoverHandlers(Picture);
            SetClickHandler(Picture);
        }

        protected override void SetEnabled(bool value)
        {
            base.SetEnabled(value);
            Picture.Enabled = value;
            PrepareColors();
        }

        protected override void SetToolTipText(string value)
        {
            base.SetToolTipText(value);
            ToolTip.SetToolTip(Picture, value);
            ToolTip.SetToolTip(Picture.Picture, value);
        }
    }
}