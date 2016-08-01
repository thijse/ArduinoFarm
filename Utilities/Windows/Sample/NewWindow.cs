using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace IuSpy
{
    public partial class NextWindow : UserControl
    {
        public NextWindow()
        {
            InitializeComponent();
            this.InfoB.Parent = HandleTB;
        }

        private void HandleTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void InfoB_Click(object sender, EventArgs e)
        {
            int handle;
            if (int.TryParse(HandleTB.Text, System.Globalization.NumberStyles.HexNumber, null, out handle))
            {
                if (handle == 0)
                    return;
                WinDetails wD = new WinDetails(HandleTB, handle);
                wD.Show(this);
            }
        }
        public string TextTB
        {
        	get
        	{
                return HandleTB.Text;
        	}
        	set
        	{
                HandleTB.Text = value;
        	}
        }
    }
}
