namespace OxLibrary.ControlsManaging;

public class OxZBounds
{
    public OxZBounds(Control control) =>
        Control = control;

    public readonly Control Control;

    public int Left 
    { 
        get => Control.Left;
        set => Control.Left = value;
    }

    public int Top
    {
        get => Control.Top;
        set => Control.Top = value;
    }

    public int Width
    {
        get => Control.Width;
        set => Control.Width = value;
    }

    public int Height
    {
        get => Control.Height;
        set => Control.Height = value;
    }

    public Point Location
    {
        get => new(Left, Top);
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    public Size Size
    {
        get => new(Width, Height);
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    public Rectangle Bounds
    {
        get => new(Location, Size);
        set
        {
            Location = value.Location;
            Size = value.Size;
        }
    }

    public int GetLocationPart(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Left,
            OxDockVariable.Height =>
                Top,
            _ =>
                0,
        };

    public void SetLocationPart(OxDockVariable variable, int value)
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

    public int GetSizePart(OxDockVariable variable) =>
        variable switch
        {
            OxDockVariable.Width =>
                Width,
            OxDockVariable.Height =>
                Height,
            _ =>
                0,
        };

    public void SetSizePart(OxDockVariable variable, int value)
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
        Left = SavedLocation.Z_X;
        Top = SavedLocation.Z_Y;
    }

    public void RestoreSize()
    {
        Width = SavedSize.Z_Width;
        Height = SavedSize.Z_Height;
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
            OxDockVariable.Width => new(Left, SavedLocation.Z_Y),
            OxDockVariable.Height => new(SavedLocation.Z_X, Top),
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