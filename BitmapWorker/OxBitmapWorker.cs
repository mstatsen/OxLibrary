namespace OxLibrary.BitmapWorker
{
    public static class OxBitmapWorker
    {
        public static Bitmap BoxingImage(Bitmap? image, OxSize boxSize) =>
            OxBitmapCalcer.Zip(image ?? OxIcons.Close, boxSize);
    }
}