using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoStudio.BoardManagement
{
    public partial class BoardOptionLine : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options"></param>
        public BoardOptionLine(string label, object[] options)
        {
            InitializeComponent();
            opt_label.Text = label;
            opt_options.Items.AddRange(options);
            if (opt_options.Items.Count > 0)
            {
                opt_options.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Return selected item
        /// </summary>
        public object SelectedItem
        {
            get { return opt_options.SelectedItem; }
        }
    }
}
