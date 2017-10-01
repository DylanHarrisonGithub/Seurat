using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class PaintBucketBrush : BrushType
    {
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private SolidBrush myBrush = new SolidBrush(Color.Black);
        private TrackBar myWidthTrackBar;
        private Random myRandom = new Random();
        private int noise = 0;
        private bool isGradated = false;
        private int rTolerance = 0;
        private int gTolerance = 0;
        private int bTolerance = 0;
        private Point origin = new Point();
        private Point axis = new Point();
        private int floodWidth = 0;
        private int maxAxialDist = 0;
        private int minAxialDist = 0;
        private int baseColor = 0;
        private const int PRIMER_FLAG = 0x01000000;
        private const int MAX_DIST = 0x00ffffff;

        public PaintBucketBrush()
        {
            Image = Properties.Resources.if_paintbucket_15221;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Paint Bucket", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddLabelCheckBox("Grad", isGradated);
            s.AddLabelTrackBar("Noise", 0, 255, noise);
            s.AddHeaderLabel("Tolerance", System.Drawing.Color.White, System.Drawing.Color.Blue);
            s.AddLabelTrackBar("r", 0, 255, rTolerance);
            s.AddLabelTrackBar("g", 0, 255, gTolerance);
            s.AddLabelTrackBar("b", 0, 255, bTolerance);

            ((CheckBox)s.StackControls[1]).CheckedChanged += (object sender, EventArgs e) =>
            {
                isGradated = ((CheckBox)sender).Checked;
            };
            myWidthTrackBar = (TrackBar)s.StackControls[2];
            ((TrackBar)s.StackControls[2]).ValueChanged += (object sender, EventArgs e) =>
            {
                noise = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[4]).ValueChanged += (object sender, EventArgs e) =>
            {
                rTolerance = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[5]).ValueChanged += (object sender, EventArgs e) =>
            {
                gTolerance = ((TrackBar)sender).Value;
            };
            ((TrackBar)s.StackControls[6]).ValueChanged += (object sender, EventArgs e) =>
            {
                bTolerance = ((TrackBar)sender).Value;
            };

            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {            
            int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            origin.X = currentCoordinates[0];
            origin.Y = currentCoordinates[1];
            if (PixelIsWithinImageBounds(cpp, currentCoordinates[0], currentCoordinates[1]))
            {
                baseColor = (int) cpp.GetActiveLayer().DBMP.pGet(currentCoordinates[0], currentCoordinates[1]);
                cpp.DrawingLayer.DBMP.clear(0);

                if (isGradated)
                {
                    floodWidth = 0;
                    minAxialDist = 0;
                    RadialPrimerFill(cpp, currentCoordinates[0], currentCoordinates[1]);
                } else
                {
                    SolidPrimerFill(cpp, currentCoordinates[0], currentCoordinates[1]);
                }

                DirectBitmap activeLayer = cpp.GetActiveLayer().DBMP;

                for (int y = 0; y < cpp.CanvasHeight; y++)
                {
                    for (int x = 0; x < cpp.CanvasWidth; x++)
                    {
                        if ((cpp.DrawingLayer.DBMP.pGet(x, y) & PRIMER_FLAG) != 0)
                        {
                            double t = (cpp.DrawingLayer.DBMP.pGet(x, y) & ~PRIMER_FLAG) - minAxialDist;
                            t /= floodWidth;
                            t = ApplyNoiseToColorInterpolationParamater(t);
                            if (e.Button == MouseButtons.Left)
                            {
                                cpp.DrawingLayer.DBMP.pSet(x, y, GetLBColorFromColorPickerGradient(cpp, t));
                            } else
                            {
                                if (e.Button == MouseButtons.Right)
                                {
                                    cpp.DrawingLayer.DBMP.pSet(x, y, GetRBColorFromColorPickerGradient(cpp, t));
                                }
                            }
                        }
                    }
                }

                Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
                g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
                cpp.DrawingLayer.DBMP.clear(0);
                g.Dispose();
                cpp.Invalidate();
            }
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
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

        private bool ColorIsWithinTolerance(CanvasPlanePanel cpp, int x, int y)
        {
            byte[] baseColorBytes = BitConverter.GetBytes(baseColor);
            byte[] testColorBytes = BitConverter.GetBytes(cpp.GetActiveLayer().DBMP.pGet(x, y));

            int rDelta = Math.Abs(((int)testColorBytes[2]) - ((int)baseColorBytes[2]));
            int gDelta = Math.Abs(((int)testColorBytes[1]) - ((int)baseColorBytes[1]));
            int bDelta = Math.Abs(((int)testColorBytes[0]) - ((int)baseColorBytes[0]));

            return (rDelta <= rTolerance) && (gDelta <= gTolerance) && (bDelta <= bTolerance);
        }

        private bool PixelIsWithinImageBounds(CanvasPlanePanel cpp, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < cpp.CanvasWidth && y < cpp.CanvasHeight)
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool PixelIsAllowed(CanvasPlanePanel cpp, int x, int y)
        {
            return PixelIsWithinImageBounds(cpp, x, y) &&
                ColorIsWithinTolerance(cpp, x, y) &&
                ((cpp.DrawingLayer.DBMP.pGet(x, y) & PRIMER_FLAG) == 0);
        }

        private void FlagRadialDistanceFromOrigin(CanvasPlanePanel cpp, int x, int y)
        {
            int dist = (int)Math.Sqrt(Math.Pow((x - origin.X), 2) + Math.Pow((y - origin.Y), 2));
            if (dist > floodWidth)
            {
                floodWidth = dist;
            }
            cpp.DrawingLayer.DBMP.pSet(x, y, (UInt32)(PRIMER_FLAG | (dist & MAX_DIST)));
        }

        private void FlagProjectionOntoAxis(CanvasPlanePanel cpp, int x, int y)
        {

        }

        private void RadialPrimerFill(CanvasPlanePanel cpp, int x, int y)
        {
            if (PixelIsAllowed(cpp, x, y))
            {
                int e, w;
                e = w = x;
                FlagRadialDistanceFromOrigin(cpp, x, y);
                //east
                while (PixelIsAllowed(cpp, e-1, y))
                {
                    FlagRadialDistanceFromOrigin(cpp, e - 1, y);
                    e--;
                }
                //west
                while (PixelIsAllowed(cpp, w+1, y))
                {
                    FlagRadialDistanceFromOrigin(cpp, w + 1, y);
                    w++;
                }
                //east->west
                for (int p = e; p <= w; p++)
                {
                    RadialPrimerFill(cpp, p, y + 1);
                    RadialPrimerFill(cpp, p, y - 1);
                }
            }
        }

        private  void SolidPrimerFill(CanvasPlanePanel cpp, int x, int y)
        {
            if (PixelIsAllowed(cpp, x, y))
            {
                int e, w;
                e = w = x;
                cpp.DrawingLayer.DBMP.pSet(x, y, (UInt32)(PRIMER_FLAG));
                //east
                while (PixelIsAllowed(cpp, e - 1, y))
                {
                    cpp.DrawingLayer.DBMP.pSet(e - 1, y, (UInt32)(PRIMER_FLAG));
                    e--;
                }
                //west
                while (PixelIsAllowed(cpp, w + 1, y))
                {
                    cpp.DrawingLayer.DBMP.pSet(w + 1, y, (UInt32)(PRIMER_FLAG));
                    w++;
                }
                //east->west
                for (int p = e; p <= w; p++)
                {
                    SolidPrimerFill(cpp, p, y + 1);
                    SolidPrimerFill(cpp, p, y - 1);
                }
            }
        }

        private UInt32 GetLBColorFromColorPickerGradient(CanvasPlanePanel cpp, double t)
        {
            UInt32 c = cpp.MyColorPicker.GetInterpolatedColor((byte)(255.0 * t), cpp.MyColorPicker.C1, cpp.MyColorPicker.C2);
            c &= 0x00ffffff;
            c |= (UInt32)(cpp.MyColorPicker.TrackBarOpacity.Value << 24);
            return c;
        }
        private UInt32 GetRBColorFromColorPickerGradient(CanvasPlanePanel cpp, double t)
        {
            UInt32 c = cpp.MyColorPicker.GetInterpolatedColor((byte)(255.0 * t), cpp.MyColorPicker.C2, cpp.MyColorPicker.C1);
            c &= 0x00ffffff;
            c |= (UInt32)(cpp.MyColorPicker.TrackBarOpacity.Value << 24);
            return c;
        }
        private double ApplyNoiseToColorInterpolationParamater(double t)
        {
            double tNoise = t + myRandom.NextDouble() * (2.0 * noise) - noise;
            if (tNoise > 1.0)
            {
                tNoise = 1.0;
            }
            if (tNoise < 0)
            {
                tNoise = 0.0;
            }
            return tNoise;
        }

    }
}
