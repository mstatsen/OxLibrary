namespace OxLibrary.Controls
{
    public delegate bool IsHighPriorityItem(object item);
    public class OxListBox : ListBox
    {
        public IsHighPriorityItem? CheckIsHighPriorityItem;
        public IsHighPriorityItem? CheckIsMandatoryItem;

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

        private enum MoveDirection
        {
            Up,
            Down
        };

        private static int MoveDirectionDelta(MoveDirection direction) => 
            direction switch
            {
                MoveDirection.Up => -1,
                MoveDirection.Down => 1,
                _ => 0,
            };

        private void MoveItem(MoveDirection direction)
        {
            if (SelectedItem == null || SelectedIndex < 0)
                return; 

            int newIndex = SelectedIndex + MoveDirectionDelta(direction);

            if (newIndex < 0 || newIndex >= Items.Count)
                return; 

            object selected = SelectedItem;

            Items.Remove(selected);
            Items.Insert(newIndex, selected);
            SetSelected(newIndex, true);
        }

        public void MoveUp() => MoveItem(MoveDirection.Up);
        public void MoveDown() => MoveItem(MoveDirection.Down);
    }
}