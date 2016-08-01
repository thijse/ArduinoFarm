using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IuSpy
{
    public partial class WinDetails : Form
    {
        #region Service functions
        Control ownControl;
        public WinDetails(Control control, Iu.Windows.CWindow window)
        {
            InitializeComponent();
            this.LostFocus += new EventHandler(WinDetails_LostFocus);
            foreach (Control cur in this.Controls)
            {
                cur.LostFocus += new EventHandler(WinDetails_LostFocus);
                cur.Click += new EventHandler(control_Click);
            }
            ownControl = control;
            this.window = window;
            this.Resize += new EventHandler(WinDetails_Resize);
            WinDetails_Resize(this, null);  
        }
        void WinDetails_LostFocus(object sender, EventArgs e)
        {
            if (!this.ContainsFocus)
                this.Close();
        }
        private void WinDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape || e.KeyData == Keys.F10 || e.KeyData == Keys.Enter)
                this.Close();
        }
        private void control_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void WinDetails_Resize(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            Point startPoint = ownControl.PointToScreen(new Point(0, ownControl.Height + 1));
            if (!Screen.PrimaryScreen.Bounds.Contains(0, startPoint.Y + form.Height))
                startPoint = ownControl.PointToScreen(new Point(0, -form.Height));
            if (startPoint.X < 0)
                startPoint.X = 0;
            if (startPoint.X > Screen.PrimaryScreen.Bounds.Width-form.Width)
                startPoint.X = Screen.PrimaryScreen.Bounds.Width - form.Width;
            this.Location = startPoint;
        }
        #endregion Service functions

        public Iu.Windows.CWindow window;
        private void WinDetails_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;
            HandleTB.Text = window.Handle.ToString("X");
            if (window.IsHung)
                TextTB.Text = window.Text;
            else
                TextTB.Text = window.TextUnsafe;

            ClassTB.Text = window.ClassName;
            ParentTB.Text = window.Parent.Handle.ToString("X");
            ChildTB.Text = window.TopChild.Handle.ToString("X");
            OwnerTB.Text = window.Owner.Handle.ToString("X");
            PreviousTB.Text = window.GetNextWindow(Iu.Windows.GetWindow_Cmd.GW_HWNDPREV).Handle.ToString("X");
            NextTB.Text = window.GetNextWindow(Iu.Windows.GetWindow_Cmd.GW_HWNDNEXT).Handle.ToString("X");
            winIcon.Image = (window.ClassIcon != null) ? window.ClassIcon.ToBitmap() : Properties.Resources.NoImage32;
            winIconSmall.Image = (window.SmallClassIcon != null) ? window.SmallClassIcon.ToBitmap() : Properties.Resources.NoImage16;

        }

        private void highlightB_Click(object sender, EventArgs e)
        {
            MainForm form = (MainForm)Owner;
            for (int i = 0; i < 3 ; i++)
            {
                form.Highlight(window);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void Inactive_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}