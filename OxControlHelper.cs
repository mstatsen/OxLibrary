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

        public static Control? AlignByBaseLine(Control baseControl,
            Control aligningControl)
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
            return aligningControl;
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
    }
}