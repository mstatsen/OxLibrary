using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;

namespace OxLibrary.Panels
{
    public class OxPaginator : OxUnderlinedPanel
    {
        private static readonly short PageButtonWidth = 28;
        private static readonly short PageButtonHeight = 20;
        private static readonly short NavigateButtonWidth = OxSH.X2(PageButtonWidth);
        private static readonly short ButtonSpace = 3;
        private static readonly short MaximumPageButtonsCount = 10;

        private short pageSize = 12;
        private short currentPage;
        private int objectCount = 0;

        private readonly List<OxTaggedButton> Buttons = new();
        private readonly OxPanel buttonsPanel = new();
        private readonly OxLabel itemsCountLabel = new()
        {
            AutoSize = false,
            Dock = OxDock.Right,
            Width = 120,
            TextAlign = ContentAlignment.MiddleRight,
            Font = OxStyles.Font(FontStyle.Bold | FontStyle.Italic)
        };

        private readonly OxButton PrevButton = CreateNavigateButton(OxIcons.Left, "Previous page");
        private readonly OxButton NextButton = CreateNavigateButton(OxIcons.Right, "Next page");
        private readonly OxButton FirstButton = CreateNavigateButton(OxIcons.First, "First page");
        private readonly OxButton LastButton = CreateNavigateButton(OxIcons.Last, "Last page");
        private readonly OxPanel FakeNextButton = new();
        private readonly OxPanel FakePrevButton = new();

        public event OxPaginatorEventHandler? PageChanged;

        public short PageSize
        {
            get => pageSize;
            set
            {
                pageSize = value;
                CurrentPage = 1;
            }
        }

        public short CurrentPage
        {
            get => currentPage;
            set => SetCurrentPage(value);
        }

        public OxTaggedButton? CurrentButton =>
            Buttons.Find(b => b.Tag.Equals(currentPage));

        private void SetCurrentPage(short value)
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

        private void IterateButtons(Action<OxTaggedButton> iterator)
        {
            foreach (OxTaggedButton button in Buttons)
                iterator(button);
        }

        private void FreezeCurrentButton(OxTaggedButton button) =>
            button.FreezeHovered = button.Equals(CurrentButton);

        private void SetButtonSize(OxTaggedButton button)
        {
            short freezeSize = OxSH.Short(button.FreezeHovered ? 8 : 0);
            button.Size = new(
                OxSH.Add(PageButtonWidth, freezeSize),
                OxSH.Add(PageButtonHeight, freezeSize)
            );
        }

        private void SetButtonFont(OxTaggedButton button) =>
            button.Font = new(
                button.Font,
                button.FreezeHovered ? FontStyle.Bold : FontStyle.Regular
            );

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

        private static short PlaceButton(OxPanel button, short left)
        {
            button.Left = left;
            return OxSH.Add(button.Right, ButtonSpace);
        }

        private void PlaceButtons()
        {
            short lastRight = PlaceButton(FirstButton, 0);
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

        private short PageCount =>
            OxSH.Ceiling((decimal)objectCount / pageSize);

        private short ButtonCount =>
            OxSH.Min(PageCount, MaximumPageButtonsCount);

        private void RenumerateButtons()
        {
            if (CurrentButton is not null)
                return;

            short buttonIndex = 1;

            if (Buttons.Count > 0)
            {
                if (Buttons[0].Tag > CurrentPage)
                    buttonIndex = CurrentPage;
                else
                if (Buttons[^1].Tag < CurrentPage)
                    buttonIndex = OxSH.Sub(CurrentPage, MaximumPageButtonsCount, 1);
            }

            foreach (OxTaggedButton button in Buttons)
                button.Tag = buttonIndex++;
        }

        private static void DisposeButton(OxTaggedButton button) =>
            button.Dispose();

        private void CreateButtons()
        {
            IterateButtons(DisposeButton);
            Buttons.Clear();

            for (short i = 1; i <= ButtonCount; i++)
                CreatePageButton(i);
        }

        private void CreatePageButton(short pageNumber)
        {
            OxTaggedButton button = new(pageNumber)
            {
                Parent = buttonsPanel,
                Tag = pageNumber,
                HandHoverCursor = true,
                Size = new(PageButtonWidth, PageButtonHeight)
            };
            button.Click += ButtonClickHandler;
            Buttons.Add(button);
        }

        private void ButtonClickHandler(object? sender, EventArgs e) =>
            CurrentPage = OxSH.Short(sender is OxTaggedButton taggedButton ? taggedButton.Tag : 0);

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

        private OxPanel PrepareFakeButton(OxPanel button, ContentAlignment textAlign)
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
                Font = OxStyles.Font(FontStyle.Bold)
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
            buttonsPanel.Size = new(
                LastButton.Right + ButtonSpace,
                Height
            );

        protected override void AfterCreated()
        {
            base.AfterCreated();
            Height = 40;
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            if (!e.Changed)
                return;

            base.OnSizeChanged(e);
            SetButtonsPanelLeft();
            SetButtonsTop();
        }

        private void SetButtonTop(OxPanel button) =>
            button.Top = OxSH.CenterOffset(buttonsPanel.Height, button.Height);

        private void SetButtonTop(OxTaggedButton button) =>
            SetButtonTop((OxPanel)button);

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
            buttonsPanel.Left = OxSH.CenterOffset(Width, buttonsPanel.Width);

        private void SetButtonBaseColor(OxTaggedButton button) => 
            button.BaseColor = BaseColor;

        public override void PrepareColors()
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