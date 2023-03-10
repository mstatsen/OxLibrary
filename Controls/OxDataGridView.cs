namespace OxLibrary.Controls
{
    public class OxDataGridView : DataGridView
    {
        protected override bool DoubleBuffered { get => true; }

        public readonly Dictionary<DataGridViewColumn, SortOrder> ColumnSorting = new();

        public event DataGridViewCellMouseEventHandler? SortingChanged;

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = Columns[e.ColumnIndex];

            if (column.SortMode != DataGridViewColumnSortMode.Programmatic)
            {
                base.OnColumnHeaderMouseClick(e);
                return;
            }

            bool existColumnSorting = true;

            if (!ColumnSorting.TryGetValue(column, out var oldSortOrder))
            {
                oldSortOrder = SortOrder.None;
                existColumnSorting = false;
            }

            if (ModifierKeys != Keys.Control)
            {
                ColumnSorting.Clear();

                foreach(DataGridViewColumn c in Columns)
                    c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            SortOrder newSortOrder =
                oldSortOrder switch
                {
                    SortOrder.Ascending => SortOrder.Descending,
                    SortOrder.Descending => SortOrder.None,
                    _ => SortOrder.Ascending,
                };

            if (newSortOrder == SortOrder.None && existColumnSorting)
                ColumnSorting.Remove(column);
            else
            if (existColumnSorting)
                ColumnSorting[column] = newSortOrder;
            else
                ColumnSorting.Add(column, newSortOrder);

            Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = newSortOrder;
            base.OnColumnHeaderMouseClick(e);
            SortingChanged?.Invoke(this, e);
        }
    }
}