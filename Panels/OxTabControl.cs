namespace OxLibrary.Panels
{
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
                Header.UnderlineVisible = Header.Dock is DockStyle.Top;
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
            activePage is not null
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
            ActivePage is not null 
                ? Pages.IndexOf(ActivePage) 
                : -1;

        public void AddPage(IOxPane page, Bitmap? icon = null)
        {
            page.Visible = false;
            page.Parent = this;

            if (page.Dock is DockStyle.None)
                page.Dock = DockStyle.Fill;

            pages.Add(page);
            CreateTabButton(page, icon ?? (page is IOxWithIcon iconedPage ? iconedPage.Icon : null));
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
            if (TabButtons is null)
                return;

            foreach (OxTabButton button in TabButtons.Values)
                button.BaseColor = button.Active ? Colors.Darker(2) : BaseColor;
        }

        private void SetTabButtonsVisualParameters()
        {
            if (TabButtons is null)
                return;

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
            if (TabButtons is null)
                return;
            
            foreach (OxTabButton button in TabButtons.Values)
            {
                button.Font = new(Font, Font.Style);
                button.ForeColor = ForeColor;
            }
        }

        private void CreateTabButton(IOxPane page, Bitmap? icon = null)
        {
            if (TabButtons.TryGetValue(page, out _))
                return;

            OxTabButton button = new(page, this, icon);
            button.SetPosition();
            TabButtons.Add(page, button);
        }

        private void SetActivePage(IOxPane? value)
        {
            bool isDifferentPage = 
                (activePage is null 
                    && value is not null) 
                || (activePage is not null 
                    && !activePage.Equals(value));
            IOxPane? oldPage = activePage;

            if (isDifferentPage)
                DeactivatePage?.Invoke(this, new OxTabControlEventArgs(oldPage, value));

            foreach (IOxPane page in Pages)
                if (!page.Equals(value))
                    page.Visible = false;

            activePage = value;

            PrepareColors();
            Update();

            if (activePage is not null)
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
            if (TabButtons is null)
                return;

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