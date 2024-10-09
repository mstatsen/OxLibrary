using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxLoadingPanel : OxFrame
    {
        public OxLoadingPanel() : base(new Size(300, 100))
        {
            base.SetVisible(false);
            Borders.SetSize(OxSize.Small);
            Dock = DockStyle.Fill;

            LoadingLabel = new OxLabel
            {
                AutoSize = false,
                Parent = this,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "- - - Loading - - -"
            };
            FontSize = 22;
            Margins.SetSize(OxSize.Small);
        }


        public bool UseParentColor { get; set; } = true;

        public bool Locked = false;
        protected override void SetVisible(bool value) { }

        public void StartLoading(bool locked = false)
        {
            if (Locked)
                return;

            PrepareColors();
            base.SetVisible(true);
            Locked = locked;
            BringToFront();
            Parent?.Update();
        }

        public void EndLoading()
        {
            Parent?.SuspendLayout();

            try
            {
                if (Locked)
                    return;

                base.SetVisible(false);
                Locked = false;
            }
            finally
            {
                Parent?.ResumeLayout();
            }
        }


        private readonly OxLabel LoadingLabel;

        protected override void SetText(string? value) =>
            LoadingLabel.Text = value;

        public float FontSize
        {
            get => LoadingLabel.Font.Size;
            set => LoadingLabel.Font =
                Styles.Font(value, FontStyle.Bold | FontStyle.Italic);
        }

        private bool InnerChangeBaseColor = false;

        protected override void PrepareColors()
        {
            if (InnerChangeBaseColor)
                return;

            InnerChangeBaseColor = true;
            BaseColor =
                UseParentColor && Parent != null && Parent is OxPane paneParent
                    ? paneParent.BaseColor
                    : DefaultColor;
            InnerChangeBaseColor = false;

            base.PrepareColors();

            if (LoadingLabel != null)
                LoadingLabel.ForeColor = Colors.Darker(4);
        }

        public override Color DefaultColor => Color.FromArgb(179, 133, 133);
    }
}