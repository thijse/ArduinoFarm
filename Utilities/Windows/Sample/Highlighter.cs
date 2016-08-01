using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IuSpy
{
    public partial class Highlighter : Form
    {
        public Highlighter()
        {
            InitializeComponent();
        }
        public void SizeFromWindow(Iu.Windows.CWindow win)
        {
            this.Bounds = win.DesktopBounds;
        }
        private void Highlighter_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = e.ClipRectangle;
            if (this.Left < 0 && this.Top < 0 && this.Height > Screen.PrimaryScreen.WorkingArea.Height && this.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                rect.Size = Screen.PrimaryScreen.WorkingArea.Size;
                rect.X = Screen.PrimaryScreen.WorkingArea.Left - this.Left;
                rect.Y = Screen.PrimaryScreen.WorkingArea.Top - this.Top;
            }

            rect.X++;
            rect.Y++;
            rect.Width -= 2;
            rect.Height -= 2;
            e.Graphics.DrawRectangle(new Pen(Color.Red, 2), rect);
        }
    }
}