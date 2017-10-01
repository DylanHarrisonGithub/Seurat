using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Seurat
{
    class CurveBrush : BrushType
    {
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private Pen myPen = new Pen(Color.Black);
        private TrackBar myWidthTrackBar;
        private List<Point> nodes;
        private bool isC1;

        public CurveBrush()
        {
            Image = Properties.Resources.if_Curve_131723;
            myPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Curve", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddLabelTrackBar("Width", 1, 25, 1);
            s.AddLabelComboBox("Cap", new string[]
                {"AnchorMask",
                "ArrowAnchor",
                "DiamondAnchor",
                "Flat",
                "Round",
                "RoundAnchor",
                "Square",
                "SquareAnchor",
                "Triangle"
                }, myPen.StartCap.ToString());

            myWidthTrackBar = (TrackBar)s.StackControls[1];
            ((TrackBar)s.StackControls[1]).ValueChanged += (object sender, EventArgs e) =>
            {
                myPen.Width = ((TrackBar)sender).Value;
            };
            ((ComboBox)s.StackControls[2]).SelectionChangeCommitted += (object sender, EventArgs e) =>
            {
                string sv = ((ComboBox)sender).SelectedItem.ToString();
                switch (sv)
                {
                    case "AnchorMask":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.AnchorMask;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.AnchorMask;
                        break;
                    case "ArrowAnchor":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        break;
                    case "DiamondAnchor":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
                        break;
                    case "Flat":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                        break;
                    case "Round":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        break;
                    case "RoundAnchor":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                        break;
                    case "Square":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                        break;
                    case "SquareAnchor":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.SquareAnchor;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor;
                        break;
                    case "Triangle":
                        myPen.StartCap = System.Drawing.Drawing2D.LineCap.Triangle;
                        myPen.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
                        break;
                    default:
                        break;
                }
            };
            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (nodes == null)
            {
                if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            isC1 = true;
                            myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C1);
                            break;
                        case MouseButtons.Right:
                            isC1 = false;
                            myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C2);
                            break;
                    }
                    nodes = new List<Point>();
                    int[] startCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                    nodes.Add(new Point(startCoord[0], startCoord[1]));
                }
            } else
            {
                if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (isC1)
                            {
                                int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                                nodes.Add(new Point(thisCoord[0], thisCoord[1]));
                            } else
                            {
                                Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
                                g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
                                cpp.DrawingLayer.DBMP.clear(0x00000000);
                                g.Dispose();
                                nodes = null;
                                cpp.Invalidate();
                            }
                            break;
                        case MouseButtons.Right:
                            if (!isC1)
                            {
                                int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                                nodes.Add(new Point(thisCoord[0], thisCoord[1]));
                            } else
                            {
                                Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
                                g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
                                cpp.DrawingLayer.DBMP.clear(0x00000000);
                                g.Dispose();
                                nodes = null;
                                cpp.Invalidate();
                            }
                            break;
                    }
                }
            }
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (nodes != null)
            {
                int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                nodes.Add(new Point(thisCoord[0], thisCoord[1]));

                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                g.Clear(Color.FromArgb(0));
                g.DrawCurve(myPen, nodes.ToArray());

                nodes.RemoveAt(nodes.Count - 1);
                cpp.Invalidate();
            }
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
    }
}
