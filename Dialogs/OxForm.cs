namespace OxLibrary.Dialogs
{
    public class OxForm : Form
    {
        private readonly bool Initialized = false;
        public OxFormMainPanel MainPanel { get; internal set; }

        public OxForm()
        {
            DoubleBuffered = true;
            AutoSize = true;
            SetUpForm();
            MainPanel = CreateMainPanel();
            PlaceMainPanel();
            Initialized = true;
        }

        protected virtual void SetUpForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            SetUpSizes();
            StartPosition = FormStartPosition.CenterParent;
        }

        public void SetUpSizes()
        {
            MaximumSize = Screen.GetWorkingArea(this).Size;
            Size wantedMinimumSize = WantedMinimumSize;
            MinimumSize = new Size(
                Math.Min(wantedMinimumSize.Width, MaximumSize.Width),
                Math.Min(wantedMinimumSize.Height, MaximumSize.Height)
            );
        }


        public virtual Size WantedMinimumSize => new(640, 480);

        protected virtual OxFormMainPanel CreateMainPanel() => new(this);

        private void PlaceMainPanel()
        {
            MainPanel.Parent = this;
            MainPanel.Left = 0;
            MainPanel.Top = 0;
            MainPanel.Width = Width;
            MainPanel.Height = Height;
            MainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (Initialized)
                MainPanel.Controls.Add(e.Control);
            else base.OnControlAdded(e);
        }

        protected override void OnTextChanged(EventArgs e) =>
            MainPanel.Text = Text;

        public void SetContentSize(int width, int height) =>
            MainPanel.SetContentSize(width, height);
        public void SetContentSize(Size size) =>
            MainPanel.SetContentSize(size);

        public virtual bool CanMaximize => true;
        public virtual bool CanMinimize => true;

        public virtual bool Sizable => true;

        public virtual Bitmap? FormIcon => null;

        public void ClearConstraints()
        {
            MinimumSize = new Size(0, 0);
            MaximumSize = MinimumSize;
        }

        public void FreezeSize()
        {
            if (MainPanel != null)
            {
                MinimumSize = new Size(MainPanel.Width, MainPanel.Height);
                MaximumSize = MinimumSize;
            }
        }

        public Color BaseColor
        {
            get => MainPanel.BaseColor;
            set => MainPanel.BaseColor = value;
        }
    }
}