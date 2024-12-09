using System.ComponentModel;

namespace OxLibrary.Interfaces;

public delegate bool IsHighPriorityItem(object item);

//TODO: replace inherits from IComonent with IOxControlContainer
public interface IOxItemsContainer : IComponent
{
    object? SelectedItem { get; set; }
    int SelectedIndex { get; set; }
    void BeginUpdate();
    void EndUpdate();
    void ClearSelected();
    void UpdateSelectedItem(object item);
    int Count { get; }
    public void RemoveAt(int index);
    public void RemoveCurrent();
    Control? Parent { get; set; }
    IsHighPriorityItem? CheckIsHighPriorityItem { get; set; }
    IsHighPriorityItem? CheckIsMandatoryItem { get; set; }
    void Add(object item);
    void AddChild(object item);
    void Clear();
    List<object> ObjectList { get; }
    int Height { get; set; }
    void MoveUp();
    void MoveDown();
    bool AvailableMoveUp { get; }
    bool AvailableMoveDown { get; }
    DockStyle Dock { get; set; }
    BorderStyle BorderStyle { get; set; }
    IOxControl AsControl();
    bool AvailableChilds { get; }
    event EventHandler Click;
    event EventHandler DoubleClick;
    event KeyEventHandler KeyUp;
    event EventHandler? SelectedIndexChanged;
}