using OxLibrary.Controls;

namespace OxLibrary.Panels
{
    public class OxCard : OxFrameWithHeader, IOxCard
    {
        private bool expanded = true;
        private bool accordion = false;
        public bool Accordion
        {
            get => accordion;
            set => SetAccordion(value);
        }

        private void SetAccordion(bool value)
        {
            accordion = value;
            CollapseOtherAccordions();
        }

        private bool AccordionProcess = false;

        private void CollapseOtherAccordions()
        {
            if (!accordion 
                || !expanded 
                || Parent is null 
                || AccordionProcess)
                return;

            AccordionProcess = true;

            try
            {
                foreach (Control control in Parent.Controls)
                    if (control is OxCard card
                        && card.Expanded
                        && !card.Equals(this) 
                        && card.accordion)
                        card.Collapse();
            }
            finally
            {
                AccordionProcess = false;
            }
        }

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

                if (value)
                    Z_RestoreSize();
                else Z_Height = OxWh.IAdd(OxWh.Add(Header.Underline.Size, HeaderHeight), Margin.Vertical);

                Parent.Realign();

                CollapseOtherAccordions();

                if (accordion && changed)
                    BaseColor = expanded
                        ? Colors.HBluer(-2).Browner(1)
                        : Colors.HBluer(2).Browner(-1);
            }
            finally
            {
                if (changed)
                    ExpandHandler?.Invoke(this, EventArgs.Empty);
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

        public event EventHandler? ExpandHandler;

        public OxCard() : base() { }

        protected override void SetHandlers()
        {
            base.SetHandlers();
            ExpandButton.Click += (s, e) => Expanded = !Expanded;
            Header.Click += AccordionExpandingChangeHandler;
        }

        private void AccordionExpandingChangeHandler(object? sender, EventArgs e)
        {
            if (Accordion)
                Expanded = !Expanded;
        }

        private readonly OxIconButton ExpandButton = new(OxIcons.Up, OxWh.W20)
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
            ExpandButton.Size = new(OxWh.W25, OxWh.W20);
            Header.AddToolButton(ExpandButton);
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