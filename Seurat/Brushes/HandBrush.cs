using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class HandBrush : BrushType
    {

        public override Cursor MyCursor { get => Cursors.Hand; }

        public HandBrush()
        {
            this.Image = Properties.Resources.if_icon_3_high_five_329409;
        }

        

    public override Panel ControlPanel()
        {            
            Panel p = new Panel();
            ControlPanelControlStack s = new ControlPanelControlStack();
            s.AddHeaderLabel("Hand", System.Drawing.Color.White, System.Drawing.Color.DarkViolet);
            s.AddHeaderLabel("", System.Drawing.Color.FromArgb(0), System.Drawing.Color.FromArgb(0));
            p.Controls.Add(s);
            p.AutoSize = true;
            return p;
        }

        public override void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            
        }

        public override void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cpp.LBDown = true;
                cpp.PreviousMouse = cpp.PanelToPlaneCoordinates(new double[] { e.X, e.Y });
            }
            if (e.Button == MouseButtons.Right)
            {
                cpp.RBDown = true;
                cpp.Center = cpp.PanelToPlaneCoordinates(new double[] { e.X, e.Y });
                cpp.Refresh();
            }
        }

        public override void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            cpp.MousePanel[0] = e.X;
            cpp.MousePanel[1] = e.Y;
            cpp.MousePlane = cpp.PanelToPlaneCoordinates(cpp.MousePanel);
            if (cpp.LBDown)
            {
                cpp.Center[0] -= (cpp.MousePlane[0] - cpp.PreviousMouse[0]);
                cpp.Center[1] -= (cpp.MousePlane[1] - cpp.PreviousMouse[1]);
                cpp.Refresh();
            }
        }

        public override void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e)
        {            
            if (e.Button == MouseButtons.Left)
            {
                cpp.LBDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                cpp.RBDown = false;
            }
        }

        public override void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                cpp.Zoom *= (1.05);
                cpp.ZoomInverse = 1.0 / cpp.Zoom;
            }
            else
            {
                cpp.Zoom *= (0.95);
                cpp.ZoomInverse = 1.0 / cpp.Zoom;
            }
            cpp.Refresh();
        }
    }
}
