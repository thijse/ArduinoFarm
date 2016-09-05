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

    public BoardSelector(BoardPackages _boards)
        {
            boards = _boards;
            // Create form
            InitializeComponent();
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
            // Clear current option list
            board_options.SuspendLayout();
            board_options.Controls.Clear();
            // Get current board description
            BoardDescription bd = v_board.SelectedItem as BoardDescription;
            // Cycle through all options
            for (int i = 0; i < bd.BoardOptions.Count; i++)
            {
                BoardOption bo = bd.BoardOptions[i];
                // Add control
                board_options.Controls.Add(new BoardOptionLine(bo.Description, bo.ToArray()));
            }
            // Resume layout
            board_options.ResumeLayout();
        }
    }
}
