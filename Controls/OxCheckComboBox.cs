using System.Windows.Forms.VisualStyles;

namespace OxLibrary.Controls
{
    public class OxCheckComboBox<T> : OxComboBox<T>
    {
        public event EventHandler? CheckChanged;

        public OxCheckComboBox() : base()
        {
            Items = new OxCheckDataList<T>(base.Items);
            Items.CheckChanged += ItemCheckChangedHanler;
            DrawStrings = false;
        }

        private void ItemCheckChangedHanler(object? sender, EventArgs e)
        {
            Invalidate();
            CheckChanged?.Invoke(sender, e);
        }

        public new OxCheckDataList<T> Items;

        public OxCheckData<T>? SelectedCheckItem
        {
            get => (OxCheckData<T>?)SelectedItemObject;
            set => SelectedItemObject = value;
        }

        public new T SelectedItem
        {
            get => SelectedCheckItem != null ? SelectedCheckItem.Data : default!;
            set => SelectedCheckItem = Items.CheckItem(value);
        }

        public List<T>? CheckedList
        {
            get => Items.CheckedList;
            set => Items.CheckedList = value;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (SelectedCheckItem != null)
                SelectedCheckItem.Checked = !SelectedCheckItem.Checked;

            CheckChanged?.Invoke(SelectedCheckItem, e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index < 0)
                return;

            CheckBoxRenderer.RenderMatchingApplicationState = true;
            CheckBoxRenderer.DrawCheckBox(
                e.Graphics,
                new Point(e.Bounds.X, e.Bounds.Y + 5),
                e.Bounds,
                Items[e.Index].ToString(), 
                Font,
                TextFormatFlags.Left,
                false,
                Items[e.Index].Checked 
                    ? CheckBoxState.CheckedNormal 
                    : CheckBoxState.UncheckedNormal
            );
        }
    }

    public class OxCheckComboBox : OxCheckComboBox<object>
    { 
    }    
}