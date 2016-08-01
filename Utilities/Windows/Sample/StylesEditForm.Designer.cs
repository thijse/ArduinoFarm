namespace IuSpy
{
    partial class StylesEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StylesEditForm));
            this.Primitive = new System.Windows.Forms.GroupBox();
            this.Derived = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.OkB = new System.Windows.Forms.Button();
            this.CancelB = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Primitive
            // 
            this.Primitive.Location = new System.Drawing.Point(3, 3);
            this.Primitive.Name = "Primitive";
            this.Primitive.Size = new System.Drawing.Size(59, 52);
            this.Primitive.TabIndex = 1;
            this.Primitive.TabStop = false;
            this.Primitive.Text = "Primitive styles";
            // 
            // Derived
            // 
            this.Derived.Location = new System.Drawing.Point(68, 12);
            this.Derived.Name = "Derived";
            this.Derived.Size = new System.Drawing.Size(85, 43);
            this.Derived.TabIndex = 2;
            this.Derived.TabStop = false;
            this.Derived.Text = "Derived styles";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 300;
            // 
            // OkB
            // 
            this.OkB.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.OkB.Location = new System.Drawing.Point(6, 62);
            this.OkB.Name = "OkB";
            this.OkB.Size = new System.Drawing.Size(75, 23);
            this.OkB.TabIndex = 3;
            this.OkB.Text = "Ok";
            this.OkB.UseVisualStyleBackColor = true;
            this.OkB.Click += new System.EventHandler(this.OkB_Click);
            // 
            // CancelB
            // 
            this.CancelB.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CancelB.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelB.Location = new System.Drawing.Point(87, 62);
            this.CancelB.Name = "CancelB";
            this.CancelB.Size = new System.Drawing.Size(75, 23);
            this.CancelB.TabIndex = 4;
            this.CancelB.Text = "Cancel";
            this.CancelB.UseVisualStyleBackColor = true;
            this.CancelB.Click += new System.EventHandler(this.CancelB_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 200;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // StylesEditForm
            // 
            this.AcceptButton = this.OkB;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelB;
            this.ClientSize = new System.Drawing.Size(204, 90);
            this.Controls.Add(this.CancelB);
            this.Controls.Add(this.OkB);
            this.Controls.Add(this.Derived);
            this.Controls.Add(this.Primitive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "StylesEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StylesEdit";
            this.VisibleChanged += new System.EventHandler(this.StylesEditForm_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Primitive;
        private System.Windows.Forms.GroupBox Derived;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button OkB;
        private System.Windows.Forms.Button CancelB;
        private System.Windows.Forms.Timer updateTimer;
    }
}