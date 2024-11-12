namespace OxLibrary.Controls
{
    public delegate Bitmap? GetItemPicture<T>(T item);
    public class OxPicturedComboBox<T> : OxComboBox<T>
    {
        public OxPicturedComboBox() { }

        private static readonly int ImageSize = 24;
        private static readonly int ImageSpace = 2;
        private static readonly int ImageLeft = 2;

        protected override Point GetTextStartPosition(Rectangle bounds) => 
            new(bounds.X + ImageLeft + ImageSize + ImageSpace, bounds.Y);

        public event GetItemPicture<T>? GetItemPicture;

        protected virtual Bitmap? OnGetPicture(T item) => 
            GetItemPicture?.Invoke(item);

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index > -1)
            {
                Bitmap? picture = OnGetPicture(TItems[e.Index]);

                if (picture != null)
                    e.Graphics.DrawImage(
                        OxImageBoxer.BoxingImage(picture, new(e.Bounds.Height, e.Bounds.Height)),
                        e.Bounds.X + ImageLeft,
                        e.Bounds.Y
                    );
            }
        }
    }
}