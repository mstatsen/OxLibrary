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

            LoadingLabel = new OxLabel()
            {
                AutoSize = false,
                Parent = this,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            SetLoadingText(3);
            FontSize = 22;
            Margins.SetSize(OxSize.Small);
            Progress<int> Step = new(step => SetLoadingText(step));
            Waiter = new OxStepWaiter(Step, StepCalcer);
        }

        private bool spaceIncrese = false;
        private int spaceCount = 3;
        private readonly OxStepWaiter Waiter;

        private int StepCalcer()
        {
            if (spaceCount == 3)
                spaceIncrese = false;
            else
            if (spaceCount == 0)
                spaceIncrese = true;

            if (spaceIncrese)
                spaceCount++;
            else spaceCount--;

            return spaceCount;
        }

        private void SetLoadingText(int step)
        {
            string spaces = "";

            for (int i = 0; i < step; i++)
                spaces += " ";

            LoadingLabel.Text = string.Format("-{0}-{0}-{0}Loading{0}-{0}-{0}-", spaces);
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


            if (!Waiter.Enabled)
            {
                spaceCount = 3;
                Waiter.Start();
            }

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
            Waiter.Stop();
        }


        private readonly OxLabel LoadingLabel;

        protected override void SetText(string? value) =>
            LoadingLabel.Text = value;

        public float FontSize
        {
            get => LoadingLabel.Font.Size;
            set => LoadingLabel.Font =
                new Font(Styles.FontFamily, value, FontStyle.Bold | FontStyle.Italic);
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