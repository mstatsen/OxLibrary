using System.Security.Policy;

namespace OxLibrary.ControlsManaging
{
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

        public OxDock Dock { get; set; }

        public bool SavingEnabled { get; private set; } = false;

        public void EnableSaving() => SavingEnabled = true;

        public void DisableSaving() => SavingEnabled = true;
        public void WithoutSave(Action method)
        { 
            DisableSaving();

            try
            {
                method();
            }
            finally
            {
                EnableSaving();
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

        public void SaveLocation()
        {
            if (SavingEnabled)
                return;

            SavedLocation = new(Left, Top);
        }

        public void SaveSize()
        {
            if (SavingEnabled)
                return;

            SavedSize = new(Width, Top);
        }

        public void SaveBounds()
        {
            SaveLocation();
            SaveSize();
        }

        private OxSize SavedSize = OxSize.Empty;

        private OxPoint SavedLocation = OxPoint.Empty;
    }
}