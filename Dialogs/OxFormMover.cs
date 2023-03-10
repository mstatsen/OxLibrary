using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OxLibrary.Dialogs
{
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
            Mover.MouseUp += MouseUpHandler;
        }

        private OxForm Form { get; }
        private Control Mover { get; }
        public bool Processing { get; set; }

        private Point LastMousePosition = new();

        private void MouseUpHandler(object? sender, MouseEventArgs e) =>
            Processing = false;

        private void MouseDownHandler(object? sender, MouseEventArgs e)
        {
            Processing = e.Button == MouseButtons.Left;
            LastMousePosition = e.Location;
        }

        private void MoveHandler(object? sender, MouseEventArgs e)
        {
            if (Processing && !LastMousePosition.Equals(e.Location))
            {
                if (Form.CanMaximize)
                    switch (Form.WindowState)
                    {
                        case FormWindowState.Maximized when LastMousePosition.Y != e.Y:
                            Form.MainPanel.SetFormState(FormWindowState.Normal);
                            break;
                        case FormWindowState.Normal when Form.PointToScreen(e.Location).Y < 20:
                            Form.MainPanel.SetFormState(FormWindowState.Maximized);
                            break;
                    }

                int deltaX = e.X - LastMousePosition.X;
                int deltaY = e.Y - LastMousePosition.Y;
                LastMousePosition.X = e.X - deltaX;
                LastMousePosition.Y = e.Y - deltaY;

                if (Form.WindowState == FormWindowState.Normal)
                    MoveForm(Form.PointToScreen(new Point(deltaX, deltaY)));
                else Processing = false;
            }
        }

        private void MoveForm(Point FinishPosition)
        {
            if (FinishPosition.Equals(Form.Location))
                return;

            List<Point> wayPoints = WayPoints(Form.Location, FinishPosition, 30);

            foreach (Point point in wayPoints)
                Form.Location = point;
        }

        public static List<Point> WayPoints(Point Start, Point Finish, int speed)
        {
            List<Point> wayPoints = new();

            Point currentPoint = new(Start.X, Start.Y);
            Point delta = new(Finish.X - Start.X, Finish.Y - Start.Y);
            Point sign = new(delta.X > 0 ? 1 : -1, delta.Y > 0 ? 1 : -1);
            delta.X = Math.Abs(delta.X);
            delta.Y = Math.Abs(delta.Y);

            int error = delta.X - delta.Y;
            int step = speed;

            while (currentPoint.X != Finish.X || currentPoint.Y != Finish.Y)
            {
                if (step == speed)
                {
                    wayPoints.Add(new Point(currentPoint.X, currentPoint.Y));
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

            if (wayPoints.Count == 0 || !wayPoints[^1].Equals(Finish))
                wayPoints.Add(Finish);

            return wayPoints;
        }
    }
}