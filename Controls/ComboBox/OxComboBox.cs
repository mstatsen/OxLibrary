namespace OxLibrary.Controls;

public delegate void GetToolTip<T>(T item, out string toolTipTitle, out string toolTipText);
public class OxComboBox<T> : ComboBox
{
    public OxComboBox()
    {
        DoubleBuffered = true;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawFixed;
    }

    private readonly ToolTip ToolTip = new()
    {
        AutomaticDelay = 500,
        ShowAlways = true
    };

    public event GetToolTip<T>? GetToolTip;

    protected virtual void OnGetToolTip(T item, out string toolTipTitle, out string toolTipText)
    { 
        toolTipTitle = string.Empty;
        toolTipText = string.Empty;
        GetToolTip?.Invoke(item, out toolTipTitle, out toolTipText);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        ToolTip.Hide(this);
    }

    protected override void OnDropDownClosed(EventArgs e)
    {
        base.OnDropDownClosed(e);
        ToolTip.Hide(this);
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedItemChanged(e);

        string toolTipTitle = string.Empty;
        string toolTipText = string.Empty;

        if (SelectedItem is not null)
            OnGetToolTip(SelectedItem, out toolTipTitle, out toolTipText);

        ToolTip.ToolTipTitle = toolTipTitle;
        ToolTip.SetToolTip(this, toolTipText);
    }

    protected bool DrawStrings { get; set; } = true;

    private readonly List<T> tItems = new();

    public new T? SelectedItem
    {
        get => (T?)base.SelectedItem;
        set => base.SelectedItem = value;
    }

    public object? SelectedItemObject
    {
        get => base.SelectedItem;
        set => base.SelectedItem = value;
    }

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

    protected virtual Point GetTextStartPosition(Rectangle bounds) => 
        new(bounds.Left, bounds.Top);

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        Color BrushColor =
            ((e.State & DrawItemState.Selected) is DrawItemState.Selected)
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

            Point textStartPosition = GetTextStartPosition(e.Bounds);

            e.Graphics.DrawString(itemString,
                e.Font ?? OxStyles.Font(10),
                new SolidBrush(Color.Black),
                new Point(textStartPosition.X, textStartPosition.Y));

            if (DroppedDown && (e.State & DrawItemState.Selected) is DrawItemState.Selected)
            {
                OnGetToolTip(item, out string toolTipTitle, out string toolTipText);
                ToolTip.ToolTipTitle = toolTipTitle;

                if (!toolTipTitle.Equals(string.Empty) 
                    || !toolTipText.Equals(string.Empty))
                    ToolTip.Show(toolTipText, this, e.Bounds.Right, e.Bounds.Bottom);
                else
                    ToolTip.Hide(this);
            }
            else
                ToolTip.Hide(this);
        }
        else ToolTip.Hide(this);
    }

    protected override void OnDropDownStyleChanged(EventArgs e)
    {
        base.OnDropDownStyleChanged(e);
        FlatStyle = FlatStyle.System;
        AutoCompleteMode = DropDownStyle is ComboBoxStyle.DropDownList
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

                if (!hoveredItemIndex.Equals(currentHoveredItemIndex))
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