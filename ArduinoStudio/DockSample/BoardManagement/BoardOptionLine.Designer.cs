namespace ArduinoStudio.BoardManagement
{
    partial class BoardOptionLine
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.opt_label = new System.Windows.Forms.Label();
            this.opt_options = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // opt_label
            // 
            this.opt_label.AutoSize = true;
            this.opt_label.Location = new System.Drawing.Point(9, 3);
            this.opt_label.Name = "opt_label";
            this.opt_label.Size = new System.Drawing.Size(49, 20);
            this.opt_label.TabIndex = 5;
            this.opt_label.Text = "OPT1";
            // 
            // opt_options
            // 
            this.opt_options.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.opt_options.FormattingEnabled = true;
            this.opt_options.Location = new System.Drawing.Point(142, 0);
            this.opt_options.Name = "opt_options";
            this.opt_options.Size = new System.Drawing.Size(269, 28);
            this.opt_options.TabIndex = 6;
            // 
            // BoardOptionLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.opt_label);
            this.Controls.Add(this.opt_options);
            this.Name = "BoardOptionLine";
            this.Size = new System.Drawing.Size(414, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label opt_label;
        private System.Windows.Forms.ComboBox opt_options;
    }
}
