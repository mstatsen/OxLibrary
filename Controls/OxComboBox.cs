namespace OxLibrary.Controls
{
    public class OxComboBox<T> : ComboBox
    {
        public OxComboBox()
        {
            DoubleBuffered = true;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected bool DrawStrings { get; set; } = true;

        private readonly List<T> tItems = new();

        public List<T> TItems
        {
            get
            {
                tItems.Clear();

                foreach (object item in Items)
                    tItems.Add((T)item);

                return tItems;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Color BrushColor =
                ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    ? new OxColorHelper(BackColor).Darker(2)
                    : BackColor;

            e.Graphics.DrawRectangle(new Pen(BrushColor), e.Bounds);
            e.Graphics.FillRectangle(new SolidBrush(BrushColor), e.Bounds);

            if (DrawStrings && e.Index > -1)
            {
                T item = TItems[e.Index];
                string? itemString =
                    item is bool boolItem 
                    ? boolItem
                        ? "Yes" 
                        : "No" 
                    : item!.ToString();

                e.Graphics.DrawString(itemString,
                    e.Font ?? new Font("Calibri Light", 10),
                    new SolidBrush(Color.Black),
                    new Point(e.Bounds.X, e.Bounds.Y));
            }
        }

        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);
            FlatStyle = FlatStyle.System;
            AutoCompleteMode = DropDownStyle == ComboBoxStyle.DropDownList
                ? AutoCompleteMode.None
                : AutoCompleteMode.Suggest;
        }

        private int hoveredItemIndex = -1;
        private const int CB_GETCURSEL = 0x0147;
        public event EventHandler<HoverItemEventArgs>? HoverItemChanged;

        protected virtual void OnHoverItem(HoverItemEventArgs e) => 
            HoverItemChanged?.Invoke(this, e);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case CB_GETCURSEL:
                    int currentHoveredItemIndex = m.Result.ToInt32();

                    if (hoveredItemIndex != currentHoveredItemIndex)
                    {
                        hoveredItemIndex = currentHoveredItemIndex;
                        OnHoverItem(
                            new HoverItemEventArgs(
                                hoveredItemIndex,
                                hoveredItemIndex < 0 ? default : TItems[hoveredItemIndex])
                        );
                    }
                    break;
                default:
                    // Add Case switches to handle other events
                    break;
            }
        }

        public class HoverItemEventArgs : EventArgs
        {
            public HoverItemEventArgs(int idx, T? hoveredItem)
            {
                HoveredItemIndex = idx;
                HoveredItem = hoveredItem;
            }
            public int HoveredItemIndex { get; }
            public T? HoveredItem { get; }
        }
    }

    public class OxComboBox : OxComboBox<object>
    { 
    }
}