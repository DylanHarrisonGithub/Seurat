using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class PencilBrush : BrushType
    {
        int[] previousCoordinates1 = { 0, 0 };

        public PencilBrush()
        {
            Image = Properties.Resources.if_pencil_285638;
        }

        public override Panel ControlPanel()
        {
            return new Panel();
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e) { }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            previousCoordinates1 = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });            
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left )
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                Pen p = new Pen(Color.FromArgb((int)cpp.MyColorPicker.C1), 5.0f);
                p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(p,
                    new Point(previousCoordinates1[0], previousCoordinates1[1]),
                    new Point(currentCoordinates[0], currentCoordinates[1]));
                g.Dispose();
                
                previousCoordinates1 = currentCoordinates;
                cpp.Invalidate();
            }
            if (e.Button == MouseButtons.Right)
            {
                int[] currentCoordinates = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
                Graphics g = Graphics.FromImage(cpp.DrawingLayer.DBMP.bmp);
                Pen p = new Pen(Color.FromArgb((int)cpp.MyColorPicker.C2), 5.0f);
                p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(p,
                    new Point(previousCoordinates1[0], previousCoordinates1[1]),
                    new Point(currentCoordinates[0], currentCoordinates[1]));
                g.Dispose();

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
            cpp.Invalidate();
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }
    }
}
