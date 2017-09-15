using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    abstract class BrushType
    {
        Image Icon;
        Image Cursor;
        String Name;
        String ToolTip;

        abstract public MouseEventBundle MouseEvents();
        abstract public Panel ControlPanel();
    }
}
