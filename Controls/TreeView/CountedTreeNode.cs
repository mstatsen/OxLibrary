namespace OxLibrary.Controls;

public class CountedTreeNode : TreeNode
{
    private int count;

    public new OxTreeView TreeView => (OxTreeView)base.TreeView;

    public CountedTreeNode(string text) : base(text) { }

    public int Count 
    { 
        get => count;
        set
        {
            int oldCount = count;
            count = value;

            if (TreeView is not null
                && !oldCount.Equals(count)
                && !TreeView.Loading)
                TreeView?.Invalidate();
        }
    }
}