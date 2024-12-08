using OxLibrary.Forms;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary;

public class OxBoxManager :
    OxControlManager,
    IOxBoxManager
{
    public OxBoxManager(Control managingBox) : base(managingBox)
    {
        OxControls = new(Box);
#pragma warning disable CS0618 // Type or member is obsolete
        Aligner = new(Box);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public IOxBox Box =>
        (IOxBox)ManagingControl;

    [Obsolete("Aligner it is used only for internal needs")]
    private readonly OxControlAligner Aligner;

#pragma warning disable CS0618 // Type or member is obsolete
    public void Realign()
    {
        Aligner.Realign();
        InnerControlZone.CopyFrom(Aligner.ControlZone);
    }

    public bool Realigning =>
        Aligner.Realigning;
#pragma warning restore CS0618 // Type or member is obsolete

    protected override void SetHandlers()
    {
        Box.ControlAdded += ControlAddedHandler;
        Box.ControlRemoved += ControlRemovedHandler;

        if (OxControl is not IOxDependedBox
            && OxControl is IOxWithPadding controlWithPadding
            && controlWithPadding.Padding is not null)
            controlWithPadding.Padding.SizeChanged += PaddingSizeChangedHandler;

        base.SetHandlers();
    }

    private void PaddingSizeChangedHandler(object sender, OxBordersChangedEventArgs e) =>
        Realign();

    private void ControlRemovedHandler(object? sender, ControlEventArgs e)
    {
        if (e.Control is not IOxControl oxControl)
            return;

        OxControls.Remove(oxControl);
    }

    private void ControlAddedHandler(object? sender, ControlEventArgs e)
    {
        if (e.Control is not IOxControl oxControl
            || e.Control.Equals(OxControl))
            return;

        if (Box is IOxDependedBox dependedBox
            && !oxControl.Equals(dependedBox.DependedFrom))
        {
            oxControl.Parent = dependedBox.DependedFrom;
            return;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        oxControl.ZBounds.SaveBounds();
#pragma warning restore CS0618 // Type or member is obsolete

        OxControls.Add(oxControl);

        if (OxControl is IOxWithColorHelper colorHelperControl)
            colorHelperControl.PrepareColors();
    }

    protected override void RealignParent()
    {
        if (Parent is null)
            Realign();
        else
            base.RealignParent();
    }

    public OxControls OxControls { get; private set; }

    public OxRectangle OuterControlZone
    {
        get
        {
            OxRectangle outerZone = new(ClientRectangle);

            if (OxControl is not IOxDependedBox)
            {
                if (OxControl is IOxWithPadding controlWithPadding)
                    outerZone -= controlWithPadding.Padding;

                if (OxControl is IOxWithBorders controlWithBorders)
                    outerZone -= controlWithBorders.Borders;

                if (OxControl is IOxWithMargin controlWithMargin)
                    outerZone -= controlWithMargin.Margin;
            }

            return outerZone;
        }
    }

    public OxRectangle innerControlZone = new();

    public OxRectangle InnerControlZone 
    {
        get
        {
            if (innerControlZone.IsEmpty)
                innerControlZone.CopyFrom(OuterControlZone);

            return innerControlZone;
        }
    }

    public bool HandleParentPadding => Box.HandleParentPadding;
}