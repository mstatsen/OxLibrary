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
            ImageSize.Width = OxWh.W(Image.Width);
            ImageSize.Height = OxWh.W(Image.Height);

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
                ImageSize.Width = ImageBox.Width;
            else ImageBox.Width = ImageSize.Width;

            if (ImageSize.Height > ImageBox.Height
                && ImageBox.Height > 0)
                ImageSize.Height = ImageBox.Height;
            else ImageBox.Height = ImageSize.Height;
        }

        private static double GetZoom(OxWidth imageSize, OxWidth imageBox) =>
            OxWh.Greater(imageSize, imageBox)
                ? (double)OxWh.Int(imageSize) / OxWh.Int(imageBox)
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
                ImageSize.Width = OxWh.W((int)(ImageSize.Z_Width / zoom));
                ImageSize.Height = OxWh.W((int)(ImageSize.Z_Height / zoom));
            }
        }

        private void AlignByCenter()
        {
            ImageBox.X = OxWh.Div(OxWh.Sub(ImageBox.Width, ImageSize.Width), OxWh.W2);
            ImageBox.Y = OxWh.Div(OxWh.Sub(ImageBox.Height, ImageSize.Height), OxWh.W2);
        }

        private Bitmap GetBitmap(OxSize imageSize, OxRectangle coordinates)
        {
            Bitmap resultBitmap = new(
                OxWh.Int(imageSize.Width),
                OxWh.Int(imageSize.Height)
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
            new(OxWh.W0,
                OxWh.W0,
                OxWh.Min(ImageBox.Width, ImageSize.Width),
                OxWh.Min(ImageBox.Height, ImageSize.Height)));

        public Bitmap FullBitmap => GetBitmap(
            new(ImageBox.Width, ImageBox.Height),
            new(ImageBox.X, ImageBox.Y, ImageSize.Width, ImageSize.Height));

        public static Bitmap Zip(Bitmap bitmap, OxSize maximumSize) =>
            new OxBitmapCalcer(bitmap, maximumSize, true).CroppedBitmap;
    }
}