using OxLibrary.Panels;

namespace OxLibrary.ControlList;

public class OxClickFrameList<TClickFrame> : OxPanelList<TClickFrame>
    where TClickFrame : OxClickFrame, new()
{
    private TClickFrame? Default()
    {
        foreach (TClickFrame button in this)
            if (button.IsVisible && button.IsEnabled && button.Default)
                return button;

        return null;
    }

    public bool ExecuteDefault()
    {
        TClickFrame? defultButton = Default();
        defultButton?.PerformClick();
        return defultButton is not null;
    }
}

public class OxClickFrameList : OxClickFrameList<OxClickFrame>
{
}