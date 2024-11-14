namespace OxLibrary.Controls
{
    public static class MoveDirectionHelper
    {
        public static int Delta(MoveDirection direction) =>
            direction switch
            {
                MoveDirection.Up => -1,
                MoveDirection.Down => 1,
                _ => 0,
            };
    }
}