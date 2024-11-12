using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxLibrary.Controls
{
    public delegate bool IsHighPriorityItem(object item);

    public interface IItemListControl : IComponent
    {
        object? SelectedItem { get; set; }
        int SelectedIndex { get; set; }

        void BeginUpdate();
        void EndUpdate();
        void ClearSelected();

        void UpdateItem(int index, object item);

        int Count { get; }

        public void RemoveAt(int index);

        Control? Parent { get; set; }

        event EventHandler? SelectedIndexChanged;
        event EventHandler DoubleClick;
        event EventHandler Click;
        event KeyEventHandler KeyUp;
        IsHighPriorityItem? CheckIsHighPriorityItem { get; set; }
        IsHighPriorityItem? CheckIsMandatoryItem { get; set; }

        void Add(object item);
        void Clear();

        List<object> ObjectList { get; }

        int Height { get; set; }

        void MoveUp();
        void MoveDown();
    }
}
