using OxLibrary.ControlsManaging;
using OxLibrary.Handlers;
using OxLibrary.Interfaces;

namespace OxLibrary.Controls;

public class OxLabel :
    Label,
    IOxControlWithManager
{
    public IOxControlManager Manager { get; }

    public OxLabel()
    {
        Manager = OxControlManagers.RegisterControl(this);
        DoubleBuffered = true;
        AutoSize = true;
        HideIfEmpty = true;
        Font = OxStyles.DefaultFont;
    }

    private bool cutByParentWidth = false;
    public bool CutByParentWidth
    { 
        get => cutByParentWidth;
        set
        {
            cutByParentWidth = value;

            if (value)
                AutoSize = false;

            RecalcText();
        } 
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalcText();
    }

    protected override void OnAutoSizeChanged(EventArgs e)
    {
        base.OnAutoSizeChanged(e);

        if (AutoSize)
        {
            cutByParentWidth = false;
            RecalcText();
        }
    }

    private void RecalcText()
    {
        string calcedText = Text;

        RecalcVisible();

        if (Text.Equals(string.Empty)
            || !cutByParentWidth
            || Parent is null)
        {
            base.Text = Text;
            return;
        }
            
        OxWidth labelRight = OxWh.Add(Left, OxTextHelper.GetTextWidth(calcedText, Font));

        while (OxWh.GreaterOrEquals(labelRight, Parent.OuterControlZone.Right)
            && calcedText.Length > 4)
        {
            calcedText = $"{calcedText.Remove(calcedText.Length - 4)}...";
            labelRight = OxWh.Add(Left, OxTextHelper.GetTextWidth(calcedText, Font));
        }

        base.Text = calcedText;
        Width = OxWh.S(labelRight, Left);
    }

    private string text = string.Empty;

    public new string Text 
    { 
        get => text;
        set
        {
            text = value;
            RecalcText();
        }
    }

    private bool hideIfEmpty;
    public bool HideIfEmpty
    {
        get => hideIfEmpty;
        set
        { 
            hideIfEmpty = value;
            RecalcText();
        }
    }

    private bool internalVisible = true;
    private bool needSaveInternalVisible = true;

    public new bool Visible
    { 
        get => base.Visible;
        set
        {
            base.Visible = internalVisible && value;

            if (needSaveInternalVisible)
                internalVisible = value;
        }
    }

    private void RecalcVisible()
    {
        needSaveInternalVisible = false;

        try
        {
            Visible = !hideIfEmpty
                || !Text.Equals(string.Empty);
        }
        finally
        {
            needSaveInternalVisible = true;
        }
    }

    #region Implemention of IOxControl using IOxControlManager
    public virtual void OnDockChanged(OxDockChangedEventArgs e) 
    {
        RecalcText();
    }
    
    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) 
    {
        RecalcText();
    }

    public virtual void OnParentChanged(OxParentChangedEventArgs e) 
    {
        RecalcText();
    }

    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) 
    {
        RecalcText();
    }

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

    public void DoWithSuspendedLayout(Action method) =>
        Manager.DoWithSuspendedLayout(method);

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

    public void AddHandler(OxHandlerType type, Delegate handler) =>
        Manager.AddHandler(type, handler);

    public void InvokeHandlers(OxHandlerType type, OxEventArgs args) =>
        Manager.InvokeHandlers(type, args);

    public void RemoveHandler(OxHandlerType type, Delegate handler) =>
        Manager.RemoveHandler(type, handler);

    #region Internal used properties and methods
    [Obsolete("ZBounds it is used only for internal needs")]
    public OxZBounds ZBounds =>
        Manager.ZBounds;
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

#pragma warning restore IDE0051 // Remove unused private members
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }
    #endregion
}