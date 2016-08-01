using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Iu.Windows;
using System.Runtime.InteropServices;

namespace IuSpy
{
    public partial class Catcher : UserControl
    {
        Cursor sightCursor;
        bool running = false;
        CWindow parent;
        CWindow curWin = 0;
        Highlighter highlighter;
        public Catcher()
        {
            InitializeComponent();
            sightCursor = new Cursor(GetType(), "Resources.Sight.cur");
            this.LostFocus += new EventHandler(Catcher_LostFocus);

            if (highlighter == null)
                highlighter = new Highlighter();
            ((CWindow)highlighter.Handle).Transparent = true;
        }
        private void Catcher_ParentChanged(object sender, EventArgs e)
        {
            highlighter.Show((IWin32Window)sender);
        }
        void Catcher_LostFocus(object sender, EventArgs e)
        {
            Catcher_MouseUp(this, null);
        }
        private void Catcher_MouseDown(object sender, MouseEventArgs e)
        {
            highlighter.BringToFront();
            this.BackgroundImage = Properties.Resources.P1;
            parent = this.Parent.Handle;
            running = true;
            this.Cursor = sightCursor;
            parent.Transparent = true;
            parent.Opacity = 0.7;
        }

        private void Catcher_MouseUp(object sender, MouseEventArgs e)
        {
            if (parent == null)
                return;
            curWin = 0;
            parent.Opacity = 1;
            parent.Transparent = false;
            this.Cursor = Cursors.Arrow;
            running = false;
            curWin.Refresh();
            this.BackgroundImage = Properties.Resources.P2;
            highlighter.Location = new Point(5000, 5000);
        }
        public delegate void WindowChangedEventHandler(CWindow window);
        public event WindowChangedEventHandler onWindowChanged;
        private void Catcher_MouseMove(object sender, MouseEventArgs e)
        {
            CWindow win = CWindow.FromPoint(((CWindow)this.Handle).PointToScreen(e.Location));
            if (win!=curWin &&  running)
            {
                curWin = win;
                if (onWindowChanged != null)
                    onWindowChanged(win);
                highlighter.SizeFromWindow(win);
            }
        }
    }
}
