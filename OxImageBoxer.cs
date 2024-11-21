namespace OxLibrary
{
    public static class OxImageBoxer
    {
        public static Bitmap BoxingImage(Bitmap? image, OxSize boxSize) =>
            OxBitmapCalcer.Zip(image ?? OxIcons.Close, boxSize);
    }
}