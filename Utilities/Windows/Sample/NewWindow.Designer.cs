namespace IuSpy
{
    partial class NextWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NextWindow));
            this.HandleTB = new System.Windows.Forms.TextBox();
            this.InfoB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HandleTB
            // 
            this.HandleTB.BackColor = System.Drawing.SystemColors.Control;
            this.HandleTB.Location = new System.Drawing.Point(0, 0);
            this.HandleTB.Name = "HandleTB";
            this.HandleTB.Size = new System.Drawing.Size(73, 20);
            this.HandleTB.TabIndex = 0;
            this.HandleTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HandleTB_KeyPress);
            // 
            // InfoB
            // 
            this.InfoB.Image = ((System.Drawing.Image)(resources.GetObject("InfoB.Image")));
            this.InfoB.Location = new System.Drawing.Point(53, -3);
            this.InfoB.Name = "InfoB";
            this.InfoB.Size = new System.Drawing.Size(18, 22);
            this.InfoB.TabIndex = 1;
            this.InfoB.UseVisualStyleBackColor = true;
            this.InfoB.Click += new System.EventHandler(this.InfoB_Click);
            // 
            // NextWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.InfoB);
            this.Controls.Add(this.HandleTB);
            this.Name = "NextWindow";
            this.Size = new System.Drawing.Size(77, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox HandleTB;
        public System.Windows.Forms.Button InfoB;
    }
}
