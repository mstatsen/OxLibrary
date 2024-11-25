using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxPaginator : OxUnderlinedPanel
    {
        private static readonly OxWidth PageButtonWidth = OxWh.W28;
        private static readonly OxWidth PageButtonHeight = OxWh.W20;
        private static readonly OxWidth NavigateButtonWidth = OxWh.Mul(PageButtonWidth, 2);
        private static readonly OxWidth ButtonSpace = OxWh.W3;
        private const int MaximumPageButtonsCount = 10;
        
        

        private int pageSize = 12;
        private int currentPage;
        private int objectCount = 0;

        private readonly List<OxTaggedButton> Buttons = new();
        private readonly OxPane buttonsPanel = new();
        private readonly OxLabel itemsCountLabel = new()
        {
            AutoSize = false,
            Dock = OxDock.Right,
            Width = OxWh.W120,
            TextAlign = ContentAlignment.MiddleRight,
            Font = Styles.Font(FontStyle.Bold | FontStyle.Italic)
        };

        private readonly OxButton PrevButton = CreateNavigateButton(OxIcons.Left, "Previous page");
        private readonly OxButton NextButton = CreateNavigateButton(OxIcons.Right, "Next page");
        private readonly OxButton FirstButton = CreateNavigateButton(OxIcons.First, "First page");
        private readonly OxButton LastButton = CreateNavigateButton(OxIcons.Last, "Last page");
        private readonly OxPane FakeNextButton = new();
        private readonly OxPane FakePrevButton = new();

        public event OxPaginatorEventHandler? PageChanged;

        public int PageSize
        {
            get => pageSize;
            set
            {
                pageSize = value;
                CurrentPage = 1;
            }
        }

        public int CurrentPage
        {
            get => currentPage;
            set => SetCurrentPage(value);
        }

        public OxTaggedButton? CurrentButton =>
            Buttons.Find(b => b.Tag.Equals(currentPage));

        private void SetCurrentPage(int value)
        {
            currentPage = value;
            SetButtonsVisible();
            RenumerateButtons();
            SetFakeButtonsVisible();
            IterateButtons(FreezeCurrentButton);
            PrepareColors();
            IterateButtons(SetButtonSize);
            SetButtonsTop();
            IterateButtons(SetButtonFont);
            PlaceButtons();
            SetItemsCountText();
            Update();
            NotifyAboutPageChanged();
        }

        private void SetItemsCountText()
        {
            int startObjectIndex = 1 + (currentPage - 1) * PageSize;

            if (startObjectIndex > objectCount)
                startObjectIndex = objectCount;

            int endObjectIndex = currentPage * PageSize;

            if (endObjectIndex > objectCount)
                endObjectIndex = ObjectCount;

            if (objectCount > 0)
                itemsCountLabel.Text = $"{startObjectIndex}-{endObjectIndex} / {ObjectCount}  ";
            else itemsCountLabel.Text = "Not items";
        }

        private void SetFakeButtonsVisible()
        {
            if (Buttons.Count > 0)
            {
                OxTaggedButton lastPageButton = Buttons[^1];
                FakeNextButton.Visible =
                    lastPageButton is not null
                    && lastPageButton.Visible
                    && !lastPageButton.Tag.Equals(PageCount);
                FakePrevButton.Visible = Buttons[0].Tag is not 1;
            }
            else
            {
                FakeNextButton.Visible = false;
                FakePrevButton.Visible = false;
            }
        }

        private void SetButtonsVisible()
        {
            PrevButton.Visible = 
                objectCount > 0 
                && currentPage is not 1;
            FirstButton.Visible = PrevButton.Visible;
            NextButton.Visible = 
                objectCount > 0 
                && !currentPage.Equals(PageCount);
            LastButton.Visible = NextButton.Visible;
        }

        private void NotifyAboutPageChanged() => 
            PageChanged?.Invoke(
                this,
                new OxPaginatorEventArgs(
                    currentPage,
                    0 + (currentPage - 1) * PageSize,
                    currentPage * PageSize)
            );

        private void IterateButtons(Func<OxTaggedButton, bool> iterator)
        {
            foreach (OxTaggedButton button in Buttons)
                iterator(button);
        }

        private bool FreezeCurrentButton(OxTaggedButton button) =>
            button.FreezeHovered = button.Equals(CurrentButton);

        private bool SetButtonSize(OxTaggedButton button)
        {
            button.Size = new(
                PageButtonWidth + (button.FreezeHovered ? 8 : 0),
                PageButtonHeight + (button.FreezeHovered ? 8 : 0)
            );
            return true;
        }

        private bool SetButtonFont(OxTaggedButton button)
        {
            button.Font = new(
                button.Font,
                button.FreezeHovered ? FontStyle.Bold : FontStyle.Regular);
            return true;
        }

        public int ObjectCount
        {
            get => objectCount;
            set => SetObjectCount(value);
        }

        private void SetObjectCount(int value)
        {
            objectCount = value;
            CreateButtons();
            PlaceButtons();
            CurrentPage = 1;
        }

        private static OxWidth PlaceButton(OxPane button, OxWidth left)
        {
            button.Left = left;
            return button.Right | ButtonSpace;
        }

        private void PlaceButtons()
        {
            OxWidth lastRight = PlaceButton(FirstButton, 0);
            lastRight = PlaceButton(PrevButton, lastRight);
            lastRight = PlaceButton(FakePrevButton, lastRight);

            foreach (OxTaggedButton button in Buttons)
                lastRight = PlaceButton(button, lastRight);

            lastRight = PlaceButton(FakeNextButton, lastRight);
            lastRight = PlaceButton(NextButton, lastRight);
            PlaceButton(LastButton, lastRight);

            SetButtonsPanelWidth();
            SetButtonsPanelLeft();
            SetButtonsTop();
        }

        private int PageCount =>
            (int)Math.Ceiling((decimal)objectCount / pageSize);

        private int ButtonCount =>
            Math.Min(PageCount, MaximumPageButtonsCount);

        private void RenumerateButtons()
        {
            if (CurrentButton is not null)
                return;

            int buttonIndex = 1;

            if (Buttons.Count > 0)
            {
                if (Buttons[0].Tag > CurrentPage)
                    buttonIndex = CurrentPage;
                else
                if (Buttons[^1].Tag < CurrentPage)
                    buttonIndex = CurrentPage - (MaximumPageButtonsCount - 1);
            }

            foreach (OxTaggedButton button in Buttons)
                button.Tag = buttonIndex++;
        }

        private bool DisposeButton(OxTaggedButton button)
        {
            button.Dispose();
            return true;
        }

        private void CreateButtons()
        {
            IterateButtons(DisposeButton);
            Buttons.Clear();

            for (int i = 1; i <= ButtonCount; i++)
                CreatePageButton(i);
        }

        private void CreatePageButton(int pageNumber)
        {
            OxTaggedButton button = new(pageNumber)
            {
                Parent = buttonsPanel,
                Tag = pageNumber,
                HandHoverCursor = true,
                Size = new(PageButtonWidth, PageButtonHeight)
            };
            button.Click += (s, e) => CurrentPage = s is not null ? ((OxTaggedButton)s).Tag : 0;
            Buttons.Add(button);
        }

        private static OxButton CreateNavigateButton(Bitmap icon, string toolTipText)
        {
            OxButton button = new(string.Empty, icon)
            {
                Visible = false,
                HandHoverCursor = true,
                ToolTipText = toolTipText,
                Size = new(NavigateButtonWidth, PageButtonHeight)
            };
            return button;
        }

        private void PrepareNavigateButton(OxButton button, EventHandler clickHandler)
        {
            button.Parent = buttonsPanel;
            button.Click += clickHandler;
        }

        private OxPane PrepareFakeButton(OxPane button, ContentAlignment textAlign)
        {
            button.Parent = buttonsPanel;
            button.Size = new(NavigateButtonWidth, PageButtonWidth);
            _ = new OxLabel()
            {
                Parent = button,
                Dock = OxDock.Fill,
                Text = "...",
                AutoSize = false,
                TextAlign = textAlign,
                Font = Styles.Font(FontStyle.Bold)
            };

            return button;
        }

        public OxPaginator() : base(OxSize.Empty) { }

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            itemsCountLabel.Parent = this;
            buttonsPanel.Parent = this;
            buttonsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            SetButtonsPanelLeft();

            PrepareNavigateButton(FirstButton, (s, e) => CurrentPage = 1);
            PrepareNavigateButton(PrevButton, (s, e) => CurrentPage--);
            PrepareNavigateButton(NextButton, (s, e) => CurrentPage++);
            PrepareNavigateButton(LastButton, (s, e) => CurrentPage = PageCount);
            PrepareFakeButton(FakePrevButton, ContentAlignment.BottomRight);
            PrepareFakeButton(FakeNextButton, ContentAlignment.BottomLeft);
        }

        private void SetButtonsPanelWidth() =>
            buttonsPanel.Size = new(LastButton.Right | ButtonSpace, Height);

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Height = OxWh.W40;
        }

        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
            {
                SetButtonsPanelLeft();
                SetButtonsTop();
            }

            return e.Changed;
        }

        private void SetButtonTop(OxPane button) =>
            button.Top = 
                OxWh.Div(
                    OxWh.Sub(buttonsPanel.Height, button.Height), 
                    OxWh.W2
                );

        private bool SetButtonTop(OxTaggedButton button)
        {
            SetButtonTop((OxPane)button);
            return true;
        }

        private void SetButtonsTop()
        {
            IterateButtons(SetButtonTop);
            SetButtonTop(PrevButton);
            SetButtonTop(NextButton);
            SetButtonTop(FirstButton);
            SetButtonTop(LastButton);
            SetButtonTop(FakePrevButton);
            SetButtonTop(FakeNextButton);
        }

        private void SetButtonsPanelLeft() =>
            buttonsPanel.Left = 
                OxWh.Div(
                    OxWh.Sub(Width, buttonsPanel.Width), 
                    OxWh.W2
                );

        private bool SetButtonBaseColor(OxTaggedButton button)
        {
            button.BaseColor = BaseColor;
            return true;
        }

        protected override void PrepareColors()
        {
            base.PrepareColors();
            buttonsPanel.BaseColor = BaseColor;
            PrevButton.BaseColor = BaseColor;
            NextButton.BaseColor = BaseColor;
            FirstButton.BaseColor = BaseColor;
            LastButton.BaseColor = BaseColor;
            FakeNextButton.BaseColor = BaseColor;
            FakePrevButton.BaseColor = BaseColor;
            IterateButtons(SetButtonBaseColor);
        }
    }
}