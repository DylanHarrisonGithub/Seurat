using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seurat
{
    class MouseEventBundle
    {
        public MouseEventHandler MouseUp;
        public MouseEventHandler MouseDown;
        public MouseEventHandler MouseMove;
        public MouseEventHandler MouseClick;
        public MouseEventHandler MouseWheel;
    }
}
