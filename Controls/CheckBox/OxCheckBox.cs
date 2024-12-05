using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls;

public class OxCheckBox :
    CheckBox, 
    IOxControlWithManager
{
    public IOxControlManager Manager { get; }

    public OxCheckBox()
    {
        Manager = OxControlManagers.RegisterControl(this);
        DoubleBuffered = true;
    }
    private bool readOnly = false;

    public bool ReadOnly
    {
        get => readOnly;
        set => readOnly = value;
    }

    protected override void OnCheckedChanged(EventArgs e)
    {
        if (!readOnly)
            base.OnCheckedChanged(e);
    }

    protected override void OnClick(EventArgs e)
    {
        if (readOnly)
            Checked = !Checked;

        base.OnClick(e);
    }

    #region Implemention of IOxControl using IOxControlManager
    public virtual void OnDockChanged(OxDockChangedEventArgs e) { }
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) { }
    public virtual void OnParentChanged(OxParentChangedEventArgs e) { }
    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) { }
    public new IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

    public new OxWidth Width
    {
        get => Manager.Width;
        set => Manager.Width = value;
    }

    public new OxWidth Height
    {
        get => Manager.Height;
        set => Manager.Height = value;
    }

    public new OxWidth Top
    {
        get => Manager.Top;
        set => Manager.Top = value;
    }

    public new OxWidth Left
    {
        get => Manager.Left;
        set => Manager.Left = value;
    }

    public new OxWidth Bottom =>
        Manager.Bottom;

    public new OxWidth Right =>
        Manager.Right;

    public new OxSize Size
    {
        get => Manager.Size;
        set => Manager.Size = value;
    }

    public new OxSize ClientSize
    {
        get => Manager.ClientSize;
        set => Manager.ClientSize = value;
    }

    public new OxPoint Location
    {
        get => Manager.Location;
        set => Manager.Location = value;
    }

    public new OxSize MinimumSize
    {
        get => Manager.MinimumSize;
        set => Manager.MinimumSize = value;
    }

    public new OxSize MaximumSize
    {
        get => Manager.MaximumSize;
        set => Manager.MaximumSize = value;
    }

    public new virtual OxDock Dock
    {
        get => Manager.Dock;
        set => Manager.Dock = value;
    }

    public new OxRectangle ClientRectangle =>
        Manager.ClientRectangle;

    public new OxRectangle Bounds
    {
        get => Manager.Bounds;
        set => Manager.Bounds = value;
    }

    public new OxPoint AutoScrollOffset
    {
        get => Manager.AutoScrollOffset;
        set => Manager.AutoScrollOffset = value;
    }

    public void DoWithSuspendedLayout(Action method) =>
        Manager.DoWithSuspendedLayout(method);

    public Control GetChildAtPoint(OxPoint pt) =>
        Manager.GetChildAtPoint(pt);

    public void Invalidate(OxRectangle rc) =>
        Manager.Invalidate(rc);

    public OxPoint PointToClient(OxPoint p) =>
        Manager.PointToClient(p);

    public OxPoint PointToScreen(OxPoint p) =>
        Manager.PointToScreen(p);

    public OxRectangle RectangleToClient(OxRectangle r) =>
        Manager.RectangleToClient(r);

    public OxRectangle RectangleToScreen(OxRectangle r) =>
        Manager.RectangleToScreen(r);

    public new event OxDockChangedEvent DockChanged
    {
        add => Manager.DockChanged += value;
        remove => Manager.DockChanged -= value;
    }

    public new event OxLocationChangedEvent LocationChanged
    {
        add => Manager.LocationChanged += value;
        remove => Manager.LocationChanged -= value;
    }

    public new event OxParentChangedEvent ParentChanged
    {
        add => Manager.ParentChanged += value;
        remove => Manager.ParentChanged -= value;
    }

    public new event OxSizeChangedEvent SizeChanged
    {
        add => Manager.SizeChanged += value;
        remove => Manager.SizeChanged -= value;
    }

    #region Internal used properties and methods
    [Obsolete("Z_Height it is used only for internal needs. Instead, use Height")]
    public int Z_Height
    {
        get => Manager.Z_Height;
        set => Manager.Z_Height = value;
    }

    [Obsolete("Z_Left it is used only for internal needs. Instead, use Left")]
    public int Z_Left
    {
        get => Manager.Z_Left;
        set => Manager.Z_Left = value;
    }

    [Obsolete("Z_Location it is used only for internal needs. Instead, use Location")]
    public Point Z_Location
    {
        get => Manager.Z_Location;
        set => Manager.Z_Location = value;
    }

    [Obsolete("Z_Size it is used only for internal needs. Instead, use Size")]
    public Size Z_Size
    {
        get => Manager.Z_Size;
        set => Manager.Z_Size = value;
    }

    [Obsolete("Z_Top it is used only for internal needs. Instead, use Top")]
    public int Z_Top
    {
        get => Manager.Z_Top;
        set => Manager.Z_Top = value;
    }

    [Obsolete("Z_Width it is used only for internal needs. Instead, use Width")]
    public int Z_Width
    {
        get => Manager.Z_Width;
        set => Manager.Z_Width = value;
    }

    [Obsolete("Z_RestoreLocation it is used only for internal needs")]
    public void Z_RestoreLocation() =>
        Manager.Z_RestoreLocation();

    [Obsolete("Z_RestoreSize it is used only for internal needs")]
    public void Z_RestoreSize() =>
        Manager.Z_RestoreSize();

    [Obsolete("Z_SaveLocation it is used only for internal needs")]
    public void Z_SaveLocation() =>
        Manager.Z_SaveLocation();

    [Obsolete("Z_SaveSize it is used only for internal needs")]
    public void Z_SaveSize() =>
        Manager.Z_SaveSize();
    #endregion

    #endregion

    #region Hidden base methods
#pragma warning disable IDE0051 // Remove unused private members
    private new void SetBounds(int x, int y, int width, int height) =>
        base.SetBounds(x, y, width, height);

    private new Size PreferredSize => base.PreferredSize;
    private new Rectangle DisplayRectangle => base.DisplayRectangle;
    private new Size GetPreferredSize(Size proposedSize) => base.GetPreferredSize(proposedSize);
    private new Size LogicalToDeviceUnits(Size value) => base.LogicalToDeviceUnits(value);
    private new void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) =>
        base.SetBounds(x, y, width, height, specified);
    private new Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue) =>
        base.GetChildAtPoint(pt, skipValue);

#pragma warning disable IDE0060 // Remove unused parameter
    private new void Invalidate(Rectangle rc, bool invalidateChildren) => Invalidate(true);
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    #endregion
}