using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class SprayCanBrush : BrushType
    {
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private SolidBrush myBrush = new SolidBrush(Color.Black);
        private TrackBar myWidthTrackBar;
        private int sprayRadius = 1;
        private int sprayNoise = 1;
        private int particleRadius = 1;
        private int particleCount = 1;
        private Random myRandom = new Random();

        public SprayCanBrush()
        {
            Image = Properties.Resources.if_spray_45299;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Spray Can", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddLabelTrackBar("Size", 1, 25, sprayRadius);
            s.AddLabelTrackBar("Noise", 1, 255, sprayNoise);
            s.AddHeaderLabel("Particles", System.Drawing.Color.White, System.Drawing.Color.Blue);
            s.AddLabelTrackBar("Size", 1, 25, particleRadius);
            s.AddLabelTrackBar("Count", 1, 25, particleCount);

            myWidthTrackBar = (TrackBar)s.StackControls[1];
            ((TrackBar)s.StackControls[1]).ValueChanged += (object sender, EventArgs e) =>
            {
                sprayRadius = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[2]).ValueChanged += (object sender, EventArgs e) =>
            {
                sprayNoise = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[4]).ValueChanged += (object sender, EventArgs e) =>
            {
                particleRadius = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[5]).ValueChanged += (object sender, EventArgs e) =>
            {
                particleCount = ((TrackBar)sender).Value;
            };

            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
            g.Clear(Color.FromArgb(0));
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                int noise = 0;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        noise = 255-cpp.MyColorPicker.TrackBarC1.Value;
                        break;
                    case MouseButtons.Right:
                        noise = 255-cpp.MyColorPicker.TrackBarC2.Value;
                        break;
                    default:
                        break;
                }

                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                int[] coords = new int[] { 0, 0 };
                double rad, theta;
                int t;
                for (int n = 0; n < particleCount; n++)
                {
                    rad = myRandom.NextDouble() * sprayRadius;
                    theta = myRandom.NextDouble() * Math.PI * 2.0;
                    coords[0] = currentCoordinates[0] + (int)(rad * Math.Cos(theta));
                    coords[1] = currentCoordinates[1] + (int)(rad * Math.Sin(theta));
                    t = noise + (int) (myRandom.NextDouble() * 2.0* sprayNoise - sprayNoise);
                    if (t > 255)
                    {
                        t = 255;
                    }
                    if (t < 0)
                    {
                        t = 0;
                    }
                    myBrush.Color = Color.FromArgb((int)cpp.MyColorPicker.GetInterpolatedColor(
                        (byte)t, 
                        (UInt32)cpp.MyColorPicker.VerticalGradient.c1,
                        (UInt32)cpp.MyColorPicker.VerticalGradient.c2));
                    g.FillEllipse(myBrush,
                        coords[0] - particleRadius/2,
                        coords[1] - particleRadius / 2,
                        particleRadius,
                        particleRadius);
                }
                
                cpp.Invalidate();
                g.Dispose();
            }
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
            g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
            cpp.DrawingLayer.DBMP.clear(0);
            g.Dispose();
            cpp.Invalidate();
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (myWidthTrackBar != null)
            {
                if (e.Delta > 0)
                {
                    if (myWidthTrackBar.Value < myWidthTrackBar.Maximum)
                    {
                        myWidthTrackBar.Value = myWidthTrackBar.Value + 1;
                    }
                }
                else
                {
                    if (myWidthTrackBar.Value > myWidthTrackBar.Minimum)
                    {
                        myWidthTrackBar.Value = myWidthTrackBar.Value - 1;
                    }
                }
            }
        }
    }
}
