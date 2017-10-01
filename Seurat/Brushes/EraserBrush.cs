using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    class EraserBrush : BrushType
    {
        int[] previousCoordinates1 = null;
        private Pen myPen = new Pen(Color.Black);
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private TrackBar myWidthTrackBar;

        public EraserBrush()
        {
            Image = Properties.Resources.if_package_purge_24217;
            myPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            myPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();

            ControlPanelControlStack c = new ControlPanelControlStack();
            c.AddHeaderLabel("Eraser", Color.White, Color.SkyBlue);
            c.AddLabelTrackBar("Width", 1, 25, (int)myPen.Width);

            ((TrackBar)c.StackControls[1]).ValueChanged += (object sender, EventArgs e) =>
            {
                TrackBar tb = (TrackBar)sender;
                myPen.Width = tb.Value;
            };
            myWidthTrackBar = (TrackBar)c.StackControls[1];
            p.Controls.Add(c);
            return p;
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {           
            cpp.DrawingLayer.DBMP.clear(0);
            cpp.DrawingLayer.isVisible = true;
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cpp.DrawingLayer.isVisible = false;
                previousCoordinates1 = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            }
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cpp.DrawingLayer.DBMP.clear(0);
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                g.DrawLine(myPen, previousCoordinates1[0], previousCoordinates1[1], currentCoordinates[0], currentCoordinates[1]);
                DirectBitmap activeDBMP = cpp.GetActiveLayer().DBMP;
                g.Dispose();

                //find smallest rectangular frame containing stroke
                int x1 = (int)(Math.Min(previousCoordinates1[0], currentCoordinates[0]) - myPen.Width);
                int y1 = (int)(Math.Min(previousCoordinates1[1], currentCoordinates[1]) - myPen.Width);
                int x2 = (int)(Math.Max(previousCoordinates1[0], currentCoordinates[0]) + myPen.Width);
                int y2 = (int)(Math.Max(previousCoordinates1[1], currentCoordinates[1]) + myPen.Width);

                //iterate over stencil rectangle and set pixels directly
                for (int y = y1; y < y2; y++)
                {
                    for (int x = x1; x < x2; x++)
                    {
                        if ((x >= 0) && (x < cpp.CanvasWidth) && (y >= 0) && (y < cpp.CanvasHeight))
                        {
                            if (cpp.DrawingLayer.DBMP.pGet(x,y) != 0)
                            {
                                activeDBMP.pSet(x, y, 0);
                            }
                        }                          
                    }
                }

                previousCoordinates1 = currentCoordinates;
                cpp.Invalidate();
            }
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
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
                } else
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
