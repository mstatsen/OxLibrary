namespace OxLibrary.Panels
{
    public class OxPanelLayouter : OxPanel
    {
        private bool panelResizing = false;
        private const int MaximumColumnCount = 48;
        private int CalcedColumnCount = MaximumColumnCount;
        private readonly OxPaneDictionary columnsPanels = new();
        private readonly OxPanelList columns = new();
        private readonly OxPanelList placedPanels = new();

        public bool IsEmpty => 
            placedPanels.Count is 0;

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            CreateColumnsPanels();
        }

        private void CreateColumnsPanels()
        {
            for (int i = 0; i < MaximumColumnCount; i++)
            {
                OxPanel column = new()
                {
                    Parent = this,
                    Dock = OxDock.Left,
                    BaseColor = BaseColor
                };
                columns.Add(column);
                columnsPanels.Add(column);
            }

            columns.Reverse();
        }

        public OxPanelLayouter() : base()
        {
            Dock = OxDock.Top;
            SetSizes();
        }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            SizeChanged += ReLayoutPanelsHandler;
        }

        public override void PrepareColors()
        {
            base.PrepareColors();

            if (columns is not null)
                foreach (OxPanel column in columns)
                    column.BaseColor = BaseColor;
        }

        private void SetSizes()
        {
            Padding.TopInt = 18;
            Padding.BottomInt = 18;
        }

        private void PlacePanel(OxPanel panel)
        {
            OxPanel column = columns[(int)panel.Tag];
            panel.Parent = column;
            panel.Dock = OxDock.Top;
            columnsPanels[column].Add(panel);
        }

        public void LayoutPanels<T>(List<T> list)
            where T : OxPanel
        {
            OxPanelList panelList = new();

            foreach (T item in list)
                panelList.Add(item);

            LayoutPanels(panelList);
        }

        public void LayoutPanels(OxPanelList panelList)
        {
            if (panelList.Count is 0)
            {
                /*TODO: need review this code
                    SetEmptySize();
                */
                return;
            }

            int oldCalcColumnsCount = IsEmpty ? -1 : CalcedColumnCount;

            if (!panelList.Equals(placedPanels))
                InitPlacedPanels(panelList);

            RecalcColumnsCount();

            if (oldCalcColumnsCount.Equals(CalcedColumnCount))
                return;

            SetColumnsDock();
            ClearColumnsPanels();
            SetPanelsVisible(false);
            PlacePanelsOnColumns();
            SetPanelsVisible(true);
            RecalcColumnsSize();
            SetVisibleChangedHandlerToPanels();
        }

        private void InitPlacedPanels(OxPanelList panelList)
        {
            placedPanels.Clear();
            placedPanels.AddRange(panelList);
            placedPanels.Reverse();
        }

        private void PlacePanelsOnColumns()
        {
            SetPanelsColumnNumber();

            foreach (OxPanel panel in placedPanels)
                PlacePanel(panel);
        }

        private void SetPanelsColumnNumber()
        {
            int columnNumber = GetLastPanelColumnNumber(placedPanels);

            foreach (OxPanel panel in placedPanels)
            {
                panel.Tag = columnNumber;
                columnNumber = GetNextColumnNumber(columnNumber);
            }
        }

        private int GetNextColumnNumber(int columnNumber)
        {
            columnNumber--;

            if (columnNumber.Equals(columns.Count))
                columnNumber = 0;
            else
            if (columnNumber < 0)
                columnNumber = CalcedColumnCount - 1;

            return columnNumber;
        }

        private void SetVisibleChangedHandlerToPanels()
        {
            foreach (OxPanel panel in placedPanels)
                panel.VisibleChanged += PanelVisibleChangedHander;
        }

        private void SetEmptySize()
        {
            SilentSizeChange(() =>
                {
                    Size = new();
                },
                Size
            );
        }

        private OxDock GetColumnDock() =>
            panelsAlign is PanelsHorizontalAlign.OneColumn
                ? OxDock.Top
                : OxDock.Left;

        private void SetColumnsDock()
        {
            foreach (OxPanel column in columns)
                column.Dock = GetColumnDock();
        }

        private void PanelVisibleChangedHander(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            Control panel = (Control)sender;

            if (panel.Visible)
                panel.SizeChanged += SizeChangeHandler;
            else panel.SizeChanged -= SizeChangeHandler;
        }

        private void RecalcColumnsCount()
        {
            if (panelsAlign is PanelsHorizontalAlign.OneColumn)
            {
                CalcedColumnCount = 1;
                return;
            }

            CalcedColumnCount = 0;
            OxWidth calcedWidth = OxWh.W0;

            foreach (OxPanel panel in placedPanels)
            {
                calcedWidth |= panel.Width;

                if (OxWh.GreaterOrEquals(calcedWidth, Width))
                    break;
                else
                    CalcedColumnCount++;
            }

            if (CalcedColumnCount is 0)
                CalcedColumnCount++;
            else 
                if (CalcedColumnCount > MaximumColumnCount)
                    CalcedColumnCount = MaximumColumnCount;
        }

        private void ReLayoutPanelsHandler(object? sender, EventArgs e)
        {
            if (!SizeChanging 
                && !panelResizing)
                LayoutPanels(placedPanels);
        }

        private int GetLastPanelColumnNumber(OxPanelList panelList)
        {
            int columnNumber = CalcedColumnCount - 1;
            int panelsCount = panelList.Count;

            if (panelsCount % CalcedColumnCount is not 0)
                columnNumber = panelsCount % CalcedColumnCount - 1;

            if (columnNumber.Equals(columns.Count))
                columnNumber = 0;

            return columnNumber;
        }

        private void SetPanelsVisible(bool visible)
        {
            placedPanels.Reverse();

            try
            {
                int itemIndex = 0;

                foreach (OxPanel panel in placedPanels)
                {
                    panel.Visible = 
                        visible 
                        && (RealPlacedCount is -1 
                            || itemIndex < RealPlacedCount);
                    itemIndex++;
                }
            }
            finally
            {
                placedPanels.Reverse();
            }
        }

        public int RealPlacedCount { get; set; } = -1;

        private void ClearColumnsPanels()
        {
            foreach (OxPanel column in columns)
                columnsPanels[column].Clear();
        }

        private OxWidth SetColumnSize(OxPanel column)
        {
            OxWidth calcedHeight = OxWh.W0;
            OxWidth maxWidth = OxWh.W1;

            foreach (OxPanel panel in columnsPanels[column])
                if (panel.Visible)
                {
                    calcedHeight |= panel.Height;
                    maxWidth = OxWh.Max(maxWidth, panel.Width);
                }

            if (panelsAlign is PanelsHorizontalAlign.OneColumn)
                calcedHeight |= OxWh.W48;

            column.Size = new(maxWidth, calcedHeight);
            return calcedHeight;
        }

        private void SizeChangeHandler(object? sender, EventArgs e) =>
            RecalcColumnsSize();

        private void RecalcColumnsSize()
        {
            if (!Visible)
                return;

            panelResizing = true;

            try
            {
                SetColumnsSize();
                OxSize columnsSize = GetSumColumnsSize();
                columnsSize.Height +=
                    Padding.TopInt
                    + Padding.BottomInt;
                Size = columnsSize;
            }
            finally
            {
                panelResizing = false;
            }
        }

        private OxSize GetSumColumnsSize()
        {
            if (columns is null)
                return OxSize.Empty;

            OxWidth sumWidth = OxWh.W0;
            OxWidth maxColumnHeight = OxWh.W0;

            foreach (OxPanel column in columns)
            {
                OxWidth columnHeight = SetColumnSize(column);
                maxColumnHeight = OxWh.Max(maxColumnHeight, columnHeight);

                if (columnsPanels[column].Count is not 0)
                    sumWidth |= column.Width;
            }

            return new(sumWidth, maxColumnHeight);
        }

        private void SetColumnsSize()
        {
            foreach (OxPanel column in columns)
                SetColumnSize(column);
        }

        private void SetLeftPadding()
        {
            switch (panelsAlign)
            {
                case PanelsHorizontalAlign.Left:
                    Padding.LeftInt = 0;
                    break;
                case PanelsHorizontalAlign.Center:
                case PanelsHorizontalAlign.Right:
                    Padding.LeftInt = (Width - GetSumColumnsSize().Width) / 2;
                    break;
                case PanelsHorizontalAlign.OneColumn:
                    Padding.HorizontalInt = 0;
                    break;
            }
        }

        public virtual void Clear()
        {
            placedPanels.Clear();

            foreach (OxPanelList panelsList in columnsPanels.Values)
            {
                foreach (OxPanel panel in panelsList)
                {
                    panel.SizeChanged -= SizeChangeHandler;
                    panel.Visible = false;
                    panel.Parent = null;
                }

                panelsList.Clear();
            }

            Size = OxSize.Empty;
            UpdateParent();
        }

        public override bool OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.Changed)
                SetLeftPadding();

            return e.Changed;
        }

        public PanelsHorizontalAlign PanelsAlign
        {
            get => panelsAlign;
            set => SetPanelsAlign(value);
        }

        private void SetPanelsAlign(PanelsHorizontalAlign value)
        {
            panelsAlign = value;
            SetTopPadding();
            SetLeftPadding();
        }

        private void SetTopPadding() => 
            Padding.TopInt =
                panelsAlign is PanelsHorizontalAlign.OneColumn
                    ? 16
                    : 18;

        protected void UpdateParent() =>
            Parent?.Update();

        private PanelsHorizontalAlign panelsAlign = PanelsHorizontalAlign.Center;

        public override Color DefaultColor => Color.GhostWhite;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            SetPanelsVisible(Visible);
            RecalcColumnsSize();
        }
    }
}