using System.ComponentModel;

namespace OxLibrary.Controls
{
    public delegate bool IsHighPriorityItem(object item);

    public interface IItemsContainer : IComponent
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
        void AddChild(object item);
        void Clear();

        void ExpandAll();

        List<object> ObjectList { get; }

        int Height { get; set; }

        void MoveUp();
        void MoveDown();

        bool AvailableMoveUp { get; }
        bool AvailableMoveDown { get; }

        DockStyle Dock { get; set; }

        BorderStyle BorderStyle { get; set; }
        Control AsControl();

        bool AvailableChilds { get; }
    }
}
