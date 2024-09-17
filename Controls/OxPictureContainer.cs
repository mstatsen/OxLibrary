using OxLibrary.Panels;

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
            Dock = DockStyle.Right
        };
        public OxActionClick<OxPictureAction>? PictureActionClick;

        private void PlaceButtons()
        {
            int left = 0;
            int top = 0;

            foreach (OxClickFrame button in Buttons)
            {
                button.Top = 0;
                button.Left = left;
                button.Margins.LeftOx = OxSize.Small;
                top += button.Height;
            }

            buttonsParent.Height = OxPictureActionHelper.DefaultHeight+ 2;
            
        }

        private void CreateButtons()
        {
            if (Buttons.Count > 0)
                return;

            foreach (OxPictureAction action in Enum.GetValues(typeof(OxPictureAction)))
                AddButton(action);
        }

        public OxClickFrame AddButton(OxPictureAction action)
        {
            OxIconButton button = new(OxPictureActionHelper.Icon(action), 24)
            {
                Parent = buttonsParent
            };

            button.Click += (s, e) => PictureActionClick?.Invoke(s,
                new OxActionEventArgs<OxPictureAction>(
                    s == null
                        ? OxPictureAction.Clear
                        : GetActionByButton((OxButton)s)
                )
            );
            button.SetContentSize(OxPictureActionHelper.DefaultWidth, OxPictureActionHelper.DefaultHeight);
            Actions.Add(action, button);
            Buttons.Add(button);
            return button;
        }

        private OxPictureAction GetActionByButton(OxButton button)
        {
            foreach (var item in Actions)
                if (item.Value == button)
                    return item.Key;

            return OxPictureAction.Clear;
        }

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

        protected override void MouseEnterHandler(object? sender, EventArgs e)
        {
            if (sender == buttonsParent)
                return;

            base.MouseEnterHandler(sender, e);

            buttonsParent.Visible = true;
            ReAlignControls();
        }

        protected override void MouseLeaveHandler(object? sender, EventArgs e)
        {
            base.MouseLeaveHandler(sender, e);
            ReAlignControls();
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            label.ForeColor = hovered ? Color.Gray : Color.Silver;
            picture.BaseColor = BaseColor;
            buttonsParent.BaseColor = Color.Transparent;
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
                ReAlignControls();
            }
        }
    }
}