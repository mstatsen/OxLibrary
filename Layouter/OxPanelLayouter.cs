using OxLibrary.Handlers;
using OxLibrary.ControlList;
using OxLibrary.Interfaces;
using OxLibrary.Geometry;

namespace OxLibrary.Panels
{
    public class OxPanelLayouter : OxPanel
    {
        private bool panelResizing = false;
        private const int MaximumColumnCount = 48;
        private int CalcedColumnCount = MaximumColumnCount;
        private readonly OxPanelsDictionary columnsPanels = new();
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
                foreach (IOxPanel column in columns)
                    column.BaseColor = BaseColor;
        }

        private void SetSizes()
        {
            Padding.Top = 18;
            Padding.Bottom = 18;
        }

        private void PlacePanel(IOxPanel panel)
        {
            if (panel.Tag is null)
                return;

            IOxPanel column = columns[(int)panel.Tag];
            panel.Parent = column;
            panel.Dock = OxDock.Top;
            columnsPanels[column].Add(panel);
        }

        public void LayoutPanels<T>(List<T> list)
            where T : IOxPanel
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
            SetPanelsVisible(OxB.F);
            PlacePanelsOnColumns();
            SetPanelsVisible(OxB.T);
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

            foreach (IOxPanel panel in placedPanels)
                PlacePanel(panel);
        }

        private void SetPanelsColumnNumber()
        {
            int columnNumber = GetLastPanelColumnNumber(placedPanels);

            foreach (IOxPanel panel in placedPanels)
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
            foreach (IOxPanel panel in placedPanels)
                panel.VisibleChanged += PanelVisibleChangedHander;
        }

        private void SetEmptySize()
        {
            WithSuspendedLayout(() =>
                {
                    Size = new();
                }
            );
        }

        private OxDock GetColumnDock() =>
            panelsAlign is OxPanelsHorizontalAlign.OneColumn
                ? OxDock.Top
                : OxDock.Left;

        private void SetColumnsDock()
        {
            foreach (IOxPanel column in columns)
                column.Dock = GetColumnDock();
        }

        private void PanelVisibleChangedHander(object? sender, EventArgs e)
        {
            if (sender is null)
                return;

            IOxPanel panel = (IOxPanel)sender;

            if (panel.IsVisible)
                panel.SizeChanged += SizeChangeHandler;
            else panel.SizeChanged -= SizeChangeHandler;
        }

        private void RecalcColumnsCount()
        {
            if (panelsAlign is OxPanelsHorizontalAlign.OneColumn)
            {
                CalcedColumnCount = 1;
                return;
            }

            CalcedColumnCount = 0;
            short calcedWidth = 0;

            foreach (IOxPanel panel in placedPanels)
            {
                calcedWidth += panel.Width;

                if (calcedWidth >= Width)
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

        private void ReLayoutPanelsHandler(object sender, OxSizeChangedEventArgs args)
        {
            if (!panelResizing)
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

        private void SetPanelsVisible(OxBool visible)
        {
            placedPanels.Reverse();

            try
            {
                int itemIndex = 0;

                foreach (IOxPanel panel in placedPanels)
                {
                    panel.SetVisible(
                        OxB.B(visible)
                        && (RealPlacedCount is -1
                            || itemIndex < RealPlacedCount));
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
            foreach (IOxPanel column in columns)
                columnsPanels[column].Clear();
        }

        private short SetColumnSize(IOxPanel column)
        {
            short calcedHeight = 0;
            short maxWidth = 1;

            foreach (IOxPanel panel in columnsPanels[column])
                if (panel.IsVisible)
                {
                    calcedHeight += panel.Height;
                    maxWidth = OxSh.Max(maxWidth, panel.Width);
                }

            if (panelsAlign is OxPanelsHorizontalAlign.OneColumn)
                calcedHeight += 48;

            column.Size = new(maxWidth, calcedHeight);
            return calcedHeight;
        }

        private void SizeChangeHandler(object sender, OxSizeChangedEventArgs args) =>
            RecalcColumnsSize();

        private void RecalcColumnsSize()
        {
            if (!IsVisible)
                return;

            panelResizing = true;

            try
            {
                SetColumnsSize();
                OxSize columnsSize = GetSumColumnsSize();
                columnsSize.Height += OxSh.Add(Padding.Top, Padding.Bottom);
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

            short sumWidth = 0;
            short maxColumnHeight = 0;

            foreach (IOxPanel column in columns)
            {
                short columnHeight = SetColumnSize(column);
                maxColumnHeight = Math.Max(maxColumnHeight, columnHeight);

                if (columnsPanels[column].Count is not 0)
                    sumWidth += column.Width;
            }

            return new(sumWidth, maxColumnHeight);
        }

        private void SetColumnsSize()
        {
            foreach (IOxPanel column in columns)
                SetColumnSize(column);
        }

        private void SetLeftPadding()
        {
            switch (panelsAlign)
            {
                case OxPanelsHorizontalAlign.Left:
                    Padding.Left = 0;
                    break;
                case OxPanelsHorizontalAlign.Center:
                case OxPanelsHorizontalAlign.Right:
                    Padding.Left = OxSh.CenterOffset(Width, GetSumColumnsSize().Width);
                    break;
                case OxPanelsHorizontalAlign.OneColumn:
                    Padding.Horizontal = 0;
                    break;
            }
        }

        public virtual void Clear()
        {
            placedPanels.Clear();

            foreach (OxPanelList panelsList in columnsPanels.Values)
            {
                foreach (IOxPanel panel in panelsList)
                {
                    panel.SizeChanged -= SizeChangeHandler;
                    panel.SetVisible(false);
                    panel.Parent = null;
                }

                panelsList.Clear();
            }

            Size = OxSize.Empty;
            UpdateParent();
        }

        public override void OnSizeChanged(OxSizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.IsChanged)
                SetLeftPadding();
        }

        public OxPanelsHorizontalAlign PanelsAlign
        {
            get => panelsAlign;
            set => SetPanelsAlign(value);
        }

        private void SetPanelsAlign(OxPanelsHorizontalAlign value)
        {
            panelsAlign = value;
            SetTopPadding();
            SetLeftPadding();
        }

        private void SetTopPadding() => 
            Padding.Top = 
                OxSh.Short(
                    panelsAlign is OxPanelsHorizontalAlign.OneColumn
                        ? 16
                        : 18
                );

        protected void UpdateParent() =>
            Parent?.Update();

        private OxPanelsHorizontalAlign panelsAlign = OxPanelsHorizontalAlign.Center;

        public override Color DefaultColor => Color.GhostWhite;

        public override void OnVisibleChanged(OxBoolChangedEventArgs e)
        {
            base.OnVisibleChanged(e);
            SetPanelsVisible(Visible);
            RecalcColumnsSize();
        }
    }
}