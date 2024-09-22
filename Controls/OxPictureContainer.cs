using OxLibrary.Panels;
using System.Drawing.Imaging;

namespace OxLibrary.Controls
{
    public class OxPictureActionEventArgs : OxActionEventArgs<OxPictureAction>
    {
        public OxPictureActionEventArgs(OxPictureAction action) : base(action) { }
    }

    public class OxPictureContainer : OxClickFrame
    {
        private static readonly string SelectText = "Click here to select image";

        private readonly OxLabel label = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = SelectText,
            ForeColor = Color.Silver,
            Cursor = Cursors.Hand,
            AutoSize = false
        };

        private readonly OxClickFrameList Buttons = new();
        public readonly Dictionary<OxPictureAction, OxClickFrame> Actions = new();
        private readonly OxPane buttonsParent = new()
        { 
            Visible = false,
            BackColor = Color.Transparent,
            Dock = DockStyle.Bottom
        };

        private void PlaceButtons()
        {
            buttonsParent.SizeChanged -= ButtonsParentSizeChanged;

            try
            {
                if (Image == null)
                {
                    buttonsParent.Visible = false;
                    return;
                }

                buttonsParent.Height = OxPictureActionHelper.DefaultHeight;
                RecalcButtonsSize();
            }
            finally
            {
                buttonsParent.SizeChanged += ButtonsParentSizeChanged;
            }
        }

        private void RecalcButtonsSize()
        {
            if (Image == null)
                return;

            int calcedWidth;
            foreach (OxClickFrame button in Buttons)
            {
                calcedWidth = (buttonsParent.Width / Buttons.Count) 
                        - (int)OxPictureActionHelper.ButtonMargin * Buttons.Count
                        + (int)OxSize.Small;
                button.SetContentSize(
                    calcedWidth,
                    OxPictureActionHelper.DefaultHeight
                );
            }
        }

        private void CreateButtons()
        {
            if (Buttons.Count > 0)
                return;

            foreach (OxPictureAction action in OxPictureActionHelper.List)
                AddButton(action);

            buttonsParent.SizeChanged += ButtonsParentSizeChanged;
            
            if (Buttons.Count > 0)
                Buttons.First()!.Margins.RightOx = OxPictureActionHelper.ButtonMargin;
            
        }

        private void ButtonsParentSizeChanged(object? sender, EventArgs e) => 
            RecalcButtonsSize();

        public OxClickFrame AddButton(OxPictureAction action)
        {
            OxIconButton button = new(
                OxPictureActionHelper.Icon(action), OxPictureActionHelper.DefaultHeight)
            {
                Parent = buttonsParent,
                Dock = DockStyle.Left,
            };

            button.Margins.LeftOx = OxPictureActionHelper.ButtonMargin;
            button.Margins.TopOx = OxSize.Small;
            button.Click += ButtonClick;
            Actions.Add(action, button);
            Buttons.Add(button);
            button.MouseEnter += MouseEnterHandler;
            button.MouseLeave += MouseLeaveHandler;
            button.ToolTipText = OxPictureActionHelper.Text(action);
            return button;
        }

        private void ButtonClick(object? sender, EventArgs e)
        {
            OxPictureAction action = OxPictureAction.Clear;

            foreach (KeyValuePair<OxPictureAction, OxClickFrame> item in Actions)
                if (item.Value == sender)
                    action = item.Key;

            switch (action)
            {
                case OxPictureAction.Clear:
                    ClearImage();
                    break;
                case OxPictureAction.Download:
                    DownloadImage();
                    break;
                case OxPictureAction.Replace:
                    ReplaceImage();
                    break;
            }
        }

        private void DownloadImage()
        {
            if (Image == null)
                return;

            SaveFileDialog dialog = new()
            { 
                FileName = "Unknown",
                Filter = "PNG picture |  *.png; "
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                if (dialog.FileName != string.Empty)
                    Image.Save(dialog.FileName, ImageFormat.Png);
        }

        private void ClearImage() => 
            Image = null;

        public override void ReAlignControls()
        {
            label.SendToBack();
            picture.SendToBack();
            buttonsParent.BringToFront();
            base.ReAlignControls();
        }

        private readonly OxPicture picture = new()
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Stretch = true,
            PicturePadding = 0,
            Cursor = Cursors.Hand
        };
        private Image? sourceImage;

        public OxPictureContainer() 
        {
            CreateButtons();
            PlaceButtons();
        }

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            label.Parent = ContentContainer;
            label.Font = new Font(Font.FontFamily, Font.Size + 1, FontStyle.Italic);
            picture.Parent = this;
            buttonsParent.Parent = this;
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

        private void ClickHandler(object? sender, EventArgs e) => ReplaceImage();

        private void ReplaceImage()
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
            SetHoverHandlers(buttonsParent);
            picture.SizeChanged += Picture_SizeChanged;
        }

        private void Picture_SizeChanged(object? sender, EventArgs e) =>
            RecalcButtonsSize();

        protected override void MouseEnterHandler(object? sender, EventArgs e)
        {
            if (Hovered)
                return;

            base.MouseEnterHandler(sender, e);

            if (!IsHovered)
                return;

            buttonsParent.Visible = picture.Visible;
            ReAlignControls();
        }

        protected override void MouseLeaveHandler(object? sender, EventArgs e)
        {
            if (!Hovered)
                return;

            base.MouseLeaveHandler(sender, e);

            if (buttonsParent.IsHovered)
                return;
            buttonsParent.Visible = false;
            ReAlignControls();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = hovered ? Color.Gray : Color.Silver;
            picture.BaseColor = BaseColor;
            buttonsParent.BackColor = Color.Transparent;
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
                PlaceButtons();
                ReAlignControls();
            }
        }
    }
}