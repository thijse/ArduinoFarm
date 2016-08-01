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
    public partial class FindWindowForm : Form
    {
        private class WinStruct
        {
            public string name="";
            public string className = "";
            public string handle = "";
        };
        public FindWindowForm()
        {
            InitializeComponent();
            UpdateWindowsCollection();
        }
        bool EnumWindowsProc(IntPtr hWnd, IntPtr parameter)
        {
            WinStruct wStruct = new WinStruct();
            CWindow w = hWnd;
            if (w.IsHung)
                wStruct.name = w.Text;
            else
                wStruct.name = w.TextUnsafe;
            wStruct.className = w.ClassName;
            wStruct.handle = w.Handle.ToString("X");
            wList.Add(wStruct);
            return true;
        }
        List<WinStruct> wList = new List<WinStruct>();
        object _SyncObj = new object();
        public CWindow activeWindow = 0;
        private void UpdateWindowsCollection()
        {
            lock (_SyncObj)
            {
                wList.Clear();
                CWindow.DesktopWindow.EnumChildWindows(EnumWindowsProc, IntPtr.Zero);
            }
            UpdateListBox();
        }
        private void UpdateListBox()
        {
            lock (_SyncObj)
            {
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                foreach (WinStruct str in wList)
                {
                    if (str.handle.IndexOf(handleTB.Text, StringComparison.InvariantCultureIgnoreCase) >= 0 &
                        str.name.IndexOf(nameTB.Text, StringComparison.InvariantCultureIgnoreCase) >= 0 &
                        str.className.IndexOf(classTB.Text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        listBox1.Items.Add("[" + str.handle + "] \"" + str.name + "\" {" + str.className + "}");
                }
                if (listBox1.Items.Count >0)
                    listBox1.SelectedIndex = 0;
                listBox1.EndUpdate();
            }
        }
        private void FindWindowForm_Shown(object sender, EventArgs e)
        {
            
/*

            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();
            for (int k = 0; k < 10 ; k++)
            {
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                for (int i = 0; i < 3000; i++)
                {
                    listBox1.Items.Add("asdfkja;lskjf asdf asdf;lkjasd;lkfj");
                }
                listBox1.EndUpdate();           	
            }
            w.Stop();
            MessageBox.Show(w.Elapsed.ToString());*/

        }

        string tClass, tHandle, tName = "";
        private void updateListBoxTimer_Tick(object sender, EventArgs e)
        {
            if (tClass == classTB.Text && tHandle == handleTB.Text && tName == nameTB.Text)
                return;
            tClass = classTB.Text ;
            tHandle = handleTB.Text ;
            tName = nameTB.Text;
            UpdateListBox();
        }
        private void FindWindowForm_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.KeyCode)
            {
            	case Keys.Up:
                    if (listBox1.SelectedIndex > 1)
                        listBox1.SelectedIndex--;
            		break;
                case Keys.Down: 
                    if (listBox1.SelectedIndex < listBox1.Items.Count-1)
                        listBox1.SelectedIndex++;
            		break;
            	case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
            		break;
                case Keys.Enter:
                    listBox1_DoubleClick(this,null);
            		break;
            	default:
                    e.Handled = false;
            		break;
            }
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedWindow != 0)
            {
                activeWindow = SelectedWindow;
                this.DialogResult = DialogResult.OK;
            }
            else
                this.DialogResult = DialogResult.Cancel;
        }
        private CWindow GetHandleFromListBoxItem(int itemIndex)
        {
            if (itemIndex >= listBox1.Items.Count|| itemIndex<0)
                return 0;
            string curHand = ((string)listBox1.Items[itemIndex]);
            curHand = curHand.Substring(1, curHand.IndexOf("]") - 1);
            int t;
            if (int.TryParse(curHand, System.Globalization.NumberStyles.HexNumber, null, out t))
                return t;
            return 0;
        }
        private CWindow SelectedWindow
        {
            get
            {
                return GetHandleFromListBoxItem(listBox1.SelectedIndex);
            }
        }
        private void MinimizeB_Click(object sender, EventArgs e)
        {
            SelectedWindow.Minimize();
            listBox1_SelectedIndexChanged(this, null);
        }
        private void RestoreB_Click(object sender, EventArgs e)
        {
            SelectedWindow.WindowState = FormWindowState.Normal;
            listBox1_SelectedIndexChanged(this, null);
        }
        private void MaximizeB_Click(object sender, EventArgs e)
        {
            SelectedWindow.WindowState = FormWindowState.Maximized;
            listBox1_SelectedIndexChanged(this, null);
        }
        private void CloseB_Click(object sender, EventArgs e)
        {
            SelectedWindow.Close();
            int selected = listBox1.SelectedIndex;
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            if (selected - 1 > 0)
                selected--;
            else if (listBox1.Items.Count < 1)
                selected = -1;
            listBox1.SelectedIndex = selected;
        }

        private void VisibleB_Click(object sender, EventArgs e)
        {
            SelectedWindow.Visible = !SelectedWindow.Visible;
            listBox1_SelectedIndexChanged(this, null);
        }
        private void EnableB_Click(object sender, EventArgs e)
        {
            SelectedWindow.Enabled = !SelectedWindow.Enabled;
            listBox1_SelectedIndexChanged(this, null);
        }
        private void UpdateIcons()
        {
            MinimizeB.Enabled = SelectedWindow.WindowState != FormWindowState.Minimized;
            RestoreB.Enabled = SelectedWindow.WindowState!= FormWindowState.Normal;
            MaximizeB.Enabled = SelectedWindow.WindowState != FormWindowState.Maximized;
            CloseB.Enabled = SelectedWindow.Exists;
            if (SelectedWindow.Enabled)
                EnableB.Image = IuSpy.Properties.Resources.Enabled16;
            else
                EnableB.Image = IuSpy.Properties.Resources.Disabled16;
            if (SelectedWindow.Visible)
                VisibleB.Image = IuSpy.Properties.Resources.Visible16;
            else
                VisibleB.Image = IuSpy.Properties.Resources.Ivisible16;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateIcons();            
        }

        private void updateIconsTimer_Tick(object sender, EventArgs e)
        {
            UpdateIcons();   
        }
    }
}