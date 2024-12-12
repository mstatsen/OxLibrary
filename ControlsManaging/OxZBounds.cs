using OxLibrary.Geometry;

namespace OxLibrary;

public class OxZBounds
{
    public OxZBounds(Control control) =>
        Control = control;

    public readonly Control Control;

    public short Left 
    { 
        get => OxSH.Short(Control.Left);
        set => Control.Left = value;
    }

    public short Top
    {
        get => OxSH.Short(Control.Top);
        set => Control.Top = value;
    }

    public short Width
    {
        get => OxSH.Short(Control.Width);
        set => Control.Width = value;
    }

    public short Height
    {
        get => OxSH.Short(Control.Height);
        set => Control.Height = value;
    }

    public OxPoint Location
    {
        get => new(Left, Top);
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    public OxSize Size
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public OxRectangle Bounds
    {
        get => new(Location, Size);
        set
        {
            Location = value.Location;
            Size = value.Size;
        }
    }

    public short GetLocationPart(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Left,
            OxDockVariable.Height =>
                Top,
            _ =>
                0,
        };

    public void SetLocationPart(OxDockVariable variable, short value)
    {
        switch (variable)
        {
            case OxDockVariable.Width:
                Left = value;
                break;
            case OxDockVariable.Height:
                Top = value;
                break;
        }
    }

    public short GetSizePart(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Width,
            OxDockVariable.Height =>
                Height,
            _ =>
                0,
        };

    public void SetSizePart(OxDockVariable variable, short value)
    {
        switch (variable)
        {
            case OxDockVariable.Width:
                Width = value;
                break;
            case OxDockVariable.Height:
                Height = value;
                break;
        }
    }

    public OxDock Dock { get; set; } = OxDock.None;

    private bool SavingDisabled = false;

    public void WithoutSave(Action method)
    {
        SavingDisabled = true;

        try
        {
            method();
        }
        finally
        {
            SavingDisabled = false;
        }
    }

    public void RestoreLocation()
    {
        Left = SavedLocation.X;
        Top = SavedLocation.Y;
    }

    public void RestoreSize()
    {
        Width = SavedSize.Width;
        Height = SavedSize.Height;
    }

    public void RestoreBounds()
    {
        RestoreLocation();
        RestoreSize();
    }

    public void SaveLocation() =>
        SaveLocation(OxDockVariable.Fill);

    public void SaveLocation(OxDockVariable variable)
    {
        if (SavingDisabled)
            return;

        SavedLocation = variable switch
        {
            OxDockVariable.Width => new(Left, SavedLocation.Y),
            OxDockVariable.Height => new(SavedLocation.X, Top),
            _ => new(Left, Top),
        };
    }

    public void SaveSize() =>
        SaveSize(OxDockVariable.Fill);

    public void SaveSize(OxDockVariable variable)
    {
        if (SavingDisabled)
            return;

        SavedSize = variable switch
        {
            OxDockVariable.Width => new(Width, SavedSize.Height),
            OxDockVariable.Height => new(SavedSize.Width, Height),
            _ => new(Width, Height),
        };
    }

    public void SaveBounds()
    {
        if (SavingDisabled)
            return;

        SaveLocation();
        SaveSize();
    }

    private OxSize SavedSize = OxSize.Empty;

    private OxPoint SavedLocation = OxPoint.Empty;
}