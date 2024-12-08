using OxLibrary.Interfaces;

namespace OxLibrary.Forms;

public class OxBoxMover
{
    public OxBoxMover(IOxBox Box, Control? mover)
    {
        this.Box = Box;
        Mover = mover;
        SetHandlers();
    }

    private void SetHandlers()
    {
        if (Mover is null)
            return;

        Mover.MouseMove += MoverMoveHandler;
        Mover.MouseDown += MoverMouseDownHandler;
        Mover.MouseUp += MoverMouseUpHandler;
    }

    private void MoverMouseUpHandler(object? sender, MouseEventArgs e) =>
        Processing = false;

    private IOxBox Box { get; }
    private Control? Mover { get; }
    public bool Processing { get; set; }

    private Point LastMousePosition = new();

    private void MoverMouseDownHandler(object? sender, MouseEventArgs e)
    {
        Processing = e.Button is MouseButtons.Left;
        LastMousePosition = e.Location;
    }

    private void MoverMoveHandler(object? sender, MouseEventArgs e)
    {
        if (!Processing
            || LastMousePosition.Equals(e.Location))
            return;

        SetWindowState(e);

        int deltaX = e.X - LastMousePosition.X;
        int deltaY = e.Y - LastMousePosition.Y;
        LastMousePosition.X = e.X - deltaX;
        LastMousePosition.Y = e.Y - deltaY;

        if (Box is not IOxForm form
            || form.WindowState is FormWindowState.Normal)
            Move(Box.PointToScreen(new(deltaX, deltaY)));
        else Processing = false;
    }

    private void SetWindowState(MouseEventArgs e)
    {
        if (Box is not IOxForm form
            || !form.CanMaximize)
            return;

        switch (form.WindowState)
        {
            case FormWindowState.Maximized when !LastMousePosition.Y.Equals(e.Y):
                form.SetState(FormWindowState.Normal);
                break;
            case FormWindowState.Normal when OxWh.Less(Box.PointToScreen(e.Location).Y, OxWh.W0):
                form.SetState(FormWindowState.Maximized);
                break;
        }
    }

    private void Move(Point FinishPosition)
    {
        if (FinishPosition.Equals(Box.Location.Point))
            return;

        List<Point> wayPoints = WayPoints(Box.Location.Point, FinishPosition, 30);

        foreach (Point point in wayPoints)
            Box.Location = new(point);
    }

    public static List<Point> WayPoints(OxPoint Start, OxPoint Finish, int speed) =>
        WayPoints(Start.Point, Finish.Point, speed);

    public static List<Point> WayPoints(Point Start, Point Finish, int speed)
    {
        List<Point> wayPoints = new();
        Point currentPoint = Start;
        Point delta = new(Finish.X - Start.X, Finish.Y - Start.Y);
        Point sign = new(delta.X > 0 ? 1 : -1, delta.Y > 0 ? 1 : -1);
        delta.X = Math.Abs(delta.X);
        delta.Y = Math.Abs(delta.Y);

        int error = delta.X - delta.Y;
        int step = speed;

        while (!currentPoint.Equals(Finish))
        {
            if (step.Equals(speed))
            {
                wayPoints.Add(currentPoint);
                step = 0;
            }

            step++;

            int error2 = error * 2;

            if (error2 > -delta.Y)
            {
                error -= delta.Y;
                currentPoint.X += sign.X;
            }

            if (error2 < delta.X)
            {
                error += delta.X;
                currentPoint.Y += sign.Y;
            }
        }

        if (wayPoints.Count is 0 
            || !wayPoints[^1].Equals(Finish))
            wayPoints.Add(Finish);

        return wayPoints;
    }
}