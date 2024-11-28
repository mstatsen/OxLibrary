using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxLoadingPanel : OxFrame
    {
        public OxLoadingPanel() : base(new(300, OxWh.W100))
        {
            base.SetVisible(false);
            Borders.Size = OxWh.W1;
            Dock = OxDock.Fill;

            LoadingLabel = new OxLabel
            {
                AutoSize = false,
                Parent = this,
                Dock = OxDock.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "- - - Loading - - -"
            };
            FontSize = 22;
            Margin.Size = OxWh.W1;
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

        public override void PrepareColors()
        {
            if (InnerChangeBaseColor)
                return;

            InnerChangeBaseColor = true;
            BaseColor =
                UseParentColor 
                && Parent is not null 
                && Parent is OxPanel paneParent
                    ? paneParent.BaseColor
                    : DefaultColor;
            InnerChangeBaseColor = false;

            base.PrepareColors();

            if (LoadingLabel is not null)
                LoadingLabel.ForeColor = Colors.Darker(4);
        }

        public override Color DefaultColor => Color.FromArgb(179, 133, 133);
    }
}