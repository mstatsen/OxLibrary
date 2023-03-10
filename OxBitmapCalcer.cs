using System.Drawing.Drawing2D;

namespace OxLibrary
{
    public class OxBitmapCalcer
    {
        private readonly bool Stretch;
        private readonly Image Image;
        public Rectangle ImageBox = new();
        public Size ImageSize = new();

        public PictureBoxSizeMode SizeMode = PictureBoxSizeMode.CenterImage;

        private void CalcParams()
        {
            ImageSize.Width = Image.Width;
            ImageSize.Height = Image.Height;

            if (Stretch && NeedZoom())
                CalcForZoom();
            else
                CalcForCenter();

            AlignByCenter();
        }

        public OxBitmapCalcer(Image image, Size boxSize, bool stretch)
        {
            Image = image;
            Stretch = stretch;
            ImageBox.Width = boxSize.Width;
            ImageBox.Height = boxSize.Height;
            CalcParams();
        }

        private bool NeedZoom() => 
            ImageSize.Width >= ImageBox.Width
            || ImageSize.Height >= ImageBox.Height;

        private void CalcForCenter()
        {
            SizeMode = PictureBoxSizeMode.CenterImage;

            if (ImageSize.Width > ImageBox.Width && ImageBox.Width != 0)
                ImageSize.Width = ImageBox.Width;
            else ImageBox.Height = ImageSize.Width;

            if (ImageSize.Height > ImageBox.Height && ImageBox.Height != 0)
                ImageSize.Height = ImageBox.Height;
            else ImageBox.Height = ImageSize.Height;
        }

        private static double GetZoom(int imageSize, int imageBox) => 
            imageSize > imageBox 
                ? ((double)imageSize / imageBox) 
                : 1;

        private void CalcForZoom()
        {
            SizeMode = PictureBoxSizeMode.Zoom;

            double zoom = Math.Max(
                GetZoom(ImageSize.Width, ImageBox.Width),
                GetZoom(ImageSize.Height, ImageBox.Height)
            );

            if (zoom > 1)
            {
                ImageSize.Width = (int)(ImageSize.Width / zoom);
                ImageSize.Height = (int)(ImageSize.Height / zoom);
            }
        }

        private void AlignByCenter()
        {
            ImageBox.X = (ImageBox.Width - ImageSize.Width) / 2;
            ImageBox.Y = (ImageBox.Height - ImageSize.Height) / 2;
        }

        private Bitmap GetBitmap(Size imageSize, Rectangle coordinates)
        {
            Bitmap resultBitmap = new(imageSize.Width, imageSize.Height);
            Graphics g = Graphics.FromImage(resultBitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(Image,
                coordinates.Left, 
                coordinates.Top,
                coordinates.Width, 
                coordinates.Height);
            g.Dispose();

            return resultBitmap;
        }

        public Bitmap CroppedBitmap => GetBitmap(
            new Size(ImageSize.Width, ImageSize.Height),
            new Rectangle(0, 0,
                Math.Min(ImageBox.Width, ImageSize.Width),
                Math.Min(ImageBox.Height, ImageSize.Height)));

        public Bitmap FullBitmap => GetBitmap(
            new Size(ImageBox.Width, ImageBox.Height),
            new Rectangle(ImageBox.Left, ImageBox.Top, ImageSize.Width, ImageSize.Height));

        public static Bitmap Zip(Bitmap bitmap, Size maximumSize) => 
            new OxBitmapCalcer(bitmap, maximumSize, true).CroppedBitmap;
    }
}