using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Panels
{
    public class OxCard : OxFrameWithHeader, IOxCard
    {
        private OxBool expanded = OxB.True;

        private void SetExpanded(OxBool value)
        {
            bool changed = !expanded.Equals(value);
            expanded = value;

            if (!Expandable)
                return;

            try
            {
                Padding[OxDock.Top].Visible = value;
                Padding[OxDock.Bottom].Visible = value;
                Borders[OxDock.Bottom].Visible = value;
                ExpandButton.Icon = ExpandButtonIcon;
                ExpandButton.ToolTipText = ExpandButtonToolTipText;
                Parent?.SuspendLayout();

                try
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    if (OxB.B(value))
                        ZBounds.RestoreSize();
                    else ZBounds.Height =
                            OxSh.Add(
                                Header.Border.Size,
                                HeaderHeight,
                                Margin.Vertical
                            );
#pragma warning restore CS0618 // Type or member is obsolete
                    Parent?.Realign();
                }
                finally
                {
                    Parent?.ResumeLayout();
                }

            }
            finally
            {
                ExpandChanged?.Invoke(this, new(Expanded, Expanded));
            }
        }

        public void SetExpanded(bool value) =>
            SetExpanded(OxB.B(value));

        public OxBool Expanded
        {
            get => expanded;
            set => SetExpanded(value);
        }

        private bool Expandable => IsVariableHeight;
        public void Expand() => Expanded = OxB.T;
        public void Collapse() => Expanded = OxB.F;

        public event ExpandChanged? ExpandChanged;

        public OxCard() : base() { }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            ExpandButton.Click += ExpandButtonClickHandler;
            //TODO: for accordion expand after one click on header
            //Header.Click += AccordionExpandingChangeHandler;
        }

        private void ExpandButtonClickHandler(object? sender, EventArgs e) =>
            Expanded = OxB.Not(Expanded);

        private readonly OxIconButton ExpandButton = new(OxIcons.Up, 20)
        {
            Default = true,
            ToolTipText = "Collapse"
        };

        private OxBool expandButtonVisible = OxB.True;

        public OxBool ExpandButtonVisible
        {
            get => expandButtonVisible;
            set
            {
                expandButtonVisible = value;
                ExpandButton.Visible = expandButtonVisible;
            }
        }

        private Bitmap ExpandButtonIcon =>
            OxB.B(expanded)
                ? OxIcons.Up 
                : OxIcons.Down;

        public string ExpandButtonToolTipText =>
            OxB.B(expanded)
                ? "Collapse" 
                : "Expand";

        protected override void PrepareInnerComponents()
        {
            base.PrepareInnerComponents();
            ExpandButton.Size = new(25, 20);
            Header.AddButton(ExpandButton);
        }

        protected override void PrepareDialog(OxPanelViewer dialog)
        {
            base.PrepareDialog(dialog);
            ExpandButton.Visible = OxB.F;
            Header.ToolBar.Visible = OxB.B(Header.ToolBar.Buttons.Count > 1);
        }

        public override void PutBack(OxPanelViewer dialog)
        {
            base.PutBack(dialog);
            ExpandButton.Visible = OxB.T;
            Header.ToolBar.Visible = OxB.T;
        }

        public override void OnVisibleChanged(OxBoolChangedEventArgs e)
        {
            base.OnVisibleChanged(e);
            ExpandButton.Visible = ExpandButtonVisible;
        }

        public void SetExpandButtonVisible(OxBool value) =>
            ExpandButtonVisible = value;

        public void SetExpandButtonVisible(bool value) =>
            SetExpandButtonVisible(OxB.B(value));

        public bool IsExpandButtonVisible =>
            OxB.B(expandButtonVisible);

        public bool IsExpanded =>
            OxB.B(Expanded);
    }
}