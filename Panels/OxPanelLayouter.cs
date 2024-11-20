namespace OxLibrary.Panels
{
    public class OxPanelLayouter : OxPane
    {
        private bool panelResizing = false;
        private const int MaximumColumnCount = 48;
        private int CalcedColumnCount = MaximumColumnCount;
        private readonly OxPaneDictionary columnsPanels = new();
        private readonly OxPaneList columns = new();
        private readonly OxPaneList placedPanels = new();

        public bool IsEmpty => 
            placedPanels.Count is 0;

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
            CreateColumnsPanels();
        }

        private void CreateColumnsPanels()
        {
            for (int i = 0; i < MaximumColumnCount; i++)
            {
                OxPane column = new()
                {
                    Parent = this,
                    Dock = OxDock.Left,
                    BaseColor = BaseColor
                };
                columns.Add(column);
                columnsPanels.Add(column);
            }

            ReAlign();
            columns.Reverse();
        }

        public override void ReAlignControls()
        {
            if (columns is not null)
                foreach (OxPane column in columns)
                    column.BringToFront();

            base.ReAlignControls();

            foreach (OxPane panel in placedPanels)
                panel.SendToBack();
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

        protected override void PrepareColors()
        {
            base.PrepareColors();

            if (columns is not null)
                foreach (OxPane column in columns)
                    column.BaseColor = BaseColor;
        }

        private void SetSizes()
        {
            Padding.TopInt = 18;
            Padding.BottomInt = 18;
        }

        private void PlacePanel(OxPane panel)
        {
            OxPane column = columns[(int)panel.Tag];
            panel.Parent = column;
            panel.Dock = OxDock.Top;
            columnsPanels[column].Add(panel);
        }

        public void LayoutPanels<T>(List<T> list)
            where T : OxPane
        {
            OxPaneList panelList = new();

            foreach (T item in list)
                panelList.Add(item);

            LayoutPanels(panelList);
        }

        public void LayoutPanels(OxPaneList panelList)
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
            ReAlign();
            SetPanelsVisible(true);
            RecalcColumnsSize();
            SetVisibleChangedHandlerToPanels();
        }

        private void InitPlacedPanels(OxPaneList panelList)
        {
            placedPanels.Clear();
            placedPanels.AddRange(panelList);
            placedPanels.Reverse();
        }

        private void PlacePanelsOnColumns()
        {
            SetPanelsColumnNumber();

            foreach (OxPane panel in placedPanels)
                PlacePanel(panel);
        }

        private void SetPanelsColumnNumber()
        {
            int columnNumber = GetLastPanelColumnNumber(placedPanels);

            foreach (OxPane panel in placedPanels)
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
            foreach (OxPane panel in placedPanels)
                panel.VisibleChanged += PanelVisibleChangedHander;
        }

        private void SetEmptySize()
        {
            StartSizeChanging();

            try
            {
                Size = new();
            }
            finally
            {
                EndSizeChanging();
            }
        }

        private OxDock GetColumnDock() =>
            panelsAlign is PanelsHorizontalAlign.OneColumn
                ? OxDock.Top
                : OxDock.Left;

        private void SetColumnsDock()
        {
            foreach (OxPane column in columns)
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
            int calcedWidth = 0;

            foreach (OxPane panel in placedPanels)
            {
                calcedWidth += OxWh.Int(panel.CalcedWidth);

                if (calcedWidth >= WidthInt)
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

        private int GetLastPanelColumnNumber(OxPaneList panelList)
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

                foreach (OxPane panel in placedPanels)
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
            foreach (OxPane column in columns)
                columnsPanels[column].Clear();
        }

        private OxWidth SetColumnSize(OxPane column)
        {
            OxWidth calcedHeight = OxWh.W0;
            OxWidth maxWidth = OxWh.W1;

            foreach (OxPane panel in columnsPanels[column])
                if (panel.Visible)
                {
                    calcedHeight |= panel.Height;
                    maxWidth = OxWh.Max(maxWidth, panel.CalcedWidth);
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

            foreach (OxPane column in columns)
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
            foreach (OxPane column in columns)
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
                    Padding.LeftInt = (WidthInt - GetSumColumnsSize().WidthInt) / 2;
                    break;
                case PanelsHorizontalAlign.OneColumn:
                    Padding.HorizontalInt = 0;
                    break;
            }
        }

        public virtual void Clear()
        {
            placedPanels.Clear();

            foreach (OxPaneList panelsList in columnsPanels.Values)
            {
                foreach (OxPane panel in panelsList)
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetLeftPadding();
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