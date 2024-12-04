namespace OxLibrary.Forms;

public class OxFormMover
{
    public OxFormMover(OxForm form, Control mover)
    {
        Form = form;
        Mover = mover;
        SetHandlers();
    }

    private void SetHandlers()
    {
        Mover.MouseMove += MoveHandler;
        Mover.MouseDown += MouseDownHandler;
        Mover.MouseUp += (s, e) => Processing = false;
    }

    private OxForm Form { get; }
    private Control Mover { get; }
    public bool Processing { get; set; }

    private Point LastMousePosition = new();

    private void MouseDownHandler(object? sender, MouseEventArgs e)
    {
        Processing = e.Button is MouseButtons.Left;
        LastMousePosition = e.Location;
    }

    private void MoveHandler(object? sender, MouseEventArgs e)
    {
        if (!Processing || LastMousePosition.Equals(e.Location))
            return;

        SetFormState(e);

        int deltaX = e.X - LastMousePosition.X;
        int deltaY = e.Y - LastMousePosition.Y;
        LastMousePosition.X = e.X - deltaX;
        LastMousePosition.Y = e.Y - deltaY;

        if (Form.WindowState is FormWindowState.Normal)
            MoveForm(
                new(
                    Form.PointToScreen(new Point(deltaX, deltaY))
                )
            );
        else Processing = false;
    }

    private void SetFormState(MouseEventArgs e)
    {
        if (!Form.CanMaximize)
            return;

        switch (Form.WindowState)
        {
            case FormWindowState.Maximized when !LastMousePosition.Y.Equals(e.Y):
                Form.MainPanel.SetFormState(FormWindowState.Normal);
                break;
            case FormWindowState.Normal when Form.PointToScreen(e.Location).Y < 20:
                Form.MainPanel.SetFormState(FormWindowState.Maximized);
                break;
        }
    }

    private void MoveForm(OxPoint FinishPosition)
    {
        if (FinishPosition.Equals(Form.Location))
            return;

        List<OxPoint> wayPoints = WayPoints(Form.Location, FinishPosition, 30);

        foreach (OxPoint point in wayPoints)
            Form.Location = point;
    }

    public static List<OxPoint> WayPoints(OxPoint Start, OxPoint Finish, int speed)
    {
        List<OxPoint> wayPoints = new();
        Point currentPoint = Start.Point;
        Point delta = new(Finish.X - Start.X, Finish.Y - Start.Y);
        Point sign = new(delta.X > 0 ? 1 : -1, delta.Y > 0 ? 1 : -1);
        delta.X = Math.Abs(delta.X);
        delta.Y = Math.Abs(delta.Y);

        int error = delta.X - delta.Y;
        int step = speed;

        while (!currentPoint.Equals(Finish.Point))
        {
            if (step.Equals(speed))
            {
                wayPoints.Add(new OxPoint(currentPoint));
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
            || !wayPoints[^1].Equals(Finish.Point))
            wayPoints.Add(Finish);

        return wayPoints;
    }
}