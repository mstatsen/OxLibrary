using OxLibrary.Panels;

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
            MainPanel = CreateMainPanel();
            PlaceMainPanel();
            SetUpForm();
            Initialized = true;
        }

        protected virtual void SetUpForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            SetUpSizes();
            StartPosition = FormStartPosition.CenterParent;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MainPanel.SetIcon();
        }

        public void SetUpSizes()
        {
            MaximumSize = Screen.GetWorkingArea(this).Size;
            Size wantedMinimumSize = WantedMinimumSize;
            MinimumSize = new(
                Math.Min(wantedMinimumSize.Width, MaximumSize.Width),
                Math.Min(wantedMinimumSize.Height, MaximumSize.Height)
            );
        }

        public virtual Size WantedMinimumSize => new(640, 480);

        protected virtual OxFormMainPanel CreateMainPanel() => new(this);

        private void PlaceMainPanel()
        {
            ((Control)MainPanel).Parent = this;
            MainPanel.Left = 0;
            MainPanel.Top = 0;
            MainPanel.Size = new(Width, Height);
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (MainPanel is not null)
                MainPanel.Size = Size;
        }

        private bool canMaximize = true;
        private bool canMinimize = true;

        public bool CanMaximize
        {
            get => canMaximize;
            set
            {
                canMaximize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }

        public bool CanMinimize
        {
            get => canMinimize;
            set
            {
                canMinimize = value;
                MainPanel.SetTitleButtonsVisible();
            }
        }


        private bool sizeble = true;
        public bool Sizeble 
        { 
            get => sizeble;
            set
            {
                sizeble = value;
                MainPanel.SetMarginsSize();
            }
        }

        public virtual Bitmap? FormIcon => null;

        public void ClearConstraints()
        {
            MinimumSize = Size.Empty;
            MaximumSize = MinimumSize;
        }

        public void FreezeSize()
        {
            if (MainPanel is not null)
            {
                MinimumSize = new(MainPanel.Width, MainPanel.Height);
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