namespace OxLibrary
{
    public static class OxImageBoxer
    {
        public static Bitmap BoxingImage(Bitmap? image, Size boxSize) =>
            OxBitmapCalcer.Zip(image ?? OxIcons.Close, boxSize);
    }
}