using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ArduinoWrapper;

namespace ArduinoStudio.BoardManagement
{
    public partial class BoardSelector : Form
    {
        private BoardPackages boards;
        private Label[] option_labels;
        private ComboBox[] option_options;

    public BoardSelector(BoardPackages _boards)
        {
            boards = _boards;
            // Create form
            InitializeComponent();
            // 'Shortcuts'
            option_labels = new Label[] { opt1_label, opt2_label, opt3_label };
            option_options = new ComboBox[] { opt1_options, opt2_options, opt3_options };
            // Fill package combo
            v_package.Items.AddRange(boards.ToArray());
            // Select first package (when not empty). Triggers 'package changed'
            if (v_package.Items.Count>0)
            {
                v_package.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Different package selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_package_SelectedIndexChanged(object sender, EventArgs e)
        {
            BoardPackage bp = v_package.SelectedItem as ArduinoWrapper.BoardPackage;
            v_architecture.SuspendLayout();
            v_architecture.Items.Clear();
            v_architecture.Items.AddRange(bp.BoardArchitectures.ToArray());
            if (v_architecture.Items.Count > 0)
            {
                v_architecture.SelectedIndex = 0;
            }
            v_architecture.ResumeLayout();
        }

        /// <summary>
        /// Different architecture selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_architecture_SelectedIndexChanged(object sender, EventArgs e)
        {
            BoardArchitecture ba = v_architecture.SelectedItem as BoardArchitecture;
            v_board.SuspendLayout();
            v_board.Items.Clear();
            v_board.Items.AddRange(ba.BoardDescriptions.ToArray());
            if (v_board.Items.Count > 0)
            {
                v_board.SelectedIndex = 0;
            }
            v_board.ResumeLayout();
        }
    
        /// <summary>
        /// Different board selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_board_SelectedIndexChanged(object sender, EventArgs e)
        {            
            // Hide all current options
            foreach (var label in option_labels) label.Hide();
            foreach (var options in option_options) options.Hide();
            // Get current board description
            BoardDescription bd = v_board.SelectedItem as BoardDescription;
            // Cycle through all (max 3...) options
            for (int i = 0; i < 3 & i < bd.BoardOptions.Count; i++)
            {
                BoardOption bo = bd.BoardOptions[i];
                // Set label
                option_labels[i].Text = bo.Description;
                option_labels[i].Show();
                // Set option values
                option_options[i].SuspendLayout();
                option_options[i].Items.Clear();
                option_options[i].Items.AddRange(bo.ToArray());
                if (option_options[i].Items.Count > 0)
                {
                    option_options[i].SelectedIndex = 0;
                }
                option_options[i].ResumeLayout();
                option_options[i].Show();                
            }
        }
    }
}
