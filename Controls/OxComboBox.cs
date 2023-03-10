namespace OxLibrary.Controls
{
    public class OxComboBox : ComboBox
    {
        public OxComboBox()
        {
            DoubleBuffered = true;
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected bool DrawStrings { get; set; } = true;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Color BrushColor =
                ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    ? new OxColorHelper(BackColor).Darker(2)
                    : BackColor;

            e.Graphics.DrawRectangle(new Pen(BrushColor), e.Bounds);
            e.Graphics.FillRectangle(new SolidBrush(BrushColor), e.Bounds);

            if (DrawStrings && e.Index > -1)
                e.Graphics.DrawString(Items[e.Index].ToString(),
                    e.Font ?? new Font("Calibri Light", 10),
                    new SolidBrush(Color.Black),
                    new Point(e.Bounds.X, e.Bounds.Y));
        }

        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);
            FlatStyle = FlatStyle.System;
            AutoCompleteMode = DropDownStyle == ComboBoxStyle.DropDownList
                ? AutoCompleteMode.None
                : AutoCompleteMode.Suggest;
        }
    }
}