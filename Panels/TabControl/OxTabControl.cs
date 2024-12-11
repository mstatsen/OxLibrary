using OxLibrary.Handlers;
using OxLibrary.Interfaces;
using OxLibrary.ControlList;

namespace OxLibrary.Panels
{
    public class OxTabControl : OxFrameWithHeader
    {
        private readonly OxPanelList pages = new();
        public readonly Dictionary<IOxPanel, OxTabButton> TabButtons = new();
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
            get => Header.Dock;
            set
            {
                Header.Dock = value;
                Header.BorderVisible = Header.Dock is OxDock.Top;
                SetHeaderPaddings();
                SetHeaderSize();
                SetButtonsPostion();
            }
        }

        private void SetHeaderPaddings()
        {
            Header.Padding.Size = 0;

            switch (TabPosition)
            {
                case OxDock.Left:
                case OxDock.Right:
                    Header.Padding.Top = 2;
                    break;
                case OxDock.Bottom:
                    Header.Padding.Left = 4;
                    break;
            }
        }

        private void SetButtonsPostion()
        {
            foreach (OxTabButton button in TabButtons.Values)
                button.SetPosition();
        }

        private void SetHeaderSize() =>
            Header.Size = new(tabHeaderSize.Width, tabHeaderSize.Height);

        public OxPanelList Pages => pages;

        private OxSize tabHeaderSize = new();
        public OxSize TabHeaderSize
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

        private IOxPanel? activePage;
        public IOxPanel? ActivePage
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

        public void AddPage(OxPanel page, Bitmap? icon = null)
        {
            page.Visible = false;
            page.Parent = this;

            if (page.Dock is OxDock.None)
                page.Dock = OxDock.Fill;

            pages.Add(page);
            CreateTabButton(page, icon ?? (page is IOxWithIcon iconedPage ? iconedPage.Icon : null));
            ReAlignTabButtons();
            ActivePage = page;
            PrepareTabButtonsColor();
            SetTabButtonsVisualParameters();
        }

        public override void PrepareColors()
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

        private void CreateTabButton(OxPanel page, Bitmap? icon = null)
        {
            if (TabButtons.TryGetValue(page, out _))
                return;

            OxTabButton button = new(page, this, icon);
            button.SetPosition();
            TabButtons.Add(page, button);
        }

        private void SetActivePage(IOxPanel? value)
        {
            bool isDifferentPage = 
                (activePage is null 
                    && value is not null) 
                || (activePage is not null 
                    && !activePage.Equals(value));
            IOxPanel? oldPage = activePage;

            if (isDifferentPage)
                DeactivatePage?.Invoke(this, new(oldPage, value));

            foreach (IOxPanel page in Pages)
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
            //ContentBox.AutoScroll = true;
            this.AutoScroll = true;
        }

        private void ReAlignTabButtons()
        {
            if (TabButtons is null)
                return;

            foreach (OxTabButton button in TabButtons.Values)
                button.BringToFront();
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
                SetTabButtonsVisualParameters();

            return;
        }

        public event OxTabControlEvent? ActivatePage;
        public event OxTabControlEvent? DeactivatePage;

        public OxTabControl() : base() =>
            Header.Title.Visible = false;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            SetTabButtonsVisualParameters();
        }
    }
}