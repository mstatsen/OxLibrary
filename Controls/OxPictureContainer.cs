using OxLibrary.Panels;

namespace OxLibrary.Controls
{
    public class OxPictureContainer : OxClickFrame
    {
        private readonly OxLabel label = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Click here to load image",
            ForeColor = Color.Silver,
            Cursor = Cursors.Hand,
            AutoSize = false
        };

        private readonly OxPicture picture = new()
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Stretch = true,
            PicturePadding = 0,
            Cursor = Cursors.Hand
        };
        private Image? sourceImage;

        public OxPictureContainer() { }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            label.Parent = ContentContainer;
            label.Font = new Font(Font.FontFamily, Font.Size + 1, FontStyle.Italic);
            picture.Parent = this;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = false;
        }

        private const string PictureFilesFilter =
            "Pictures (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.gif, *.bmp;) " +
                "| " +
                "*.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.gif; *.bmp;";

        private static string SelectPictureFile()
        {
            OpenFileDialog dialog = new()
            {
                Filter = PictureFilesFilter
            };

            return dialog.ShowDialog() == DialogResult.OK 
                ? dialog.FileName 
                : string.Empty;
        }

        private void ClickHandler(object? sender, EventArgs e)
        {
            string fileName = SelectPictureFile();

            if (fileName != string.Empty)
                Image = new Bitmap(fileName);
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            Click += ClickHandler;
            SetClickHandler(label);
            SetClickHandler(picture);
            SetHoverHandlers(label);
            SetHoverHandlers(picture);
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = hovered ? Color.Gray : Color.Silver;
            picture.BaseColor = BaseColor;
        }

        public Image? Image
        {
            get => sourceImage;
            set
            {
                sourceImage = value;
                label.Visible = sourceImage == null;
                picture.Visible = sourceImage != null;
                picture.Image = sourceImage;
            }
        }
    }
}