namespace OxLibrary.Controls
{
    public class OxCheckData<T>
    {
        public event EventHandler? CheckChanged;

        private bool _checked = false;
        public bool Checked 
        { 
            get => _checked;
            set
            {
                _checked = value;
                CheckChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public T Data { get; set; }

        public OxCheckData(T data) =>
            Data = data;

        public override string? ToString() => Data?.ToString();
    }

    public class OxCheckDataList<T> : List<OxCheckData<T>>
    {
        public event EventHandler? CheckChanged;
        private readonly ComboBox.ObjectCollection Items;

        public OxCheckDataList(ComboBox.ObjectCollection items) =>
            Items = items;

        public new OxCheckData<T> Add(OxCheckData<T> item)
        {
            base.Add(item);
            Items.Add(item);
            item.CheckChanged += CheckChanged;
            return item;
        }

        public List<T>? CheckedList
        {
            get
            {
                List<T> checkedItems = new();

                foreach (OxCheckData<T> checkedItem in FindAll(c => c.Checked))
                    checkedItems.Add(checkedItem.Data);

                return checkedItems;
            }
            set
            {
                foreach (OxCheckData<T> checkedItem in this)
                    checkedItem.Checked = value != null && value.Contains(checkedItem.Data);
            }
        }

        public T? Item(OxCheckData<T>? checkItem)
        {
            OxCheckData<T>? item = Find(c => c.Equals(checkItem));
            return item != null ? item.Data : default;
        }

        public OxCheckData<T>? CheckItem(T? item) =>
            Find(c => item != null && item.Equals(c.Data));

        public OxCheckData<T> Add(T item) =>
            Add(new OxCheckData<T>(item));

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                Add(item);
        }

        public new void Remove(OxCheckData<T>? item)
        {
            if (item == null)
                return;

            base.Remove(item);
            item.CheckChanged -= CheckChanged;
            Items.Remove(item);
        }

        public void Remove(T item) => Remove(CheckItem(item));

        public new void Clear()
        {
            foreach (OxCheckData<T> item in this)
                item.CheckChanged -= CheckChanged;

            base.Clear();
            Items.Clear();
        }

        public new OxCheckData<T>? Insert(int index, OxCheckData<T>? item)
        {
            if (item == null)
                return null;

            base.Insert(index, item);
            Items.Insert(index, item);
            item.CheckChanged += CheckChanged;
            return item;
        }

        public OxCheckData<T>? Insert(int index, T item) => Insert(index, CheckItem(item));

        public new void InsertRange(int index, IEnumerable<OxCheckData<T>> collection) { }

        public new int RemoveAll(Predicate<OxCheckData<T>> match)
        {
            foreach (OxCheckData<T> item in this)
                item.CheckChanged -= CheckChanged;

            int removed = base.RemoveAll(match);

            foreach (OxCheckData<T> item in this)
                item.CheckChanged += CheckChanged;

            RenumerateItems();
            return removed;
        }

        private void RenumerateItems()
        {
            Items.Clear();

            foreach (OxCheckData<T> data in this)
                Items.Add(data);
        }

        public new void RemoveAt(int index)
        {
            this[index].CheckChanged -= CheckChanged;
            base.RemoveAt(index);
            Items.RemoveAt(index);
        }

        public new void RemoveRange(int index, int count)
        {
            foreach (OxCheckData<T> item in this)
                item.CheckChanged -= CheckChanged;

            base.RemoveRange(index, count);
            RenumerateItems();

            foreach (OxCheckData<T> item in this)
                item.CheckChanged += CheckChanged;
        }

        public new void Reverse()
        {
            base.Reverse();
            RenumerateItems();
        }

        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            RenumerateItems();
        }

        public new void Sort()
        {
            base.Sort();
            RenumerateItems();
        }

        public new void Sort(IComparer<OxCheckData<T>> comparer)
        {
            base.Sort(comparer);
            RenumerateItems();
        }

        public new void Sort(int index, int count, IComparer<OxCheckData<T>> comparer)
        {
            base.Sort(index, count, comparer);
            RenumerateItems();
        }

        public new void Sort(Comparison<OxCheckData<T>> comparison)
        {
            base.Sort(comparison);
            RenumerateItems();
        }
    }
}