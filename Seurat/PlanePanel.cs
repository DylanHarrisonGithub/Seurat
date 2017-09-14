using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;

namespace Seurat
{
    class PlanePanel : Panel
    {

        public double[] Center = { 0.0, 0.0 };
        double Zoom = 1.0;
        double ZoomInverse = 1.0;
        double[] PanelResolution = { 0.0, 0.0 };
        double[] HalfPanelResolution = { 0.0, 0.0 };

        public UInt32 BackgroundColor = 0xffffffff;
        public UInt32 GridColor = 0xffC8C8FF;
        public UInt32 AxesColor = 0xff000000;

        double[] MousePlane = { 0.0, 0.0 };
        double[] MousePanel = { 0.0, 0.0 };
        double[] PreviousMouse = { 0.0, 0.0 };
        bool LBDown = false;
        bool RBDown = false;

        public PlanePanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            PanelResolution[0] = ClientRectangle.Width;
            PanelResolution[1] = ClientRectangle.Height;

            HalfPanelResolution[0] = PanelResolution[0] / 2.0;
            HalfPanelResolution[1] = PanelResolution[1] / 2.0;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            PanelResolution[0] = ClientRectangle.Width;
            PanelResolution[1] = ClientRectangle.Height;

            HalfPanelResolution[0] = PanelResolution[0] / 2.0;
            HalfPanelResolution[1] = PanelResolution[1] / 2.0;
            Refresh();
        }

        public double[] PanelToPlaneCoordinates(double[] PanelCoordinates)
        {
            double[] PlaneCoordinate = { 0.0, 0.0 };
            PlaneCoordinate[0] = (PanelCoordinates[0] - HalfPanelResolution[0]) * ZoomInverse + Center[0];
            PlaneCoordinate[1] = -(PanelCoordinates[1] - HalfPanelResolution[1]) * ZoomInverse + Center[1];
            return PlaneCoordinate;
        }

        public double[] PlaneToPanelCoordinates(double[] PlaneCoordinates)
        {
            double[] PanelCoordinate = { 0.0, 0.0 };
            PanelCoordinate[0] = (PlaneCoordinates[0] - Center[0]) * Zoom + HalfPanelResolution[0];
            PanelCoordinate[1] = -(PlaneCoordinates[1] - Center[1]) * Zoom + HalfPanelResolution[1];
            return PanelCoordinate;
        }

        protected void DrawAxes(Graphics g)
        {
            double[] NorthWest = PanelToPlaneCoordinates(new double[] { 0.0, 0.0 });
            double[] SouthEast = PanelToPlaneCoordinates(new double[] { PanelResolution[0], PanelResolution[1] });
            Pen p = new Pen((Color)Color.FromArgb((int)AxesColor));

            if ((NorthWest[0] <= 0) && (SouthEast[0] >= 0))
            {
                double[] North = PlaneToPanelCoordinates(new double[] { 0.0, NorthWest[1] });
                double[] South = PlaneToPanelCoordinates(new double[] { 0.0, SouthEast[1] });

                g.DrawLine(p, (int)North[0], (int)North[1], (int)South[0], (int)South[1]);
            }
            if ((NorthWest[1] >= 0) && (SouthEast[1] <= 0))
            {
                double[] East = PlaneToPanelCoordinates(new double[] { NorthWest[0], 0.0 });
                double[] West = PlaneToPanelCoordinates(new double[] { SouthEast[0], 0.0 });

                g.DrawLine(p, (int)East[0], (int)East[1], (int)West[0], (int)West[1]);
            }
        }

