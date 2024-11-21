using OxLibrary.Controls;
using OxLibrary.Dialogs;
using OxLibrary.Panels;

namespace OxLibrary
{
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
                    OxPane => 
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

        public static void CenterForm(OxForm form)
        {
            Screen screen = Screen.FromControl(form);
            form.SetBounds(
                OxWh.Add(
                    screen.Bounds.Left, 
                    OxWh.Div(
                        OxWh.Sub(screen.WorkingArea.Width, form.Width),
                        OxWh.W2
                    )
                ),
                OxWh.Add(
                    screen.Bounds.Top,
                    OxWh.Div(
                        OxWh.Sub(screen.WorkingArea.Height, form.Height), 
                        OxWh.W2
                    )
                ),
                form.Width,
                form.Height
            );
        }

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
}