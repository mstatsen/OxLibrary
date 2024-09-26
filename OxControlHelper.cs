using OxLibrary.Controls;
using OxLibrary.Dialogs;

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
            T aligningControl)
            where T : Control
        {
            if (aligningControl == null)
                return null;

            int addPoints = 2;

            switch (baseControl)
            {
                case OxLabel _:
                    addPoints = 0;
                    break;
            }

            aligningControl.Top =
                (baseControl == null)
                    ? aligningControl.Top
                    : baseControl.Top
                        + GetBaseLine(baseControl)
                        - GetBaseLine(aligningControl)
                        + addPoints;
            return (T)aligningControl;
        }

        public static void CenterForm(OxForm form)
        {
            Screen screen = Screen.FromControl(form);
            form.SetBounds(
                screen.Bounds.Left + (screen.WorkingArea.Width - form.Width) / 2,
                screen.Bounds.Top + (screen.WorkingArea.Height - form.Height) / 2,
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

            return (foundControl != null) && foundControl.HasChildren 
                ? GetControlUnderMouse(foundControl, desktopPoint) 
                : foundControl;
        }
    }
}