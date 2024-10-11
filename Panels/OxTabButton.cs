using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxTabButton : OxButton
    {
        public IOxPane Page { get; internal set; }
        public OxTabControl TabControl { get; internal set; }

        public OxTabButton(IOxPane page, OxTabControl tabControl, Bitmap? icon = null) : base(page.Text, icon)
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
            Click += (s, e) => TabControl.ActivePage = Page;

            foreach (OxDock dock in OxDockHelper.All())
                Borders[dock].VisibleChanged += BorderVisibleChanged;
        }

        protected override void AfterCreated()
        {
            base.AfterCreated();
            HiddenBorder = false;
        }

        private void BorderVisibleChanged(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            OxBorder border = (OxBorder)sender;
            Margins[border.OxDock].SetSize(border.Visible ? 0 : border.GetSize());
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
}