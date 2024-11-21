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

            //ContentBox.StartSizeRecalcing();
            try
            {
                Padding[OxDock.Top].Visible = value;
                Padding[OxDock.Bottom].Visible = value;
                Borders[OxDock.Bottom].Visible = value;
                //ContentBox.Visible = value;
                ExpandButton.Icon = ExpandButtonIcon;
                ExpandButton.ToolTipText = ExpandButtonToolTipText;

                CollapseOtherAccordions();

                if (accordion && changed)
                    BaseColor = expanded
                        ? Colors.HBluer(-2).Browner(1)
                        : Colors.HBluer(2).Browner(-1);
            }
            finally
            {
                Update();

                //ContentBox.EndSizeRecalcing();

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
            //ApplyVisibleChangedHandler(ContentBox);
            ExpandButton.Click += (s, e) => Expanded = !Expanded;
            Header.Click += AccordionExpandingChangeHandler;
        }

        private void AccordionExpandingChangeHandler(object? sender, EventArgs e)
        {
            if (Accordion)
                Expanded = !Expanded;
        }

        /*
        protected override OxWidth GetCalcedHeight()
        {
            OxWidth calcedHeight = base.GetCalcedHeight();

            if (!Expanded)
                calcedHeight -= Height;

            return calcedHeight;
        }
        */

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

        protected override void PrepareInnerControls()
        {
            base.PrepareInnerControls();
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