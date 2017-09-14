using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Seurat
{
    class LayerTabPanel : TabControl
    {
        private TabPage m_DraggedTab;

        public LayerTabPanel()
        {
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            m_DraggedTab = TabAt(e.Location);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || m_DraggedTab == null || m_DraggedTab.Name == "tabPageAddLayer")
            {
                return;
            }

            TabPage tab = TabAt(e.Location);

            if (tab == null || tab == m_DraggedTab || tab.Name == "tabPageAddLayer")
            {
                return;
            }

            Swap(m_DraggedTab, tab);
            SelectedTab = m_DraggedTab;
        }

        private TabPage TabAt(Point position)
        {
            int count = TabCount;

            for (int i = 0; i < count; i++)
            {
                if (GetTabRect(i).Contains(position))
                {
                    return TabPages[i];
                }
            }

            return null;
        }

        private void Swap(TabPage a, TabPage b)
        {
            int i = TabPages.IndexOf(a);
            int j = TabPages.IndexOf(b);
            TabPages[i] = b;
            TabPages[j] = a;
        }
    }
}
