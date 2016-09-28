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
        private bool updating;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_boards"></param>
        public BoardSelector(BoardPackages _boards, BoardConfig bc)
        {
            boards = _boards;
            // Create form
            InitializeComponent();
            // Fill package list (and the rest)
            FillPackageList(boards, bc);
        }

        /// <summary>
        /// Different package selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_package_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            BoardPackage bp = v_package.SelectedItem as ArduinoWrapper.BoardPackage;
            FillArchitectureList(bp, null);            
        }
       
        /// <summary>
        /// Different architecture selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_architecture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            BoardArchitecture ba = v_architecture.SelectedItem as BoardArchitecture;
            FillBoardList(ba, null);            
        }

            
        /// <summary>
        /// Different board selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v_board_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            // Get current board description
            BoardDescription bd = v_board.SelectedItem as BoardDescription;
            FillOptionList(bd, null);
        }


        /// <summary>
        /// Fill package list, select (configured) value
        /// </summary>
        /// <param name="boards"></param>
        /// <param name="bc"></param>
        private void FillPackageList(BoardPackages boards, BoardConfig bc)
        {
            // Fill package combo
            BoardPackage[] bps = boards.ToArray();
            v_package.Items.AddRange(bps);
            // Show current config            
            BoardPackage bp = (bc == null) ? null : boards.FirstOrDefault(p => p.Name == bc.CompilerConfiguration.Package);
            if (bp == null) bp = boards.FirstOrDefault();
            if (bp != null)
            {
                updating = true;
                v_package.SelectedItem = bp;
                updating = false;
                FillArchitectureList(bp, bc);
            }
        }


        /// <summary>
        /// Fill architecture list, select (configured) value
        /// </summary>
        /// <param name="bp"></param>
        private void FillArchitectureList(BoardPackage bp, BoardConfig bc)
        {
            v_architecture.SuspendLayout();
            v_architecture.Items.Clear();
            BoardArchitecture[] bas = bp.BoardArchitectures.ToArray();
            v_architecture.Items.AddRange(bas);
            // Show current config            
            BoardArchitecture ba = (bc == null) ? null : bas.FirstOrDefault(p => p.Name == bc.CompilerConfiguration.Architecture);
            if (ba == null) ba = bas.FirstOrDefault();
            if (ba != null)
            {
                updating = true;
                v_architecture.SelectedItem = ba;
                updating = false;
                FillBoardList(ba, bc);
            }
            // Resume layout
            v_architecture.ResumeLayout();
        }

        

        /// <summary>
        /// Fill board list, select (configured) value
        /// </summary>
        /// <param name="ba"></param>
        private void FillBoardList(BoardArchitecture ba, BoardConfig bc)
        {
            v_board.SuspendLayout();
            v_board.Items.Clear();
            BoardDescription[] bds = ba.BoardDescriptions.ToArray();
            v_board.Items.AddRange(bds);
            if (bc == null && v_board.Items.Count > 0)
            {
                v_board.SelectedIndex = 0;
            }
            v_board.ResumeLayout();
        }

        /// <summary>
        /// Fill option list
        /// </summary>
        /// <param name="bd"></param>
        private void FillOptionList(BoardDescription bd, BoardConfig bc)
        {
            // Clear current option list
            board_options.SuspendLayout();
            board_options.Controls.Clear();

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
