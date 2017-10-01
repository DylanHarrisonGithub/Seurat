using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    class LineBrush: BrushType
    {
        private Pen myPen = new Pen(Color.Black);
        public override Cursor MyCursor { get => Cursors.Arrow; }
        private int[] endPoint = null;

        public LineBrush()
        {
            Image = Properties.Resources.if_minus_214643;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();

            ControlPanelControlStack c = new ControlPanelControlStack();
            c.AddHeaderLabel("Line", Color.White, Color.DarkViolet);
            c.AddLabelTrackBar("Width", 1, 25, (int)myPen.Width);
            c.AddLabelComboBox("Cap", new string[]
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

            ((TrackBar)c.StackControls[1]).ValueChanged += (object sender, EventArgs e) =>
            {
                TrackBar tb = (TrackBar)sender;
                myPen.Width = tb.Value;
            };
            ((ComboBox)c.StackControls[2]).SelectionChangeCommitted += (object sender, EventArgs e) =>
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

            p.Controls.Add(c);
            return p;
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (endPoint == null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C1);
                        break;
                    case MouseButtons.Right:
                        myPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C2);
                        break;
                    default:
                        break;
                }
                endPoint = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            } else
            {
                Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
                g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
                cpp.DrawingLayer.DBMP.clear(0x00000000);
                g.Dispose();
                cpp.Invalidate();
                endPoint = null;
            }
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (endPoint != null)
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                g.Clear(Color.FromArgb(0));
                g.DrawLine(myPen, endPoint[0], endPoint[1], currentCoordinates[0], currentCoordinates[1]);
                cpp.Invalidate();
            }
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }
    }
}
