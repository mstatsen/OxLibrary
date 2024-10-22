namespace OxLibrary.Panels
{
    public class OxPanelLayouter : OxPanel
    {
        private bool panelResizing = false;
        private const int MaximumColumnCount = 24;
        private int CalcedColumnCount = MaximumColumnCount;
        private readonly OxPaneDictionary columnsPanels = new();
        private readonly OxPaneList columns = new();
        private readonly OxPaneList placedPanels = new();

        public bool IsEmpty => placedPanels.Count == 0;

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
                    Parent = ContentContainer,
                    Dock = DockStyle.Left,
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
            if (columns != null)
                foreach (IOxPane column in columns)
                    column.BringToFront();

            base.ReAlignControls();

            foreach (IOxPane panel in placedPanels)
                panel.SendToBack();
        }

        public OxPanelLayouter() : base()
        {
            Dock = DockStyle.Top;
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

            if (columns != null)
                foreach (IOxPane column in columns)
                    column.BaseColor = BaseColor;
        }

        private void SetSizes()
        {
            Paddings.Top = 18;
            Paddings.Bottom = 18;
        }

        private void PlacePanel(IOxPane panel)
        {
            IOxPane column = columns[(int)panel.Tag];
            panel.Parent = (Control)column;
            panel.Dock = DockStyle.Top;
            columnsPanels[column].Add(panel);
        }


        public void LayoutPanels<T>(List<T> list)
            where T : IOxPane
        {
            OxPaneList panelList = new();

            foreach (T item in list)
                panelList.Add(item);

            LayoutPanels(panelList);
        }

        public void LayoutPanels(OxPaneList panelList)
        {
            if (panelList.Count == 0)
            {
                SetEmptySize();
                return;
            }

            int oldCalcColumnsCount = IsEmpty ? -1 : CalcedColumnCount;

            if (panelList != placedPanels)
                InitPlacedPanels(panelList);

            RecalcColumnsCount();

            if (oldCalcColumnsCount == CalcedColumnCount)
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

            foreach (IOxPane panel in placedPanels)
                PlacePanel(panel);
        }

        private void SetPanelsColumnNumber()
        {
            int columnNumber = GetLastPanelColumnNumber(placedPanels);

            foreach (IOxPane panel in placedPanels)
            {
                panel.Tag = columnNumber;
                columnNumber = GetNextColumnNumber(columnNumber);
            }
        }

        private int GetNextColumnNumber(int columnNumber)
        {
            columnNumber--;

            if (columnNumber == columns.Count)
                columnNumber = 0;

            if (columnNumber < 0)
                columnNumber = CalcedColumnCount - 1;

            return columnNumber;
        }

        private void SetVisibleChangedHandlerToPanels()
        {
            foreach (IOxPane panel in placedPanels)
                panel.VisibleChanged += PanelVisibleChangedHander;
        }

        private void SetEmptySize()
        {
            StartSizeRecalcing();

            try
            {
                SetContentSize(0, 0);
            }
            finally
            {
                EndSizeRecalcing();
            }
        }

        private void SetColumnsDock()
        {
            foreach (IOxPane column in columns)
                column.Dock = panelsAlign == PanelsHorizontalAlign.OneColumn
                    ? DockStyle.Top
                    : DockStyle.Left;
        }

        private void PanelVisibleChangedHander(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            Control panel = (Control)sender;

            if (panel.Visible)
                panel.SizeChanged += SizeChangeHandler;
            else panel.SizeChanged -= SizeChangeHandler;
        }

        private void RecalcColumnsCount()
        {
            if (panelsAlign == PanelsHorizontalAlign.OneColumn)
            {
                CalcedColumnCount = 1;
                return;
            }

            CalcedColumnCount = 0;
            int calcedWidth = 0;

            foreach (IOxPane panel in placedPanels)
            {
                calcedWidth += panel.CalcedWidth;

                if (calcedWidth >= Width)
                    break;
                else
                    CalcedColumnCount++;
            }

            if (CalcedColumnCount == 0)
                CalcedColumnCount++;
        }

        private void ReLayoutPanelsHandler(object? sender, EventArgs e)
        {
            if (!SizeRecalcing && !panelResizing)
                LayoutPanels(placedPanels);
        }

        private int GetLastPanelColumnNumber(OxPaneList panelList)
        {
            int columnNumber = CalcedColumnCount - 1;
            int panelsCount = panelList.Count;

            if (panelsCount % CalcedColumnCount != 0)
                columnNumber = panelsCount % CalcedColumnCount - 1;

            if (columnNumber == columns.Count)
                columnNumber = 0;

            return columnNumber;
        }

        private void SetPanelsVisible(bool visible)
        {
            placedPanels.Reverse();

            try
            {
                int itemIndex = 0;

                foreach (IOxPane panel in placedPanels)
                {
                    panel.Visible = visible && (RealPlacedCount == -1 || itemIndex < RealPlacedCount);
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
            foreach (IOxPane column in columns)
                columnsPanels[column].Clear();
        }

        private int SetColumnSize(IOxPane column)
        {
            int calcedHeight = 0;
            int maxWidth = 1;

            foreach (IOxPane panel in columnsPanels[column])
                if (panel.Visible)
                {
                    calcedHeight += panel.Height;
                    maxWidth = Math.Max(maxWidth, panel.CalcedWidth);
                }

            if (panelsAlign == PanelsHorizontalAlign.OneColumn)
                calcedHeight += 48;

            column.SetContentSize(maxWidth, calcedHeight);
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
                Size columnsSize = GetSumColumnsSize();
                columnsSize.Height +=
                    Paddings.CalcedSize(OxDock.Top)
                    + Paddings.CalcedSize(OxDock.Bottom);
                SetContentSize(columnsSize);
            }
            finally
            {
                panelResizing = false;
            }
        }

        private Size GetSumColumnsSize()
        {
            if (columns == null)
                return Size.Empty;

            int sumWidth = 0;
            int maxColumnHeight = 0;

            foreach (IOxPane column in columns)
            {
                int columnHeight = SetColumnSize(column);
                maxColumnHeight = Math.Max(maxColumnHeight, columnHeight);

                if (columnsPanels[column].Count != 0)
                    sumWidth += column.Width;
            }

            return new Size(sumWidth, maxColumnHeight);
        }

        private void SetColumnsSize()
        {
            foreach (IOxPane column in columns)
                SetColumnSize(column);
        }

        private void SetLeftPadding()
        {
            switch (panelsAlign)
            {
                case PanelsHorizontalAlign.Left:
                    Paddings.Left = 0;
                    break;
                case PanelsHorizontalAlign.Center:
                case PanelsHorizontalAlign.Right:
                    Paddings.Left = (Width - GetSumColumnsSize().Width) / 2;
                    break;
                case PanelsHorizontalAlign.OneColumn:
                    Paddings.Horizontal = 0;
                    break;
            }
        }

        public virtual void Clear()
        {
            placedPanels.Clear();

            foreach (OxPaneList panelsList in columnsPanels.Values)
            {
                foreach (OxPanel panel in panelsList.Cast<OxPanel>())
                {
                    panel.SizeChanged -= SizeChangeHandler;
                    panel.Visible = false;
                    panel.Parent = null;
                }
                panelsList.Clear();
            }

            SetContentSize(0, 0);
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
            Paddings.Top =
                panelsAlign == PanelsHorizontalAlign.OneColumn
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