        protected void DrawGrid(Graphics g)
        {
            double[] NorthWest = PanelToPlaneCoordinates(new double[] { 0.0, 0.0 });
            double[] SouthEast = PanelToPlaneCoordinates(new double[] { PanelResolution[0], PanelResolution[1] });
            double PlaneWidth = SouthEast[0] - NorthWest[0];
            double PlaneHeight = NorthWest[1] - SouthEast[1];
            double wMagnitude = Math.Floor(Math.Log10(PlaneWidth));
            double hMagnitude = Math.Floor(Math.Log10(PlaneHeight));
            double p1, p0, ones, tens;

            if (wMagnitude < hMagnitude)
            {
                double m = wMagnitude;
                p1 = Math.Pow(10, m - 1);
                p0 = Math.Pow(10, m);
                ones = Math.Floor(PlaneWidth / p1);
                tens = Math.Floor(PlaneWidth / p0);
            }
            else
            {
                double m = hMagnitude;
                p1 = Math.Pow(10, m - 1);
                p0 = Math.Pow(10, m);
                ones = Math.Floor(PlaneHeight / p1);
                tens = Math.Floor(PlaneHeight / p0);
            }

            double alpha = 255.0 * (1.0 - (ones / 99.0));
            Color cx = Color.FromArgb((int)GridColor);
            Pen p = new Pen(Color.FromArgb((int)alpha, cx.R, cx.G, cx.B));

            //ones
            double[] topCoord = new double[] { 0.0, 0.0 };
            double[] bottomCoord = new double[] { 0.0, 0.0 };
            double leftmostOne = Math.Floor(NorthWest[0] / p1) * p1;
            while (leftmostOne < SouthEast[0])
            {
                topCoord[0] = leftmostOne;
                topCoord[1] = NorthWest[1];
                bottomCoord[0] = leftmostOne;
                bottomCoord[1] = SouthEast[1];
                topCoord = PlaneToPanelCoordinates(topCoord);
                bottomCoord = PlaneToPanelCoordinates(bottomCoord);
                g.DrawLine(p, (int)topCoord[0], (int)topCoord[1], (int)bottomCoord[0], (int)bottomCoord[1]);
                leftmostOne += p1;
            }
            double bottommostOne = Math.Floor(SouthEast[1] / p1) * p1;
            double[] leftCoord = new double[] { 0.0, 0.0 };
            double[] rightCoord = new double[] { 0.0, 0.0 };
            while (bottommostOne < NorthWest[1])
            {
                leftCoord[0] = NorthWest[0];
                leftCoord[1] = bottommostOne;
                rightCoord[0] = SouthEast[0];
                rightCoord[1] = bottommostOne;
                leftCoord = PlaneToPanelCoordinates(leftCoord);
                rightCoord = PlaneToPanelCoordinates(rightCoord);
                g.DrawLine(p, (int)leftCoord[0], (int)leftCoord[1], (int)rightCoord[0], (int)rightCoord[1]);
                bottommostOne += p1;
            }

            //tens
            p = new Pen(Color.FromArgb(255, cx.R, cx.G, cx.B));

            leftmostOne = Math.Floor(NorthWest[0] / p0) * p0;
            while (leftmostOne < SouthEast[0])
            {
                topCoord[0] = leftmostOne;
                topCoord[1] = NorthWest[1];
                bottomCoord[0] = leftmostOne;
                bottomCoord[1] = SouthEast[1];
                topCoord = PlaneToPanelCoordinates(topCoord);
                bottomCoord = PlaneToPanelCoordinates(bottomCoord);
                g.DrawLine(p, (int)topCoord[0], (int)topCoord[1], (int)bottomCoord[0], (int)bottomCoord[1]);
                leftmostOne += p0;
            }
            bottommostOne = Math.Floor(SouthEast[1] / p0) * p0;
            while (bottommostOne < NorthWest[1])
            {
                leftCoord[0] = NorthWest[0];
                leftCoord[1] = bottommostOne;
                rightCoord[0] = SouthEast[0];
                rightCoord[1] = bottommostOne;
                leftCoord = PlaneToPanelCoordinates(leftCoord);
                rightCoord = PlaneToPanelCoordinates(rightCoord);
                g.DrawLine(p, (int)leftCoord[0], (int)leftCoord[1], (int)rightCoord[0], (int)rightCoord[1]);
                bottommostOne += p0;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb((int)BackgroundColor));

            DrawGrid(g);
            DrawAxes(g);
        }

        public static MouseEventHandler DefaultMouseClickEventHandler = new MouseEventHandler(
            (object sender, MouseEventArgs e) =>
            {
                PlanePanel self = (PlanePanel)sender;
                self.Center = self.PanelToPlaneCoordinates(new double[] { e.X, e.Y });
                self.Refresh();
            });
        public static MouseEventHandler DefaultMouseWheelEventHandler = new MouseEventHandler(
            (object sender, MouseEventArgs e) =>
            {
                PlanePanel self = (PlanePanel)sender;
                if (e.Delta > 0)
                {
                    self.Zoom *= (1.05);
                    self.ZoomInverse = 1.0 / self.Zoom;
                }
                else
                {
                    self.Zoom *= (0.95);
                    self.ZoomInverse = 1.0 / self.Zoom;
                }
                self.Refresh();
            });
        public static MouseEventHandler DefaultMouseDownEventHandler = new MouseEventHandler(
            (object sender, MouseEventArgs e) =>
            {
                PlanePanel self = (PlanePanel)sender;
                if (e.Button == MouseButtons.Left)
                {
                    self.LBDown = true;
                    self.PreviousMouse = self.PanelToPlaneCoordinates(new double[] { e.X, e.Y });
                }
                if (e.Button == MouseButtons.Right)
                {
                    self.RBDown = true;
                }
            });
        public static MouseEventHandler DefaultMouseUpEventHandler = new MouseEventHandler(
            (object sender, MouseEventArgs e) =>
            {
                PlanePanel self = (PlanePanel)sender;
                if (e.Button == MouseButtons.Left)
                {
                    self.LBDown = false;
                }
                if (e.Button == MouseButtons.Right)
                {
                    self.RBDown = false;
                }
            });
        public static MouseEventHandler DefaultMouseMoveEventHandler = new MouseEventHandler(
            (object sender, MouseEventArgs e) =>
            {
                PlanePanel self = (PlanePanel)sender;
                self.MousePanel[0] = e.X;
                self.MousePanel[1] = e.Y;
                self.MousePlane = self.PanelToPlaneCoordinates(self.MousePanel);
                if (self.LBDown)
                {
                    self.Center[0] -= (self.MousePlane[0] - self.PreviousMouse[0]);
                    self.Center[1] -= (self.MousePlane[1] - self.PreviousMouse[1]);
                    self.Refresh();
                }
            });
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        cp.Style |= 0x00040000; // 0x840000;  // Turn on WS_BORDER + WS_THICKFRAME
        //        return cp;
        //    }
        //}
    }
}
