using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Panels
{
    public class OxCard : OxFrameWithHeader, IOxCard
    {
        private bool expanded = true;

        private void SetExpanded(bool value)
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

#pragma warning disable CS0618 // Type or member is obsolete
                if (value)
                    ZBounds.RestoreBounds();
                else ZBounds.Height =
                        OxSH.Add(
                            Header.Border.Size, 
                            HeaderHeight,
                            Margin.Vertical
                        );
#pragma warning restore CS0618 // Type or member is obsolete
                Parent?.DoWithSuspendedLayout(() => Parent?.Realign());
            }
            finally
            {
                ExpandChanged?.Invoke(this, new(!Expanded, Expanded));
            }
        }

        public bool Expanded
        {
            get => expanded;
            set => SetExpanded(value);
        }

        private bool Expandable => IsVariableHeight;
        public void Expand() => Expanded = true;
        public void Collapse() => Expanded = false;

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
            Expanded = !Expanded;

        private readonly OxIconButton ExpandButton = new(OxIcons.Up, 20)
        {
            Default = true,
            ToolTipText = "Collapse"
        };

        private bool expandButtonVisible = true;

        public bool ExpandButtonVisible
        {
            get => expandButtonVisible;
            set
            {
                expandButtonVisible = value;
                ExpandButton.Visible = expandButtonVisible;
            }
        }

        private Bitmap ExpandButtonIcon =>
            expanded 
                ? OxIcons.Up 
                : OxIcons.Down;

        public string ExpandButtonToolTipText =>
            expanded 
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
            ExpandButton.Visible = false;
            Header.ToolBar.Visible = Header.ToolBar.Buttons.Count > 1;
        }

        public override void PutBack(OxPanelViewer dialog)
        {
            base.PutBack(dialog);
            ExpandButton.Visible = true;
            Header.ToolBar.Visible = true;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            ExpandButton.Visible = ExpandButtonVisible;
        }
    }
}