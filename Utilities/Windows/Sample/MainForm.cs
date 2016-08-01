using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Iu.Windows;
using System.Threading;

namespace IuSpy
{
    public partial class MainForm : Form
    {
        Highlighter highlighter = new Highlighter();
        bool changing = false;
        public CWindow curWin;
        TreeForm treeForm = new TreeForm();

        public MainForm()
        {
            InitializeComponent();
            curWin = 0;
            UpdateWinStateButtons();
            this.Top = 0;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            //Form.
        }

        public void catcher1_onWindowChanged(Iu.Windows.CWindow window)
        {
            changing = true;
            curWin = window;
            if (!window.Exists)
            {
                HandleTB.ForeColor = Color.Red;
                changing = false;
                return;
            }
            else
                HandleTB.ForeColor = Color.Black;
            HandleTB.Text = window.Handle.ToString("X");
            MenuTB.Text = (window.Menu != IntPtr.Zero).ToString();
            if (window.IsHung)
                WindowTextTB.Text = window.Text;
            else
                WindowTextTB.Text = window.TextUnsafe;
            ClassNameTB.Text = window.ClassName;
            DeskLocationX.Value = window.DesktopLocation.X;
            DeskLocationY.Value = window.DesktopLocation.Y;
            ClientLocationX.Value = window.Location.X;
            ClientLocationY.Value = window.Location.Y;
            WinSizeW.Value = window.Size.Width;
            WinSizeH.Value = window.Size.Height;
            ClientW.Value = window.ClientSize.Width;
            ClientH.Value = window.ClientSize.Height;

            ParentNW.TextTB = window.Parent.Handle.ToString("X");
            OwnerNW.TextTB = window.Owner.Handle.ToString("X");
            PreviousNW.TextTB = window.GetNextWindow(GetWindow_Cmd.GW_HWNDPREV).Handle.ToString("X");
            ChildNW.TextTB = window.TopChild.Handle.ToString("X");
            NextNW.TextTB = window.GetNextWindow(GetWindow_Cmd.GW_HWNDNEXT).Handle.ToString("X");

            Icon ic=null;
            if (!window.IsHung)
                ic = window.Icon;
            winIcon.Image = (ic != null) ? ic.ToBitmap() : Properties.Resources.NoImage32;
            if (!window.IsHung)
                ic = window.SmallIcon;
            winIconSmall.Image = (ic != null) ? ic.ToBitmap() : Properties.Resources.NoImage16;
            winClassIcon.Image = (window.ClassIcon != null) ? window.ClassIcon.ToBitmap() : Properties.Resources.NoImage32;
            winClassIconSmall.Image = (window.SmallClassIcon != null) ? window.SmallClassIcon.ToBitmap() : Properties.Resources.NoImage16;

            StylesTB.Text = ((uint)(IntPtr)window.Styles).ToString("X");
            ExStylesTB.Text = ((uint)(IntPtr)window.ExStyles).ToString("X");

            UpdateWinStateButtons();

            changing = false;
            if (treeForm.Visible)
                treeForm.TrySelectNode(window);
        }
        private void Coordinates_ValueChanged(object sender, EventArgs e)
        {
            if (changing)
                return;
            int val = (int)((NumericUpDown)sender).Value;
            switch (((NumericUpDown)sender).Name)
            {
                case "DeskLocationX": curWin.DesktopLocation = new Point(val, curWin.DesktopLocation.Y);
                    break;
                case "DeskLocationY": curWin.DesktopLocation = new Point(curWin.DesktopLocation.X, val);
                    break;
                case "ClientLocationX": curWin.Location = new Point(val, curWin.Location.Y);
                    break;
                case "ClientLocationY": curWin.Location = new Point(curWin.Location.X, val);
                    break;
                case "WinSizeW": curWin.Size = new Size(val, curWin.Size.Height);
                    break;
                case "WinSizeH": curWin.Size = new Size(curWin.Size.Width, val);
                    break;
                case "ClientW": curWin.ClientSize = new Size(val, curWin.ClientSize.Height);
                    break;
                case "ClientH": curWin.ClientSize = new Size(curWin.ClientSize.Width, val);
                    break;
            }
        }
        private void WindowTreeB_Click(object sender, EventArgs e)
        {
            if (!treeForm.Created)
            {
                treeForm.Dispose();
                treeForm = new TreeForm();
            }
            if (treeForm.Visible)
            {
                treeForm.WindowState = FormWindowState.Normal;
                treeForm.Activate();
            }
            else
                treeForm.Show(this);
        }
        private void NextWindow_Click(object sender, EventArgs e)
        {
            string text = ((Button)sender).Name;
            text = text.Substring(0, text.Length - 1) + "NW";
            text = ((NextWindow)this.Controls[text]).HandleTB.Text;
            int handle;
            if (int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out handle))
            {
                if (handle == 0)
                    return;
                catcher1_onWindowChanged(handle);
            }
        }
        public void Highlight(CWindow window)
        {
            if (!this.Created || highlighter.Visible)
                return;
            highlighter.SizeFromWindow(window);
            highlighter.Show(this);
            highlighter.SizeFromWindow(window);
            Application.DoEvents();
            Thread.Sleep(100);
            highlighter.Hide();
            Application.DoEvents();
        }
        private void HighlightB_Click(object sender, EventArgs e)
        {
            int handle;
            if (int.TryParse(HandleTB.Text, System.Globalization.NumberStyles.HexNumber, null, out handle))
            {
                for (int i = 0; i < 5; i++)
                {
                    Highlight(handle);
                    Thread.Sleep(100);
                }
            }
        }
        private void WindowTextTB_TextChanged(object sender, EventArgs e)
        {
            if (!changing)
                if (curWin.IsHung)
                    curWin.Text = WindowTextTB.Text;
                else
                    curWin.TextUnsafe = WindowTextTB.Text;
        }
        private void MinimizeB_Click(object sender, EventArgs e)
        {
            curWin.Minimize();
        }
        private void UpdateWinStateButtons()
        {
            if(!curWin.Exists)
            {
                MinimizeB.Enabled = false;
                MaximizeB.Enabled = false;
                RestoreB.Enabled = false;
                CloseB.Enabled = false;
                VisibleB.Enabled = false;
                EnabledB.Enabled = false;
                return;
            }
            switch (curWin.WindowState)
            {
            	case  FormWindowState.Maximized:
                    MinimizeB.Enabled = true;
                    MaximizeB.Enabled = false;
                    RestoreB.Enabled = true;
            		break;
                case FormWindowState.Minimized:
                    MinimizeB.Enabled = false;
                    MaximizeB.Enabled = true;
                    RestoreB.Enabled = true;
            		break;
                case FormWindowState.Normal:
                    MinimizeB.Enabled = true;
                    MaximizeB.Enabled = true;
                    RestoreB.Enabled = false;
            		break;
            }
            CloseB.Enabled = true;
            VisibleB.Enabled = true;
            EnabledB.Enabled = true;
            if (curWin.Visible)
                VisibleB.Image = Properties.Resources.VisibleTrue24;
            else
                VisibleB.Image = Properties.Resources.VisibleFalse24;
            if (curWin.Enabled)
                EnabledB.Image = Properties.Resources.Enabled24;
            else
                EnabledB.Image = Properties.Resources.Disabled24;
        }
        private void RestoreB_Click(object sender, EventArgs e)
        {
            curWin.WindowState = FormWindowState.Normal;
        }
        private void MaximizeB_Click(object sender, EventArgs e)
        {
            curWin.WindowState = FormWindowState.Maximized;
        }
        private void CloseB_Click(object sender, EventArgs e)
        {
            curWin.Close();
        }
        private void VisibleB_Click(object sender, EventArgs e)
        {
            curWin.Visible = !curWin.Visible;
        }
        private void EnabledB_Click(object sender, EventArgs e)
        {
            curWin.Enabled = !curWin.Enabled;
        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            catcher1_onWindowChanged(curWin);
        }
        private void EditStylesB_Click(object sender, EventArgs e)
        {
            StylesEditForm f = new StylesEditForm(curWin, ((Button)sender).Name.Contains("Ex"));
            f.Show(this);
        }
        private void gcCollect_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void FindB_Click(object sender, EventArgs e)
        {
            FindWindowForm findForm = new FindWindowForm();
            findForm.ShowDialog(this);
            if (findForm.DialogResult == DialogResult.OK && findForm.activeWindow != null)
                catcher1_onWindowChanged(findForm.activeWindow);
        }
    }
}