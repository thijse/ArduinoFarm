namespace IuSpy
{
    partial class WinDetails
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
            this.HandleTB = new System.Windows.Forms.TextBox();
            this.ParentTB = new System.Windows.Forms.TextBox();
            this.OwnerTB = new System.Windows.Forms.TextBox();
            this.ChildTB = new System.Windows.Forms.TextBox();
            this.NextTB = new System.Windows.Forms.TextBox();
            this.PreviousTB = new System.Windows.Forms.TextBox();
            this.TextTB = new System.Windows.Forms.TextBox();
            this.ClassTB = new System.Windows.Forms.TextBox();
            this.highlightB = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.winIconSmall = new System.Windows.Forms.PictureBox();
            this.winIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winIconSmall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.winIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // HandleTB
            // 
            this.HandleTB.BackColor = System.Drawing.SystemColors.Window;
            this.HandleTB.Location = new System.Drawing.Point(64, 5);
            this.HandleTB.Name = "HandleTB";
            this.HandleTB.Size = new System.Drawing.Size(72, 20);
            this.HandleTB.TabIndex = 0;
            this.HandleTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // ParentTB
            // 
            this.ParentTB.BackColor = System.Drawing.SystemColors.Window;
            this.ParentTB.Location = new System.Drawing.Point(23, 85);
            this.ParentTB.Name = "ParentTB";
            this.ParentTB.Size = new System.Drawing.Size(57, 20);
            this.ParentTB.TabIndex = 4;
            this.ParentTB.Text = "Parent";
            this.ParentTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // OwnerTB
            // 
            this.OwnerTB.BackColor = System.Drawing.SystemColors.Window;
            this.OwnerTB.Location = new System.Drawing.Point(23, 131);
            this.OwnerTB.Name = "OwnerTB";
            this.OwnerTB.Size = new System.Drawing.Size(57, 20);
            this.OwnerTB.TabIndex = 6;
            this.OwnerTB.Text = "Owner";
            this.OwnerTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // ChildTB
            // 
            this.ChildTB.BackColor = System.Drawing.SystemColors.Window;
            this.ChildTB.Location = new System.Drawing.Point(23, 108);
            this.ChildTB.Name = "ChildTB";
            this.ChildTB.Size = new System.Drawing.Size(57, 20);
            this.ChildTB.TabIndex = 5;
            this.ChildTB.Text = "Child";
            this.ChildTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // NextTB
            // 
            this.NextTB.BackColor = System.Drawing.SystemColors.Window;
            this.NextTB.Location = new System.Drawing.Point(105, 85);
            this.NextTB.Name = "NextTB";
            this.NextTB.Size = new System.Drawing.Size(57, 20);
            this.NextTB.TabIndex = 7;
            this.NextTB.Text = "Next";
            this.NextTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // PreviousTB
            // 
            this.PreviousTB.BackColor = System.Drawing.SystemColors.Window;
            this.PreviousTB.Location = new System.Drawing.Point(105, 108);
            this.PreviousTB.Name = "PreviousTB";
            this.PreviousTB.Size = new System.Drawing.Size(57, 20);
            this.PreviousTB.TabIndex = 8;
            this.PreviousTB.Text = "Previous";
            this.PreviousTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // TextTB
            // 
            this.TextTB.BackColor = System.Drawing.SystemColors.Window;
            this.TextTB.Location = new System.Drawing.Point(38, 33);
            this.TextTB.Name = "TextTB";
            this.TextTB.Size = new System.Drawing.Size(124, 20);
            this.TextTB.TabIndex = 2;
            this.TextTB.Text = "Text";
            this.TextTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // ClassTB
            // 
            this.ClassTB.BackColor = System.Drawing.SystemColors.Window;
            this.ClassTB.Location = new System.Drawing.Point(38, 59);
            this.ClassTB.Name = "ClassTB";
            this.ClassTB.Size = new System.Drawing.Size(124, 20);
            this.ClassTB.TabIndex = 3;
            this.ClassTB.Text = "Class";
            this.ClassTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Inactive_KeyDown);
            // 
            // highlightB
            // 
            this.highlightB.Image = global::IuSpy.Properties.Resources.Highlight16;
            this.highlightB.Location = new System.Drawing.Point(138, 3);
            this.highlightB.Name = "highlightB";
            this.highlightB.Size = new System.Drawing.Size(24, 24);
            this.highlightB.TabIndex = 8;
            this.highlightB.UseVisualStyleBackColor = true;
            this.highlightB.Click += new System.EventHandler(this.highlightB_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(70)))), ((int)(((byte)(213)))));
            this.label5.Location = new System.Drawing.Point(1, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Text:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(70)))), ((int)(((byte)(213)))));
            this.label4.Location = new System.Drawing.Point(0, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "Class:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::IuSpy.Properties.Resources.parent16;
            this.pictureBox1.Location = new System.Drawing.Point(5, 87);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.TabIndex = 44;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::IuSpy.Properties.Resources.Owner_16;
            this.pictureBox2.Location = new System.Drawing.Point(5, 133);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.TabIndex = 45;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::IuSpy.Properties.Resources.Previous16;
            this.pictureBox3.Location = new System.Drawing.Point(87, 110);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.TabIndex = 46;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::IuSpy.Properties.Resources.Next16;
            this.pictureBox4.Location = new System.Drawing.Point(87, 87);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(16, 16);
            this.pictureBox4.TabIndex = 47;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::IuSpy.Properties.Resources.Child16;
            this.pictureBox5.Location = new System.Drawing.Point(5, 110);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(16, 16);
            this.pictureBox5.TabIndex = 48;
            this.pictureBox5.TabStop = false;
            // 
            // winIconSmall
            // 
            this.winIconSmall.Location = new System.Drawing.Point(42, 8);
            this.winIconSmall.Name = "winIconSmall";
            this.winIconSmall.Size = new System.Drawing.Size(16, 16);
            this.winIconSmall.TabIndex = 49;
            this.winIconSmall.TabStop = false;
            // 
            // winIcon
            // 
            this.winIcon.Location = new System.Drawing.Point(4, 0);
            this.winIcon.Name = "winIcon";
            this.winIcon.Size = new System.Drawing.Size(32, 32);
            this.winIcon.TabIndex = 50;
            this.winIcon.TabStop = false;
            // 
            // WinDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(167, 155);
            this.ControlBox = false;
            this.Controls.Add(this.winIcon);
            this.Controls.Add(this.winIconSmall);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ClassTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.highlightB);
            this.Controls.Add(this.TextTB);
            this.Controls.Add(this.PreviousTB);
            this.Controls.Add(this.NextTB);
            this.Controls.Add(this.ChildTB);
            this.Controls.Add(this.OwnerTB);
            this.Controls.Add(this.ParentTB);
            this.Controls.Add(this.HandleTB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "WinDetails";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.VisibleChanged += new System.EventHandler(this.WinDetails_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WinDetails_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winIconSmall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.winIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HandleTB;
        private System.Windows.Forms.TextBox ParentTB;
        private System.Windows.Forms.TextBox OwnerTB;
        private System.Windows.Forms.TextBox ChildTB;
        private System.Windows.Forms.TextBox NextTB;
        private System.Windows.Forms.TextBox PreviousTB;
        private System.Windows.Forms.TextBox TextTB;
        private System.Windows.Forms.TextBox ClassTB;
        private System.Windows.Forms.Button highlightB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox winIconSmall;
        private System.Windows.Forms.PictureBox winIcon;



    }
}