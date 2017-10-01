using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class DropperBrush : BrushType
    {
        public override Cursor MyCursor { get => Cursors.Arrow; }

        public DropperBrush()
        {
            Image = Properties.Resources.if_eyedropper_84569;
        }

        public override Panel ControlPanel()
        {
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Dropper", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            if (thisCoord[0] >= 0 && thisCoord[1] >= 0 && thisCoord[0] < cpp.CanvasWidth && thisCoord[1] < cpp.CanvasHeight)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        cpp.MyColorPicker.setC1(cpp.GetActiveLayer().DBMP.pGet(thisCoord[0], thisCoord[1]));
                        break;
                    case MouseButtons.Right:
                        cpp.MyColorPicker.setC2(cpp.GetActiveLayer().DBMP.pGet(thisCoord[0], thisCoord[1]));
                        break;
                }
            }
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            int[] thisCoord = cpp.CanvasPlanePanelToCanvasCoordinates(new double[] { e.X, e.Y });
            if (thisCoord[0] >= 0 && thisCoord[1] >= 0 && thisCoord[0] < cpp.CanvasWidth && thisCoord[1] < cpp.CanvasHeight)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        cpp.MyColorPicker.setC1(cpp.GetActiveLayer().DBMP.pGet(thisCoord[0], thisCoord[1]));
                        break;
                    case MouseButtons.Right:
                        cpp.MyColorPicker.setC2(cpp.GetActiveLayer().DBMP.pGet(thisCoord[0], thisCoord[1]));
                        break;
                }
            }
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
        }
    }
}
