using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Interfaces;
using OxLibrary.Panels;

namespace OxLibrary;

public static class OxControlHelper
{
    private static short GetBaseLine(IOxControl control) =>
        OxSH.Div(
            OxSH.Mul(
                control.Font.GetHeight(),
                control.Font.FontFamily.GetCellAscent(control.Font.Style)
            ),
            control.Font.FontFamily.GetLineSpacing(control.Font.Style));

    public static T? AlignByBaseLine<T>(IOxControl baseControl, T? aligningControl)
        where T : IOxControl, new() => AlignByBaseLine(baseControl, aligningControl);

    public static IOxControl? AlignByBaseLine(
        IOxControl baseControl,
        IOxControl? aligningControl)
    {
        if (aligningControl is null)
            return null;

        aligningControl.Top =
            aligningControl switch
            {
                OxPanel =>
                    OxSH.Sub(baseControl.Top, OxSH.Half(aligningControl.Height - baseControl.Height)),
                _ =>
                    OxSH.IfElse(
                        baseControl is null,
                            aligningControl.Top,
                            baseControl!.Top
                                + OxSH.Sub(
                                    GetBaseLine(baseControl), 
                                    GetBaseLine(aligningControl))
                                + OxSH.IfElseZero(baseControl is not OxLabel, 2)
                    )
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