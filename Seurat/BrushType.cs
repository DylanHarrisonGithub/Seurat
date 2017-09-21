using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    abstract class BrushType : ToolStripButton
    {

        abstract public Cursor MyCursor { get; }
        abstract public Panel ControlPanel();

        abstract public void MouseUp(CanvasPlanePanel cpp, MouseEventArgs e);
        abstract public void MouseDown(CanvasPlanePanel cpp, MouseEventArgs e);
        abstract public void MouseMove(CanvasPlanePanel cpp, MouseEventArgs e);
        abstract public void MouseClick(CanvasPlanePanel cpp, MouseEventArgs e);
        abstract public void MouseWheel(CanvasPlanePanel cpp, MouseEventArgs e);

    }
}
