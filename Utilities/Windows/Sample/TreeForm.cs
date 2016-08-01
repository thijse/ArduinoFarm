using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Iu.Windows;

namespace IuSpy
{
    public partial class TreeForm : Form
    {
        List<string> classes = new List<string>();
        List<string> notFound = new List<string>();
        bool updateBegan = false;
        public TreeForm()
        {
            InitializeComponent();           
        }
        private void TreeForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                UpdateTree();
        }
        public TreeNode AddWinNode(TreeNode parentNode, CWindow window)
        {
            string winHandle = window.Handle.ToString("X");
            string winClass = window.ClassName;
            string winText;
            if (window.IsHung)
                winText = window.Text;
            else
                winText = window.TextUnsafe;
            string text = "[" + winHandle + "] \"" + winText + "\" {" + winClass + "}";
            string imKey = "NoIcon";
            Icon ic;

            if (!window.IsHung)
                if (imList.Images.ContainsKey(winHandle))
                    imKey = winHandle;
                else if ((ic = TestIcon(window.SmallIcon)) != null)
                {
                    imKey = winHandle;
                    imList.Images.Add(imKey, ic);
                }
                else if (imList.Images.ContainsKey(winClass))
                    imKey = winClass;
                else
                    if (!classes.Contains(winClass))
                        if ((ic = TestIcon(window.SmallClassIcon)) != null)
                        {
                            classes.Add(winClass);
                            imKey = winClass;
                            imList.Images.Add(imKey, ic);
                        }

            if (parentNode == null)
                return treeView1.Nodes.Add(winHandle, text, imKey);
            else
                return parentNode.Nodes.Add(winHandle, text, imKey);
        }
        private Icon TestIcon(Icon ic)
        {
            if (ic != null)
                if (ic.Height == 16 && ic.Width==16)
                    return ic;
            return null;
        }
        public void UpdateTree()
        {
            updateBegan = true;
            treeView1.ImageList = null;
            //classes.Clear();
            //imList.Images.Clear();
            treeView1.Nodes.Clear();
            if (!imList.Images.ContainsKey("NoIcon"))
                imList.Images.Add("NoIcon", Properties.Resources.NoImage16);

            TreeNode rootNode = AddWinNode(null, CWindow.DesktopWindow);
            List<CWindow> topWins = CWindow.TopLevelWindows;
            foreach (CWindow win in topWins)
            {
                GetNext(win, AddWinNode(rootNode, win));
            }
            treeView1.ImageList = imList;
            UpdateTreeViewIcons();
            TrySelectNode(((MainForm)this.Owner).curWin);
            updateBegan = false;
        }
        public bool TrySelectNode(CWindow window)
        {
            string selHandle = window.Handle.ToString("X");
            TreeNode[] nods = treeView1.Nodes.Find(selHandle, true);
            if(nods.Length==0&&window.Handle != IntPtr.Zero&&!updateBegan&&!notFound.Contains(selHandle))
            {
                UpdateTree();
                nods = treeView1.Nodes.Find(selHandle, true);
            }
            if (nods.Length > 0)
            {
                TreeNode nod = nods[0];
                if (nod != null)
                {
                    treeView1.SelectedNode = nod;
                    return true;
                }
            }
            else
                notFound.Add(selHandle);
            return false;
        }

        public void UpdateTreeViewIcons()
        {
            UpdateNodes(treeView1.Nodes);
        }
        private void UpdateNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                UpdateNodes(node.Nodes);
                if (node.ImageKey != "NoIcon")
                    node.ImageKey = node.ImageKey;
            }
        }
        public void GetNext(CWindow win,TreeNode parentNode)
        {
            List<CWindow> wins = win.Children;
            foreach (CWindow curWin in wins)
            {
                GetNext(curWin, AddWinNode(parentNode, curWin));
            }
        }

        private void treeView1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                UpdateTree();
        }
        public CWindow GetWindow(string hexHandle)
        {
            int handle;
            if (!int.TryParse(hexHandle, System.Globalization.NumberStyles.HexNumber, null, out handle))
                return null;
            return handle;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ((MainForm)this.Owner).catcher1_onWindowChanged(GetWindow(e.Node.Name));
            e.Node.SelectedImageKey = e.Node.ImageKey;
        }
    }
}