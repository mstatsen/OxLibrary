
namespace OxLibrary.Controls
{
    public partial class OxListBox : ListBox, IItemsContainer
    {
        private IsHighPriorityItem? checkIsHighPriorityItem;
        private IsHighPriorityItem? checkIsMandatoryItem;

        public IsHighPriorityItem? CheckIsHighPriorityItem 
        { 
            get => checkIsHighPriorityItem;
            set => checkIsHighPriorityItem = value;
        }

        public IsHighPriorityItem? CheckIsMandatoryItem
        { 
            get => checkIsMandatoryItem;
            set => checkIsMandatoryItem = value;
        }
            
        private bool IsHighPriorityItem(object item) =>
            CheckIsHighPriorityItem == null 
            || CheckIsHighPriorityItem.Invoke(item);

        private bool IsMandatoryItem(object item) =>
            CheckIsMandatoryItem == null
            || CheckIsMandatoryItem.Invoke(item);

        public OxListBox()
        {
            DoubleBuffered = true;
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += DrawItemHadler;
            ItemHeight = 28;
        }

        private void DrawItemHadler(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            FontStyle fontStyle = FontStyle.Regular;

            if (IsHighPriorityItem(Items[e.Index]))
                fontStyle |= FontStyle.Underline;

            if (IsMandatoryItem(Items[e.Index]))
                fontStyle |= FontStyle.Bold;

            Font itemFont = new(e.Font ?? Styles.Font(11), fontStyle);

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(
                    e.Graphics,
                    itemFont,
                    e.Bounds,
                    e.Index,
                    e.State ^ DrawItemState.Selected,
                    e.ForeColor,
                    new OxColorHelper(BackColor).Darker(2));

            e.DrawBackground();
            Rectangle textBounds = new(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            textBounds.X += 2;
            textBounds.Y +=
                (textBounds.Height -
                TextRenderer.MeasureText(Items[e.Index].ToString(), e.Font).Height) / 2;

            string? itemText = Items[e.Index].ToString();

            e.Graphics.DrawString(
                itemText,
                itemFont,
                Enabled ? Brushes.Black : Brushes.Silver,
                textBounds, 
                StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void MoveItem(MoveDirection direction)
        {
            if (SelectedItem == null || SelectedIndex < 0)
                return; 

            int newIndex = SelectedIndex + MoveDirectionHelper.Delta(direction);

            if (newIndex < 0 || newIndex >= Items.Count)
                return; 

            object selected = SelectedItem;

            Items.Remove(selected);
            Items.Insert(newIndex, selected);
            SetSelected(newIndex, true);
        }

        public void MoveUp() => MoveItem(MoveDirection.Up);
        public void MoveDown() => MoveItem(MoveDirection.Down);

        public void UpdateSelectedItem(object item) =>
            Items[SelectedIndex] = item;

        public int Count => Items.Count;

        public List<object> ObjectList
        {
            get
            {
                List<object> result = new();

                foreach (var item in Items)
                    result.Add(item);

                return result;
            }
        }

        public bool AvailableMoveUp => SelectedIndex > 0;

        public bool AvailableMoveDown => (SelectedIndex > -1)
                && (SelectedIndex < Count - 1);

        public void RemoveAt(int index) => Items.RemoveAt(index);
        public void RemoveCurrent() => Items.RemoveAt(SelectedIndex);

        public void Add(object item) => Items.Add(item);
        public void AddChild(object item) => Items.Add(item);
        public void Clear() => Items.Clear();

        public Control AsControl() => this;

        public bool AvailableChilds => false;
    }
}