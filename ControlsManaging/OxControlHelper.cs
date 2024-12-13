using OxLibrary.Controls;
using OxLibrary.Geometry;
using OxLibrary.Interfaces;
using OxLibrary.Panels;
using System.Runtime.InteropServices;

namespace OxLibrary;

public static class OxControlHelper
{
    private static short BaseLine(IOxControl control) =>
        OxSh.Div(
            OxSh.Mul(
                control.Font.GetHeight(),
                control.Font.FontFamily.GetCellAscent(control.Font.Style)
            ),
            control.Font.FontFamily.GetLineSpacing(control.Font.Style));

    public static T? AlignByBaseLine<T>(
        IOxControl baseControl,
        T? aligningControl)
        where T : IOxControl
    {
        if (aligningControl is null)
            return default;

        aligningControl.Top =
            aligningControl switch
            {
                OxPanel =>
                    OxSh.Sub(
                        baseControl.Top, 
                        OxSh.CenterOffset(aligningControl.Height, baseControl.Height)
                    ),
                _ =>
                    baseControl is null
                        ? aligningControl.Top
                        : OxSh.Add(
                            baseControl!.Top,
                            OxSh.Sub(
                                BaseLine(baseControl), 
                                BaseLine(aligningControl)),
                            baseControl is not OxLabel ? 2 : 0
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

    [DllImport("user32.dll")]
    private static extern bool HideCaret(IntPtr hWnd);
    public static void HideTextCursor(IntPtr handle) =>
        HideCaret(handle);
}