namespace IuSpy
{
    partial class FindWindowForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindWindowForm));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.handleTB = new System.Windows.Forms.TextBox();
            this.nameTB = new System.Windows.Forms.TextBox();
            this.classTB = new System.Windows.Forms.TextBox();
            this.updateListBoxTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.EnableB = new System.Windows.Forms.Button();
            this.VisibleB = new System.Windows.Forms.Button();
            this.CloseB = new System.Windows.Forms.Button();
            this.MaximizeB = new System.Windows.Forms.Button();
            this.RestoreB = new System.Windows.Forms.Button();
            this.MinimizeB = new System.Windows.Forms.Button();
            this.updateIconsTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(2, 80);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(305, 134);
            this.listBox1.TabIndex = 1;
            this.listBox1.TabStop = false;
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // handleTB
            // 
            this.handleTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.handleTB.Location = new System.Drawing.Point(53, 5);
            this.handleTB.Name = "handleTB";
            this.handleTB.Size = new System.Drawing.Size(109, 20);
            this.handleTB.TabIndex = 1;
            // 
            // nameTB
            // 
            this.nameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTB.Location = new System.Drawing.Point(42, 30);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(265, 20);
            this.nameTB.TabIndex = 2;
            // 
            // classTB
            // 
            this.classTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.classTB.Location = new System.Drawing.Point(42, 56);
            this.classTB.Name = "classTB";
            this.classTB.Size = new System.Drawing.Size(265, 20);
            this.classTB.TabIndex = 3;
            // 
            // updateListBoxTimer
            // 
            this.updateListBoxTimer.Enabled = true;
            this.updateListBoxTimer.Interval = 500;
            this.updateListBoxTimer.Tick += new System.EventHandler(this.updateListBoxTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Handle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Text";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Class";
            // 
            // EnableB
            // 
            this.EnableB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EnableB.Image = global::IuSpy.Properties.Resources.Enabled16;
            this.EnableB.Location = new System.Drawing.Point(282, 2);
            this.EnableB.Name = "EnableB";
            this.EnableB.Size = new System.Drawing.Size(24, 24);
            this.EnableB.TabIndex = 9;
            this.toolTip1.SetToolTip(this.EnableB, "Enabled.");
            this.EnableB.UseVisualStyleBackColor = true;
            this.EnableB.Click += new System.EventHandler(this.EnableB_Click);
            // 
            // VisibleB
            // 
            this.VisibleB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.VisibleB.Image = global::IuSpy.Properties.Resources.Visible16;
            this.VisibleB.Location = new System.Drawing.Point(259, 2);
            this.VisibleB.Name = "VisibleB";
            this.VisibleB.Size = new System.Drawing.Size(24, 24);
            this.VisibleB.TabIndex = 8;
            this.toolTip1.SetToolTip(this.VisibleB, "Visible.");
            this.VisibleB.UseVisualStyleBackColor = true;
            this.VisibleB.Click += new System.EventHandler(this.VisibleB_Click);
            // 
            // CloseB
            // 
            this.CloseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseB.Image = global::IuSpy.Properties.Resources.Close16;
            this.CloseB.Location = new System.Drawing.Point(236, 2);
            this.CloseB.Name = "CloseB";
            this.CloseB.Size = new System.Drawing.Size(24, 24);
            this.CloseB.TabIndex = 7;
            this.toolTip1.SetToolTip(this.CloseB, "Close.");
            this.CloseB.UseVisualStyleBackColor = true;
            this.CloseB.Click += new System.EventHandler(this.CloseB_Click);
            // 
            // MaximizeB
            // 
            this.MaximizeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MaximizeB.Image = global::IuSpy.Properties.Resources.Maximize16;
            this.MaximizeB.Location = new System.Drawing.Point(213, 2);
            this.MaximizeB.Name = "MaximizeB";
            this.MaximizeB.Size = new System.Drawing.Size(24, 24);
            this.MaximizeB.TabIndex = 6;
            this.toolTip1.SetToolTip(this.MaximizeB, "Maximize.");
            this.MaximizeB.UseVisualStyleBackColor = true;
            this.MaximizeB.Click += new System.EventHandler(this.MaximizeB_Click);
            // 
            // RestoreB
            // 
            this.RestoreB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RestoreB.Image = global::IuSpy.Properties.Resources.Restore16;
            this.RestoreB.Location = new System.Drawing.Point(190, 2);
            this.RestoreB.Name = "RestoreB";
            this.RestoreB.Size = new System.Drawing.Size(24, 24);
            this.RestoreB.TabIndex = 5;
            this.toolTip1.SetToolTip(this.RestoreB, "Restore.");
            this.RestoreB.UseVisualStyleBackColor = true;
            this.RestoreB.Click += new System.EventHandler(this.RestoreB_Click);
            // 
            // MinimizeB
            // 
            this.MinimizeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimizeB.BackColor = System.Drawing.SystemColors.Control;
            this.MinimizeB.Image = global::IuSpy.Properties.Resources.Minimize16;
            this.MinimizeB.Location = new System.Drawing.Point(167, 2);
            this.MinimizeB.Name = "MinimizeB";
            this.MinimizeB.Size = new System.Drawing.Size(24, 24);
            this.MinimizeB.TabIndex = 4;
            this.toolTip1.SetToolTip(this.MinimizeB, "Minimize.");
            this.MinimizeB.UseVisualStyleBackColor = false;
            this.MinimizeB.Click += new System.EventHandler(this.MinimizeB_Click);
            // 
            // updateIconsTimer
            // 
            this.updateIconsTimer.Enabled = true;
            this.updateIconsTimer.Interval = 500;
            this.updateIconsTimer.Tick += new System.EventHandler(this.updateIconsTimer_Tick);
            // 
            // FindWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 217);
            this.Controls.Add(this.EnableB);
            this.Controls.Add(this.VisibleB);
            this.Controls.Add(this.CloseB);
            this.Controls.Add(this.MaximizeB);
            this.Controls.Add(this.RestoreB);
            this.Controls.Add(this.MinimizeB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameTB);
            this.Controls.Add(this.classTB);
            this.Controls.Add(this.handleTB);
            this.Controls.Add(this.listBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(267, 159);
            this.Name = "FindWindowForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FindWindow";
            this.Shown += new System.EventHandler(this.FindWindowForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindWindowForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox handleTB;
        private System.Windows.Forms.TextBox nameTB;
        private System.Windows.Forms.TextBox classTB;
        private System.Windows.Forms.Timer updateListBoxTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button EnableB;
        private System.Windows.Forms.Button VisibleB;
        private System.Windows.Forms.Button CloseB;
        private System.Windows.Forms.Button MaximizeB;
        private System.Windows.Forms.Button RestoreB;
        private System.Windows.Forms.Button MinimizeB;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer updateIconsTimer;
    }
}