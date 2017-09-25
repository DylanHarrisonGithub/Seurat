using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Seurat
{
    class PencilBrush : BrushType
    {
        int[] previousCoordinates1 = { 0, 0 };
        private Pen MyPen;
        private GraphicsPath myPath;
        public override Cursor MyCursor { get => Cursors.Arrow; }

        public PencilBrush()
        {
            Image = Properties.Resources.if_pencil_285638;
            MyPen = new Pen(Color.Black);
            MyPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            MyPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            MyPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round ;
        }

        

        public override Panel ControlPanel()
        {
            Panel p = new Panel();

            ControlPanelControlStack c = new ControlPanelControlStack();
            c.AddHeaderLabel("Pencil", Color.White, Color.SkyBlue);
            c.AddLabelTrackBar("Width", 1, 25, (int) MyPen.Width);
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
                }, MyPen.StartCap.ToString());

            p.Controls.Add(c);
            
            ((TrackBar)c.StackControls[1]).ValueChanged += (object sender, EventArgs e) => 
            {
                TrackBar tb = (TrackBar)sender;
                MyPen.Width = tb.Value;
            };
            ((ComboBox)c.StackControls[2]).SelectionChangeCommitted += (object sender, EventArgs e) => 
            {
                string sv = ((ComboBox)sender).SelectedItem.ToString();
                switch(sv)
                {
                    case "AnchorMask":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.AnchorMask;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.AnchorMask;
                        break;
                    case "ArrowAnchor":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                        break;
                    case "DiamondAnchor":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
                        break;
                    case "Flat":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                        break;
                    case "Round":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        break;
                    case "RoundAnchor":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                        break;
                    case "Square":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                        break;
                    case "SquareAnchor":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.SquareAnchor;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.SquareAnchor;
                        break;
                    case "Triangle":
                        MyPen.StartCap = System.Drawing.Drawing2D.LineCap.Triangle;
                        MyPen.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
                        break;
                    default:
                        break;
                }
            };
            return p;
        }


        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e) { }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            previousCoordinates1 = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            myPath = new GraphicsPath();
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left )
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                myPath.AddLine(
                    new Point(previousCoordinates1[0], previousCoordinates1[1]),
                    new Point(currentCoordinates[0], currentCoordinates[1]));

                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                MyPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C1);

                g.Clear(Color.FromArgb(0x00000000));
                g.DrawPath(MyPen, myPath);

                previousCoordinates1 = currentCoordinates;
                cpp.Invalidate();
            }
            if (e.Button == MouseButtons.Right)
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                myPath.AddLine(
                    new Point(previousCoordinates1[0], previousCoordinates1[1]),
                    new Point(currentCoordinates[0], currentCoordinates[1]));

                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                MyPen.Color = Color.FromArgb((int)cpp.MyColorPicker.C2);

                g.Clear(Color.FromArgb(0x00000000));
                g.DrawPath(MyPen, myPath);

                previousCoordinates1 = currentCoordinates;
                cpp.Invalidate();
            }
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(cpp.GetActiveLayer().DBMP.bmp);
            g.DrawImage(cpp.DrawingLayer.DBMP.bmp, new Point(0, 0));
            cpp.DrawingLayer.DBMP.clear(0x00000000);
            g.Dispose();
            myPath.Dispose();
            cpp.Invalidate();
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }
    }
}
