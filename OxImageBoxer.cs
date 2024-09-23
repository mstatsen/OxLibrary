namespace OxLibrary
{
    public static class OxImageBoxer
    {
        public static Bitmap BoxingImage(Bitmap? image, Size boxSize) =>
            image == null
                ? OxIcons.Close
                : OxBitmapCalcer.Zip(image, boxSize);
    }
}