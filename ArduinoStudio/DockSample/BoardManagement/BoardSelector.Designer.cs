namespace ArduinoStudio.BoardManagement
{
    partial class BoardSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.v_package = new System.Windows.Forms.ComboBox();
            this.v_architecture = new System.Windows.Forms.ComboBox();
            this.v_board = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.board_options = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.v_comport = new System.Windows.Forms.ComboBox();
            this.v_terminal_reset = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.d_baud_rate = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.do_compile_and_upload = new System.Windows.Forms.Button();
            this.do_show_terminal = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Package";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Architecture";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Board";
            // 
            // v_package
            // 
            this.v_package.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_package.FormattingEnabled = true;
            this.v_package.Location = new System.Drawing.Point(147, 33);
            this.v_package.Name = "v_package";
            this.v_package.Size = new System.Drawing.Size(269, 28);
            this.v_package.TabIndex = 4;
            this.v_package.SelectedIndexChanged += new System.EventHandler(this.v_package_SelectedIndexChanged);
            // 
            // v_architecture
            // 
            this.v_architecture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_architecture.FormattingEnabled = true;
            this.v_architecture.Location = new System.Drawing.Point(147, 67);
            this.v_architecture.Name = "v_architecture";
            this.v_architecture.Size = new System.Drawing.Size(269, 28);
            this.v_architecture.TabIndex = 4;
            this.v_architecture.SelectedIndexChanged += new System.EventHandler(this.v_architecture_SelectedIndexChanged);
            // 
            // v_board
            // 
            this.v_board.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_board.FormattingEnabled = true;
            this.v_board.Location = new System.Drawing.Point(147, 101);
            this.v_board.Name = "v_board";
            this.v_board.Size = new System.Drawing.Size(269, 28);
            this.v_board.TabIndex = 4;
            this.v_board.SelectedIndexChanged += new System.EventHandler(this.v_board_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.v_board);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.v_architecture);
            this.groupBox1.Controls.Add(this.v_package);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(463, 154);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Board selection";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.board_options);
            this.groupBox3.Location = new System.Drawing.Point(12, 172);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(463, 177);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Board options";
            // 
            // board_options
            // 
            this.board_options.AutoScroll = true;
            this.board_options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.board_options.Location = new System.Drawing.Point(3, 22);
            this.board_options.Name = "board_options";
            this.board_options.Size = new System.Drawing.Size(457, 152);
            this.board_options.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "Baud rate";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(109, 20);
            this.label9.TabIndex = 1;
            this.label9.Text = "Terminal reset";
            // 
            // v_comport
            // 
            this.v_comport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_comport.FormattingEnabled = true;
            this.v_comport.Location = new System.Drawing.Point(148, 33);
            this.v_comport.Name = "v_comport";
            this.v_comport.Size = new System.Drawing.Size(241, 28);
            this.v_comport.TabIndex = 4;
            // 
            // v_terminal_reset
            // 
            this.v_terminal_reset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.v_terminal_reset.FormattingEnabled = true;
            this.v_terminal_reset.Items.AddRange(new object[] {
            "DTR, RTS high"});
            this.v_terminal_reset.Location = new System.Drawing.Point(148, 101);
            this.v_terminal_reset.Name = "v_terminal_reset";
            this.v_terminal_reset.Size = new System.Drawing.Size(241, 28);
            this.v_terminal_reset.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "COM port";
            // 
            // d_baud_rate
            // 
            this.d_baud_rate.AutoSize = true;
            this.d_baud_rate.Location = new System.Drawing.Point(148, 69);
            this.d_baud_rate.Name = "d_baud_rate";
            this.d_baud_rate.Size = new System.Drawing.Size(18, 20);
            this.d_baud_rate.TabIndex = 5;
            this.d_baud_rate.Text = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.d_baud_rate);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.v_terminal_reset);
            this.groupBox2.Controls.Add(this.v_comport);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(481, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(415, 154);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Communication";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.do_compile_and_upload);
            this.groupBox4.Controls.Add(this.do_show_terminal);
            this.groupBox4.Location = new System.Drawing.Point(481, 172);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(415, 177);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(169, 94);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(81, 94);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 29);
            this.button2.TabIndex = 0;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // do_compile_and_upload
            // 
            this.do_compile_and_upload.Location = new System.Drawing.Point(100, 25);
            this.do_compile_and_upload.Name = "do_compile_and_upload";
            this.do_compile_and_upload.Size = new System.Drawing.Size(154, 29);
            this.do_compile_and_upload.TabIndex = 0;
            this.do_compile_and_upload.Text = "Compile && upload";
            this.do_compile_and_upload.UseVisualStyleBackColor = true;
            // 
            // do_show_terminal
            // 
            this.do_show_terminal.Location = new System.Drawing.Point(9, 25);
            this.do_show_terminal.Name = "do_show_terminal";
            this.do_show_terminal.Size = new System.Drawing.Size(85, 29);
            this.do_show_terminal.TabIndex = 0;
            this.do_show_terminal.Text = "Terminal";
            this.do_show_terminal.UseVisualStyleBackColor = true;
            // 
            // BoardSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 373);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "BoardSelector";
            this.Text = "Configure board";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox v_package;
        private System.Windows.Forms.ComboBox v_architecture;
        private System.Windows.Forms.ComboBox v_board;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox v_comport;
        private System.Windows.Forms.ComboBox v_terminal_reset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label d_baud_rate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button do_compile_and_upload;
        private System.Windows.Forms.Button do_show_terminal;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FlowLayoutPanel board_options;
    }
}