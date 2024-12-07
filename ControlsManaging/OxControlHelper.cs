using OxLibrary.Controls;
using OxLibrary.Panels;

namespace OxLibrary;

public static class OxControlHelper
{
    private static int GetBaseLine(Control control) =>
        (int)(
            control.Font.GetHeight()
                * control.Font.FontFamily.GetCellAscent(control.Font.Style)
                / control.Font.FontFamily.GetLineSpacing(control.Font.Style));

    public static T? AlignByBaseLine<T>(Control baseControl,
        T? aligningControl)
        where T : Control
    {
        if (aligningControl is null)
            return null;

        aligningControl.Top =
            aligningControl switch
            {
                OxPanel =>
                    baseControl.Top - (aligningControl.Height - baseControl.Height) / 2,
                _ =>
                    baseControl is null
                        ? aligningControl.Top
                        : baseControl.Top
                            + GetBaseLine(baseControl)
                            - GetBaseLine(aligningControl)
                            + (baseControl is OxLabel ? 0 : 2),
            };
        return aligningControl;
    }

    public static OxSize ScreenSize(Control control) =>
        new(Screen.GetWorkingArea(control).Size);

    public static Control? GetControlUnderMouse(Control topControl) =>
        GetControlUnderMouse(topControl, Cursor.Position);

    private static Control? GetControlUnderMouse(Control topControl, Point desktopPoint)
    {
        Point thisPoint = topControl.PointToClient(desktopPoint);
        Control foundControl = topControl.GetChildAtPoint(thisPoint);

        return foundControl is not null
            && foundControl.HasChildren
                ? GetControlUnderMouse(foundControl, desktopPoint)
                : foundControl;
    }
}