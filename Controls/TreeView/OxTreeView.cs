namespace OxLibrary.Controls
{
    public class OxTreeView : TreeView, IItemsContainer
    {
        public OxTreeView() : base()
        {
            FullRowSelect = true;
            ShowRootLines = false;
            ShowLines = false;
            HideSelection = false;
            BorderStyle = BorderStyle.None;
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            ItemHeight = 26;
        }

        public bool DoubleClickExpand { get; set; } = true;
        public bool AllowCollapse { get; set; } = true;

        private bool IsDoubleClick = false;

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            base.OnBeforeCollapse(e);

            if (e.Action is TreeViewAction.Collapse)
                e.Cancel |= !AllowCollapse 
                    || (!DoubleClickExpand 
                        && IsDoubleClick);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            if (e.Action is TreeViewAction.Expand)
                e.Cancel |= !DoubleClickExpand 
                    && IsDoubleClick;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            IsDoubleClick = e.Clicks > 1;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Color selectedBrushColor = new OxColorHelper(BackColor).Darker(2);
            StandardBrush = new SolidBrush(BackColor);
            SelectedBrush = new SolidBrush(selectedBrushColor);
            StandardPen = new Pen(BackColor);
            SelectedPen = new Pen(selectedBrushColor);
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            base.OnDrawNode(e);

            if (e.Node is null)
                return;

            e.Graphics.DrawRectangle(
                e.Node.IsSelected ? SelectedPen : StandardPen, 
                e.Bounds);
            e.Graphics.FillRectangle(
                e.Node.IsSelected ? SelectedBrush : StandardBrush, 
                e.Bounds);
            Brush textBrush = Enabled ? Brushes.Black : Brushes.Silver;
            Font nodeFont = new(Font, e.Node.IsSelected ? FontStyle.Bold : FontStyle.Regular);

            int textTop = e.Bounds.Y 
                + (e.Bounds.Height - TextRenderer.MeasureText(e.Node.Text, nodeFont).Height) / 2;
            int textLeft = e.Bounds.X + (e.Node.Level + 1) * 24;

            if (e.Node.Nodes.Count > 0)
            {
                Bitmap icon =
                    e.Node.IsExpanded
                        ? expandedIcon
                        : collapsedIcon;

                e.Graphics.DrawImage(
                    icon,
                    textLeft - icon.Width + 2,
                    e.Bounds.Y + (e.Bounds.Height - icon.Height) / 2);
            }

            e.Graphics.DrawString(e.Node.Text,
                nodeFont,
                textBrush,
                new Point(textLeft, textTop));

            if (e.Node is CountedTreeNode countedTreeNode)
            {
                string countString = countedTreeNode.Count.ToString();
                Font countFont = new(
                    nodeFont.FontFamily, 
                    nodeFont.Size - 2, 
                    nodeFont.Style | FontStyle.Italic);
                Size countTextSize = TextRenderer.MeasureText(countString, countFont);
                e.Graphics.DrawString(countString,
                    countFont,
                    textBrush,
                    new Point(
                        e.Bounds.Width - countTextSize.Width - 1,
                        e.Bounds.Y + (e.Bounds.Height - countTextSize.Height) / 2));
            }
        }

        private readonly Bitmap expandedIcon = new(OxIcons.Down, new(22, 22));
        private readonly Bitmap collapsedIcon = new(OxIcons.Right, new(18, 18));
        private Brush StandardBrush = default!;
        private Brush SelectedBrush = default!;
        private Pen StandardPen = default!;
        private Pen SelectedPen = default!;

        private bool loading = false;

        public event EventHandler? SelectedIndexChanged;

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            SelectedIndexChanged?.Invoke(this, e);
        }

        public bool Loading 
        { 
            get => loading;
            set
            {
                if (!loading.Equals(value))
                {
                    loading = value;

                    if (!loading)
                        Invalidate();
                }
            }
        }

        public object? SelectedItem
        {
            get => SelectedNode?.Tag;
            set => SelectedNode = GetNodeByTag(value);
        }
        public int SelectedIndex 
        {
            get => 
                SelectedNode is null 
                    ? -1 
                    : SelectedNode.Index;
            set => SelectedNode = Nodes[value];
        }

        public int Count => 
            Nodes.Count;

        public IsHighPriorityItem? CheckIsHighPriorityItem 
        { 
            get => null;
            set { }
        }
        public IsHighPriorityItem? CheckIsMandatoryItem 
        {
            get => null;
            set { }
        }

        public List<object> ObjectList
        {
            get
            {
                List<object> result = new();

                foreach (TreeNode node in Nodes)
                    result.Add(node.Tag);

                return result;
            }
        }

        public TreeNode? GetNodeByTag(object? tag, TreeNodeCollection? treeNodes = null)
        {
            if (tag is null)
                return Nodes.Count > 0 
                    ? Nodes[0] 
                    : null;

            treeNodes ??= Nodes;

            foreach (TreeNode node in treeNodes)
            {
                if (node.Tag.Equals(tag))
                    return node;

                if (node.Nodes.Count is 0)
                    continue;

                TreeNode? findNode = GetNodeByTag(tag, node.Nodes);

                if (findNode is not null)
                    return findNode;
            }

            return null;
        }

        public void ClearSelected() => 
            SelectedNode = null;

        public void UpdateSelectedItem(object item) => 
            SelectedNode.Tag = item;

        public void RemoveAt(int index) => 
            Nodes.RemoveAt(index);

        public void RemoveCurrent() => 
            SelectedNode.Remove();

        public void Add(object item) => Nodes.Add(item.ToString()).Tag = item;

        public bool AvailableMoveUp =>
            SelectedNode is not null
            && (SelectedNode.Parent is null
                ? SelectedIndex > 0
                : SelectedNode.Index > 0);

        public bool AvailableMoveDown =>
            SelectedNode is not null
            && (SelectedNode.Parent is null
                ? SelectedIndex > -1 && SelectedIndex < Count - 1
                : SelectedNode.Index < SelectedNode.Parent.Nodes.Count - 1);

        public void Clear() => Nodes.Clear();

        private void MoveNode(MoveDirection direction)
        {
            if (SelectedNode is null)
                return;

            TreeNodeCollection parentNodes = 
                SelectedNode.Parent is not null 
                    ? SelectedNode.Parent.Nodes 
                    : Nodes;

            int newIndex = SelectedIndex + MoveDirectionHelper.Delta(direction);

            if (newIndex < 0 || newIndex >= parentNodes.Count)
                return;

            TreeNode node = SelectedNode;
            parentNodes.Remove(node);
            parentNodes.Insert(newIndex, node);
            SelectedNode = node;
        }

        public void MoveUp() => MoveNode(MoveDirection.Up);

        public void MoveDown() => MoveNode(MoveDirection.Down);

        public Control AsControl() => this;

        public bool AvailableChilds => true;

        public void AddChild(object item)
        {
            TreeNode childNode = new(item.ToString())
            {
                Tag = item
            };
            SelectedNode.Nodes.Add(childNode);
            SelectedNode = childNode;
        }
    }

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
}