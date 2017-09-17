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
        private CanvasPlanePanel DelegateCanvasPlanePanel = null;

        public LayerTabPanel()
        {
            //tabs must have unique names
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            this.Selecting += (object sender, TabControlCancelEventArgs e) => {
                if (e.TabPage != null)
                {
                    if (e.TabPage.Name == "tabPageAddLayer")
                    {
                        TabPage NewTab = new TabPage();
                        NewTab.Name = "Layer" + TabPages.Count.ToString();
                        NewTab.Text = NewTab.Name;
                        TabPages.Insert(TabPages.Count - 1, NewTab);
                        e.Cancel = true;

                        if (DelegateCanvasPlanePanel != null)
                        {
                            DelegateCanvasPlanePanel.Layers.Add(new CanvasLayer(
                                NewTab.Name,
                                true,
                                DelegateCanvasPlanePanel.Width,
                                DelegateCanvasPlanePanel.Height));
                        }
                    }
                }
            };
            this.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                if (DelegateCanvasPlanePanel != null && SelectedTab != null)
                {
                    foreach (CanvasLayer l in DelegateCanvasPlanePanel.Layers) {
                        if (l.Name == SelectedTab.Name)
                        {
                            l.isActiveLayer = true;
                        } else
                        {
                            l.isActiveLayer = false;
                        }
                    }
                   
                }
            };
            
        }

        public void SetDelegateCanvasPlanePanel(CanvasPlanePanel cpp)
        {
            DelegateCanvasPlanePanel = cpp;
            TabPages.Clear();
            
            foreach (CanvasLayer l in DelegateCanvasPlanePanel.Layers)
            {
                TabPages.Add(l.Name);
            }
            TabPage t = new TabPage();
            t.Name = "tabPageAddLayer";
            t.Text = "+";
            TabPages.Add(t);
            this.Invalidate();
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

            if (DelegateCanvasPlanePanel != null)
            {
                //danger!!!               
                CanvasLayer tempSwap = DelegateCanvasPlanePanel.Layers[i];
                DelegateCanvasPlanePanel.Layers[i] = DelegateCanvasPlanePanel.Layers[j];
                DelegateCanvasPlanePanel.Layers[j] = tempSwap;
                DelegateCanvasPlanePanel.Invalidate();
                //MessageBox.Show("swapped");
            }
        }
    }
}
