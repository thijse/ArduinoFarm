using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ArduinoStudio
{
    public partial class PropertyWindow : ToolWindow
    {
        public PropertyWindow()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 0;
            propertyGrid.SelectedObject = propertyGrid;
        }
    }
}