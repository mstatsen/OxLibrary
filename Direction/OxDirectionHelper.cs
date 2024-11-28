using OxLibrary.Panels;

namespace OxLibrary
{
    public static class OxDirectionHelper
    {
        public static OxDirection GetDirection(OxDock dock) =>
            dock switch
            {
                OxDock.Left => OxDirection.Left,
                OxDock.Right => OxDirection.Right,
                OxDock.Top => OxDirection.Top,
                OxDock.Bottom => OxDirection.Bottom,
                _ => OxDirection.None,
            };

        public static OxDirection GetDirection(OxPanel border, OxPoint position)
        {
            OxDirection direction = GetDirection(border.Dock);
            OxWidth borderSize = OxDockHelper.IsVertical(border.Dock)
                ? border.Height
                : border.Width;
            OxWidth error = OxWh.Mul(borderSize, 2);

            switch (border.Dock)
            {
                case OxDock.Left:
                case OxDock.Right:
                    if (position.Y < error)
                        direction |= OxDirection.Top;
                    else
                    if (OxWh.Greater(position.Y, OxWh.Sub(border.Height, error)))
                        direction |= OxDirection.Bottom;
                    break;
                case OxDock.Top:
                case OxDock.Bottom:
                    if (position.X < error)
                        direction |= OxDirection.Left;
                    else
                    if (OxWh.Greater(position.X, OxWh.Sub(border.Width, error)))
                        direction |= OxDirection.Right;
                    break;
            }

            return direction;
        }

        public static bool IsLeft(OxDirection direction) => direction is OxDirection.Left;
        public static bool IsTop(OxDirection direction) => direction is OxDirection.Top;
        public static bool IsRight(OxDirection direction) => direction is OxDirection.Right;
        public static bool IsBottom(OxDirection direction) => direction is OxDirection.Bottom;

        public static bool ContainsLeft(OxDirection direction) => (direction & OxDirection.Left) is not 0;
        public static bool ContainsTop(OxDirection direction) => (direction & OxDirection.Top) is not 0;
        public static bool ContainsRight(OxDirection direction) => (direction & OxDirection.Right) is not 0;
        public static bool ContainsBottom(OxDirection direction) => (direction & OxDirection.Bottom) is not 0;

        public static bool IsLeftTop(OxDirection direction) =>
            ContainsLeft(direction)
            && ContainsTop(direction)
            && !ContainsRight(direction)
            && !ContainsBottom(direction);

        public static bool IsLeftBottom(OxDirection direction) =>
            ContainsLeft(direction)
            && !ContainsTop(direction)
            && !ContainsRight(direction)
            && ContainsBottom(direction);

        public static bool IsRightTop(OxDirection direction) =>
            !ContainsLeft(direction)
            && ContainsTop(direction)
            && ContainsRight(direction)
            && !ContainsBottom(direction);

        public static bool IsRightBottom(OxDirection direction) =>
            !ContainsLeft(direction)
            && !ContainsTop(direction)
            && ContainsRight(direction)
            && ContainsBottom(direction);

        public static bool IsHorizontal(OxDirection direction) =>
            IsLeft(direction) || IsRight(direction);

        public static bool IsVertical(OxDirection direction) =>
            IsTop(direction) || IsBottom(direction);

        public static Cursor GetSizerCursor(OxDirection direction)
        {
            if (IsHorizontal(direction))
                return Cursors.SizeWE;

            if (IsVertical(direction))
                return Cursors.SizeNS;

            if (IsLeftTop(direction)
                || IsRightBottom(direction))
                return Cursors.SizeNWSE;

            if (IsRightTop(direction)
                || IsLeftBottom(direction))
                return Cursors.SizeNESW;

            return default!;
        }
    }
}