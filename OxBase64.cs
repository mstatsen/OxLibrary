using System.Drawing.Imaging;

namespace OxLibrary
{
    public static class OxBase64
    {
        private static MemoryStream NewMemoryStream() =>
            new()
            {
                Position = 0
            };

        private static Size BitmapBoxSizeForBase64 = new(220, 107);

        public static string BitmapToBase64(Bitmap? bitmap)
        {
            if (bitmap == null)
                return string.Empty;

            MemoryStream memoryStream = NewMemoryStream();
            bitmap = OxBitmapCalcer.Zip(bitmap, BitmapBoxSizeForBase64);
            bitmap.Save(memoryStream, ImageFormat.Png);
            byte[] imageBytes = memoryStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Bitmap? Base64ToBitmap(string base64String)
        {
            if (base64String == string.Empty)
                return null;

            MemoryStream memoryStream = NewMemoryStream();

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                memoryStream.Write(imageBytes, 0, imageBytes.Length);
                return (Bitmap)Image.FromStream(memoryStream, false);
            }
            finally
            {
                memoryStream.Dispose();
            }
        }
    }
}