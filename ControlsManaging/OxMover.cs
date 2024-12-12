using OxLibrary.Geometry;
using OxLibrary.Interfaces;

namespace OxLibrary;

public class OxMover
{
    public OxMover(IOxControl Control, Control? mover)
    {
        this.Control = Control;
        MoverControl = mover;
        SetHandlers();
    }

    private void SetHandlers()
    {
        if (MoverControl is null)
            return;

        MoverControl.MouseMove += MoverControlMoveHandler;
        MoverControl.MouseDown += MoverControlMouseDownHandler;
        MoverControl.MouseUp += MoverControlMouseUpHandler;
    }

    private void MoverControlMouseUpHandler(object? sender, MouseEventArgs e) =>
        Processing = false;

    private IOxControl Control { get; }
    private Control? MoverControl { get; }
    public bool Processing { get; set; }

    private Point LastMousePosition = new();

    private void MoverControlMouseDownHandler(object? sender, MouseEventArgs e)
    {
        Processing = e.Button is MouseButtons.Left;
        LastMousePosition = e.Location;
    }

    private void MoverControlMoveHandler(object? sender, MouseEventArgs e)
    {
        if (!Processing
            || LastMousePosition.Equals(e.Location))
            return;

        SetWindowState(e);

        int deltaX = e.X - LastMousePosition.X;
        int deltaY = e.Y - LastMousePosition.Y;
        LastMousePosition.X = e.X - deltaX;
        LastMousePosition.Y = e.Y - deltaY;

        if (Control is not IOxForm form
            || form.WindowState is FormWindowState.Normal)
            Move(Control.PointToScreen(new(deltaX, deltaY)));
        else Processing = false;
    }

    private void SetWindowState(MouseEventArgs e)
    {
        if (Control is not IOxForm form
            || !form.CanMaximize)
            return;

        switch (form.WindowState)
        {
            case FormWindowState.Maximized when !LastMousePosition.Y.Equals(e.Y):
                form.SetState(FormWindowState.Normal);
                break;
            case FormWindowState.Normal when Control.PointToScreen(e.Location).Y < 0:
                form.SetState(FormWindowState.Maximized);
                break;
        }
    }

    private void Move(Point FinishPosition)
    {
        if (FinishPosition.Equals(Control.Location.Point))
            return;

        List<Point> wayPoints = WayPoints(Control.Location.Point, FinishPosition, 30);

        foreach (Point point in wayPoints)
            Control.Location = new(point);
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

    public static void MoveToCenter(IOxControl control)
    {
        OxSize parentSize;
        OxPoint parentLocation;

        if (control.Parent is not null)
        {
            parentSize = new(control.Parent.OuterControlZone.Size);
            parentLocation = new(control.Parent.OuterControlZone.Location);
        }
        else
        {
            parentSize = new(Screen.GetWorkingArea((Control)control).Size);
            parentLocation = new(Screen.GetWorkingArea((Control)control).Location);
        }

        control.Location = new(
            OxSH.Add(
                parentLocation.X,
                OxSH.Half(parentSize.Width - control.Width)
            ),
            OxSH.Add(
                parentLocation.Y,
                OxSH.Half(parentSize.Height - control.Height)
            )
        );
        control.Size = new(control.Width, control.Height);
    }
}