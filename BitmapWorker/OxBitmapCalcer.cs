using System.Drawing.Drawing2D;

namespace OxLibrary.BitmapWorker
{
    public class OxBitmapCalcer
    {
        private readonly bool Stretch;
        private readonly Image Image;
        public OxRectangle ImageBox = new();
        public OxSize ImageSize = new();

        public PictureBoxSizeMode SizeMode = PictureBoxSizeMode.CenterImage;

        private void CalcParams()
        {
            ImageSize.Width = (short)Image.Width;
            ImageSize.Height = (short)Image.Height;

            if (Stretch && NeedZoom())
                CalcForZoom();
            else
                CalcForCenter();

            AlignByCenter();
        }

        public OxBitmapCalcer(Image image, OxSize boxSize, bool stretch)
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

            if (ImageSize.Width > ImageBox.Width
                && ImageBox.Width > 0)
                ImageSize.Width = (short)ImageBox.Width;
            else ImageBox.Width = ImageSize.Width;

            if (ImageSize.Height > ImageBox.Height
                && ImageBox.Height > 0)
                ImageSize.Height = (short)ImageBox.Height;
            else ImageBox.Height = ImageSize.Height;
        }

        private static double GetZoom(short imageSize, short imageBox) =>
            imageSize > imageBox
                ? (double)imageSize / imageBox
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
                ImageSize.Width = (short)(ImageSize.Width / zoom);
                ImageSize.Height = (short)(ImageSize.Height / zoom);
            }
        }

        private void AlignByCenter()
        {
            ImageBox.X = (short)((ImageBox.Width - ImageSize.Width) / 2);
            ImageBox.Y = (short)((ImageBox.Height - ImageSize.Height) / 2);
        }

        private Bitmap GetBitmap(OxSize imageSize, OxRectangle coordinates)
        {
            Bitmap resultBitmap = new(
                imageSize.Width,
                imageSize.Height
            );
            Graphics g = Graphics.FromImage(resultBitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle rect = coordinates.Rectangle;
            g.DrawImage(Image,
                rect.Left,
                rect.Top,
                rect.Width,
                rect.Height);
            g.Dispose();

            return resultBitmap;
        }

        public Bitmap CroppedBitmap => GetBitmap(
            new(ImageSize.Width, ImageSize.Height),
            new(0,
                0,
                Math.Min(ImageBox.Width, ImageSize.Width),
                Math.Min(ImageBox.Height, ImageSize.Height)));

        public Bitmap FullBitmap => GetBitmap(
            new(ImageBox.Width, ImageBox.Height),
            new(ImageBox.X, ImageBox.Y, ImageSize.Width, ImageSize.Height));

        public static Bitmap Zip(Bitmap bitmap, OxSize maximumSize) =>
            new OxBitmapCalcer(bitmap, maximumSize, true).CroppedBitmap;
    }
}