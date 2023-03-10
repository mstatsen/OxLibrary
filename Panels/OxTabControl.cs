using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxTabControlEventArgs
    {
        public IOxPane? OldPage { get; private set; }
        public IOxPane? Page { get; private set; }
        public OxTabControlEventArgs(IOxPane? oldPage, IOxPane? page)
        {
            OldPage = oldPage;
            Page = page;
        }
    }

    public delegate void OxTabControlEvent(object sender, OxTabControlEventArgs e);

    public class OxTabButton : OxButton
    {
        public IOxPane Page { get; internal set; }
        public OxTabControl TabControl { get; internal set; }

        public OxTabButton(IOxPane page, OxTabControl tabControl) : base(page.Text, null)
        {
            TabControl = tabControl;
            Page = page;
            Text = Page.Text;
            Parent = TabControl.Header;
            Dock = DockStyle.Left;
            Font = TabControl.Font;
            SetContentSize(tabControl.TabHeaderSize.Width, tabControl.TabHeaderSize.Height);
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            Click += ClickHander;

            foreach (OxDock dock in OxDockHelper.All())
                Borders[dock].VisibleChanged += BorderVisibleChanged;
        }

        private void ClickHander(object? sender, EventArgs e) =>
            TabControl.ActivePage = Page;


        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = false;
        }

        private void BorderVisibleChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            OxDock dock = ((OxBorder)sender).OxDock;
            Margins[dock].SetSize(Borders[dock].Visible ? 0 : Borders[dock].GetSize());
        }

        public bool Active =>
            Page != null && Page.Equals(TabControl.ActivePage);

        public int PageIndex =>
            TabControl != null && Page != null ? TabControl.Pages.IndexOf(Page) : -1;

        public bool IsFirstPage
        {
            get
            {
                if (TabControl == null)
                    return true;

                foreach (OxTabButton button in TabControl.VisibleTabButtons())
                {
                    if (button.Equals(this))
                        break;

                    return false;
                }

                return true;
            }
        }

        public bool IsLastPage
        {
            get
            {
                if (TabControl == null)
                    return true;

                List<OxTabButton> buttonList = TabControl.VisibleTabButtons();
                buttonList.Reverse();

                foreach (OxTabButton button in buttonList)
                {
                    if (button.Equals(this))
                        break;

                    return false;
                }

                return true;
            }
        }

        private bool IsPrevButton(OxTabButton? currentButton)
        {
            if (TabControl == null)
                return false;

            if (currentButton == null)
                return false;

            List<OxTabButton> buttonList = TabControl.VisibleTabButtons();
            buttonList.Reverse();
            bool nextReturn = false;

            foreach (OxTabButton button in buttonList)
            {
                if (nextReturn)
                    return button.Equals(this);

                nextReturn = button.Equals(currentButton);
            }

            return false;
        }

        internal void SetVisualParameters()
        {
            if (TabControl == null)
                return;

            Font = new Font(
                Font.FontFamily,
                Font.Size,
                Active ? FontStyle.Bold : FontStyle.Regular);

            OxDock startDock = OxDock.Left;
            OxDock endDock = OxDock.Right;

            switch (TabControl.TabPosition)
            {
                case OxDock.Left:
                case OxDock.Right:
                    startDock = OxDock.Top;
                    endDock = OxDock.Bottom;
                    break;
            }

            Borders[startDock].Visible =
                Active
                || IsFirstPage;

            Borders[endDock].Visible =
                Active
                || IsLastPage
                || !IsPrevButton(TabControl.ActiveTabButton);

            Margins[TabControl.TabPosition].SetSize(
                Active ?
                    (int)OxSize.None
                    : OxDockHelper.IsVertical(TabControl.TabPosition)
                        ? (int)OxSize.Large
                        : (int)OxSize.Extra * 3
            );

            Borders[TabControl.TabPosition].Visible = true;
            OxDock oppositeDock = OxDockHelper.Opposite(TabControl.TabPosition);
            Margins[oppositeDock].SetSize(OxSize.None);
            Borders[oppositeDock].Visible = false;
            Text += string.Empty; //for recalc label size
        }

        internal void SetPosition()
        {
            if (TabControl == null)
                return;

            DockStyle buttonDock = DockStyle.Left;

            switch (TabControl.TabPosition)
            {
                case OxDock.Left:
                case OxDock.Right:
                    buttonDock = DockStyle.Top;
                    break;
            }

            Dock = buttonDock;
            Width = TabControl.TabHeaderSize.Width;
            Height = TabControl.TabHeaderSize.Height;
            SetVisualParameters();
        }
    }

    public class OxTabControl : OxFrameWithHeader
    {
        private readonly OxPaneList pages = new();
        public readonly Dictionary<IOxPane, OxTabButton> TabButtons = new();
        internal List<OxTabButton> VisibleTabButtons()
        {
            List<OxTabButton> buttonList = new();

            foreach (OxTabButton button in TabButtons.Values)
                if (button.Visible)
                    buttonList.Add(button);

            return buttonList;
        }

        public OxDock TabPosition
        {
            get => OxDockHelper.Dock(Header.Dock);
            set
            {
                Header.Dock = OxDockHelper.Dock(value);
                Header.UnderlineVisible = Header.Dock == DockStyle.Top;
                SetHeaderPaddings();
                SetHeaderSize();
                SetButtonsPostion();
                ReAlign();
            }
        }

        private void SetHeaderPaddings()
        {
            Header.Paddings.SetSize(OxSize.None);

            switch (TabPosition)
            {
                case OxDock.Left:
                case OxDock.Right:
                    Header.Paddings.TopOx = OxSize.Medium;
                    break;
                case OxDock.Bottom:
                    Header.Paddings.LeftOx = OxSize.Large;
                    break;
            }
        }

        private void SetButtonsPostion()
        {
            foreach (OxTabButton button in TabButtons.Values)
                button.SetPosition();
        }

        private void SetHeaderSize() =>
            Header.SetContentSize(tabHeaderSize.Width, tabHeaderSize.Height);

        public OxPaneList Pages => pages;

        private Size tabHeaderSize;
        public Size TabHeaderSize
        {
            get => tabHeaderSize;
            set
            {
                tabHeaderSize = value;
                SetHeaderSize();
                SetButtonsPostion();
            }
        }

        internal OxTabButton? ActiveTabButton =>
            activePage != null
                ? TabButtons[activePage]
                : null;

        private IOxPane? activePage;
        public IOxPane? ActivePage
        {
            get => activePage;
            set => SetActivePage(value);
        }

        public void ActivateFirstPage()
        {
            if (Pages.Count > 0)
                ActivePage = Pages[0];
        }

        public int ActivePageIndex =>
            ActivePage != null ? Pages.IndexOf(ActivePage) : -1;

        public void AddPage(IOxPane page)
        {
            page.Visible = false;
            page.Parent = this;

            if (page.Dock == DockStyle.None)
                page.Dock = DockStyle.Fill;

            pages.Add(page);

            CreateTabButton(page);
            ReAlignTabButtons();
            ActivePage = page;
            PrepareTabButtonsColor();
            SetTabButtonsVisualParameters();
        }

        protected override void PrepareColors()
        {
            PrepareTabButtonsColor();
            base.PrepareColors();
            SetTabButtonsVisualParameters();
        }

        private void PrepareTabButtonsColor()
        {
            if (TabButtons != null)
                foreach (OxTabButton button in TabButtons.Values)
                    button.BaseColor = button.Active ? Colors.Darker(2) : BaseColor;
        }

        private void SetTabButtonsVisualParameters()
        {
            if (TabButtons != null)
                foreach (OxTabButton button in TabButtons.Values)
                    button.SetVisualParameters();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SetTabButtonsFont();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SetTabButtonsFont();
        }

        private void SetTabButtonsFont()
        {
            if (TabButtons != null)
                foreach (OxTabButton button in TabButtons.Values)
                {
                    button.Font = new Font(Font.FontFamily, Font.Size, Font.Style);
                    button.ForeColor = ForeColor;
                }
        }

        private void CreateTabButton(IOxPane page)
        {
            if (!TabButtons.TryGetValue(page, out _))
            {
                OxTabButton button = new(page, this);
                button.SetPosition();
                TabButtons.Add(page, button);
            }
        }

        private void SetActivePage(IOxPane? value)
        {
            bool isDifferentPage = activePage != value;
            IOxPane? oldPage = activePage;

            if (isDifferentPage)
                DeactivatePage?.Invoke(this, new OxTabControlEventArgs(oldPage, value));

            foreach (IOxPane page in Pages)
                if (page != value && page.Visible)
                    page.Visible = false;

            activePage = value;

            PrepareColors();
            Update();

            if (activePage != null)
                activePage.Visible = true;

            if (isDifferentPage)
                ActivatePage?.Invoke(this, new OxTabControlEventArgs(oldPage, activePage));
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Borders[OxDock.Top].Visible = false;
            Text = string.Empty;
            ContentContainer.AutoScroll = true;
        }

        public override void ReAlignControls()
        {
            ContentContainer.ReAlign();
            Paddings.ReAlign();
            Borders.ReAlign();
            Header.ReAlign();
            Margins.ReAlign();
            ReAlignTabButtons();
            SendToBack();
        }

        private void ReAlignTabButtons()
        {
            if (TabButtons != null)
                foreach (OxTabButton button in TabButtons.Values)
                    button.BringToFront();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetTabButtonsVisualParameters();
        }

        public event OxTabControlEvent? ActivatePage;
        public event OxTabControlEvent? DeactivatePage;

        public OxTabControl() : base() =>
            Header.Label.Visible = false;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            SetTabButtonsVisualParameters();
        }
    }
}