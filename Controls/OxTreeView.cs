namespace OxLibrary.Controls
{
    public class OxTreeView : TreeView
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

            if (e.Node == null)
                return;

            FontStyle fontStyle = e.Node.IsSelected
                ? FontStyle.Bold
                : FontStyle.Regular;

            e.Graphics.DrawRectangle(
                e.Node.IsSelected ? SelectedPen : StandardPen, 
                e.Bounds);
            e.Graphics.FillRectangle(
                e.Node.IsSelected ? SelectedBrush : StandardBrush, 
                e.Bounds);
            Brush textBrush = Enabled ? Brushes.Black : Brushes.Silver;
            Font nodeFont = new(Font, fontStyle);

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

        private readonly Bitmap expandedIcon = new(OxIcons.down, new Size(22, 22));
        private readonly Bitmap collapsedIcon = new(OxIcons.right, new Size(18, 18));
        private Brush StandardBrush = default!;
        private Brush SelectedBrush = default!;
        private Pen StandardPen = default!;
        private Pen SelectedPen = default!;
    }

    public class CountedTreeNode : TreeNode
    {
        private int count;

        public CountedTreeNode(string text) : base(text) { }

        public int Count 
        { 
            get => count;
            set
            {
                int oldCount = count;
                count = value;

                if (oldCount != count)
                    TreeView?.Invalidate();
            }
        }
    }
}