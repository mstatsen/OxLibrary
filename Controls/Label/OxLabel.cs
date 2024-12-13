using OxLibrary.Geometry;
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
        AutoSize = OxB.T;
        HideIfEmpty = true;
        Font = OxStyles.DefaultFont;
    }

    private OxBool cutByParentWidth = OxB.F;
    public OxBool CutByParentWidth
    { 
        get => cutByParentWidth;
        set
        {
            cutByParentWidth = value;

            if (OxB.B(value))
                AutoSize = OxB.F;

            RecalcText();
        } 
    }

    public bool IsCutByParentWidth =>
        OxB.B(CutByParentWidth);

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalcText();
    }

    private void RecalcText()
    {
        string calcedText = Text;

        RecalcVisible();

        if (Text.Equals(string.Empty)
            || !IsCutByParentWidth
            || Parent is null)
        {
            base.Text = Text;
            return;
        }
            
        short labelRight = OxSh.Add(Left, OxTextHelper.Width(calcedText, Font));

        while (labelRight >=  Parent.OuterControlZone.Right
            && calcedText.Length > 4)
        {
            calcedText = $"{calcedText.Remove(calcedText.Length - 4)}...";
            labelRight = OxSh.Add(Left, OxTextHelper.Width(calcedText, Font));
        }

        base.Text = calcedText;
        Width = OxSh.Sub(labelRight, Left);
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

    private void RecalcVisible()
    {
        needSaveInternalVisible = false;

        try
        {
            SetVisible(
                !hideIfEmpty
                || !Text.Equals(string.Empty)
            );
        }
        finally
        {
            needSaveInternalVisible = true;
        }
    }

    #region Implemention of IOxControl using IOxControlManager
    public virtual void OnAutoSizeChanged(OxBoolChangedEventArgs e)
    {
        base.OnAutoSizeChanged(e);

        if (!e.IsChanged ||
            !IsAutoSize)
            return;

        cutByParentWidth = OxB.F;
        RecalcText();
    }
        

    public virtual void OnDockChanged(OxDockChangedEventArgs e)  =>
        RecalcText();

    public virtual void OnEnabledChanged(OxBoolChangedEventArgs e) { }

    public virtual void OnLocationChanged(OxLocationChangedEventArgs e) =>
        RecalcText();

    public virtual void OnParentChanged(OxParentChangedEventArgs e) =>
        RecalcText();

    public virtual void OnSizeChanged(OxSizeChangedEventArgs e) =>
        RecalcText();

    public virtual void OnVisibleChanged(OxBoolChangedEventArgs e) =>
        RecalcText();

    public new IOxBox? Parent
    {
        get => Manager.Parent;
        set => Manager.Parent = value;
    }

    public new short Width
    {
        get => Manager.Width;
        set => Manager.Width = value;
    }

    public new short Height
    {
        get => Manager.Height;
        set => Manager.Height = value;
    }

    public new short Top
    {
        get => Manager.Top;
        set => Manager.Top = value;
    }

    public new short Left
    {
        get => Manager.Left;
        set => Manager.Left = value;
    }

    public new short Bottom =>
        Manager.Bottom;

    public new short Right =>
        Manager.Right;

    public new OxSize Size
    {
        get => Manager.Size;
        set => Manager.Size = value;
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

    public new OxBool AutoSize
    {
        get => Manager.AutoSize;
        set => Manager.AutoSize = value;
    }

    public bool IsAutoSize =>
        Manager.IsAutoSize;

    public void SetAutoSize(bool value) =>
        Manager.SetAutoSize(value);

    public new virtual OxDock Dock
    {
        get => Manager.Dock;
        set => Manager.Dock = value;
    }

    public new OxBool Enabled
    {
        get => Manager.Enabled;
        set => Manager.Enabled = value;
    }

    public bool IsEnabled =>
        Manager.IsEnabled;

    public void SetEnabled(bool value) =>
        Manager.SetEnabled(value);

    public new OxBool Visible
    {
        get => Manager.Visible;
        set => Manager.Visible = value;
    }

    public bool IsVisible =>
        Manager.IsVisible;

    public void SetVisible(bool value)
    {
        bool calcvalue = internalVisible && value;

        if (needSaveInternalVisible)
            internalVisible = value;

        Manager.SetVisible(calcvalue);
    }

    public void WithSuspendedLayout(Action method) =>
        Manager.WithSuspendedLayout(method);

    public new event OxBoolChangedEvent AutoSizeChanged
    {
        add => Manager.AutoSizeChanged += value;
        remove => Manager.AutoSizeChanged -= value;
    }

    public new event OxDockChangedEvent DockChanged
    {
        add => Manager.DockChanged += value;
        remove => Manager.DockChanged -= value;
    }

    public new event OxBoolChangedEvent EnabledChanged
    {
        add => Manager.EnabledChanged += value;
        remove => Manager.EnabledChanged -= value;
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

    public new event OxBoolChangedEvent VisibleChanged
    {
        add => Manager.VisibleChanged += value;
        remove => Manager.VisibleChanged -= value;
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
    protected sealed override void OnAutoSizeChanged(EventArgs e) { }
    protected sealed override void OnDockChanged(EventArgs e) { }
    protected sealed override void OnEnabledChanged(EventArgs e) { }
    protected sealed override void OnLocationChanged(EventArgs e) { }
    protected sealed override void OnParentChanged(EventArgs e) { }
    protected sealed override void OnSizeChanged(EventArgs e) { }

    protected sealed override void OnVisibleChanged(EventArgs e) { }
    #endregion
}