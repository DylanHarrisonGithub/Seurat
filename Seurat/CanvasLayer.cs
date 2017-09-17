using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seurat
{
    class CanvasLayer
    {

        public string Name;
        public bool isVisible = true;
        public bool isActiveLayer = false;
        public DirectBitmap DBMP = null;

        public CanvasLayer(string name, bool visible, int width, int height)
        {
            Name = name;
            isVisible = visible;
            DBMP = new DirectBitmap(width, height);
        }
    }
}
