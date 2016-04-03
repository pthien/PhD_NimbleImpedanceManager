namespace NimbleBluetoothImpedanceManager
{
    partial class LevelControl
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
            this.btnStimOn = new System.Windows.Forms.Button();
            this.btnStimOff = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblCurrentLevel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEnterValue = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusStimOn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusRampLevel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStimOn
            // 
            this.btnStimOn.Location = new System.Drawing.Point(12, 12);
            this.btnStimOn.Name = "btnStimOn";
            this.btnStimOn.Size = new System.Drawing.Size(107, 23);
            this.btnStimOn.TabIndex = 0;
            this.btnStimOn.Text = "Stimulation On";
            this.btnStimOn.UseVisualStyleBackColor = true;
            this.btnStimOn.Click += new System.EventHandler(this.btnStimOn_Click);
            // 
            // btnStimOff
            // 
            this.btnStimOff.Location = new System.Drawing.Point(134, 12);
            this.btnStimOff.Name = "btnStimOff";
            this.btnStimOff.Size = new System.Drawing.Size(105, 23);
            this.btnStimOff.TabIndex = 1;
            this.btnStimOff.Text = "Stimulation Off";
            this.btnStimOff.UseVisualStyleBackColor = true;
            this.btnStimOff.Click += new System.EventHandler(this.btnStimOff_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(327, 57);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(33, 25);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "/\\";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(327, 88);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(33, 25);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "\\/";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTarget.Location = new System.Drawing.Point(240, 70);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(48, 26);
            this.lblTarget.TabIndex = 4;
            this.lblTarget.Text = "100";
            // 
            // lblCurrentLevel
            // 
            this.lblCurrentLevel.AutoSize = true;
            this.lblCurrentLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentLevel.Location = new System.Drawing.Point(240, 128);
            this.lblCurrentLevel.Name = "lblCurrentLevel";
            this.lblCurrentLevel.Size = new System.Drawing.Size(36, 26);
            this.lblCurrentLevel.TabIndex = 5;
            this.lblCurrentLevel.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 26);
            this.label1.TabIndex = 6;
            this.label1.Text = "Target Ramp Level:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(213, 26);
            this.label2.TabIndex = 7;
            this.label2.Text = "Current Ramp Level:";
            // 
            // btnEnterValue
            // 
            this.btnEnterValue.Location = new System.Drawing.Point(366, 57);
            this.btnEnterValue.Name = "btnEnterValue";
            this.btnEnterValue.Size = new System.Drawing.Size(54, 56);
            this.btnEnterValue.TabIndex = 9;
            this.btnEnterValue.Text = "Specify Value";
            this.btnEnterValue.UseVisualStyleBackColor = true;
            this.btnEnterValue.Click += new System.EventHandler(this.btnEnterValue_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStimOn,
            this.toolStripStatusLabel1,
            this.statusRampLevel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 180);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(433, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusStimOn
            // 
            this.statusStimOn.Name = "statusStimOn";
            this.statusStimOn.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(418, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // statusRampLevel
            // 
            this.statusRampLevel.Name = "statusRampLevel";
            this.statusRampLevel.Size = new System.Drawing.Size(0, 17);
            // 
            // LevelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 202);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnEnterValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCurrentLevel);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnStimOff);
            this.Controls.Add(this.btnStimOn);
            this.Name = "LevelControl";
            this.Text = "LevelControl";
            this.Load += new System.EventHandler(this.LevelControl_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStimOn;
        private System.Windows.Forms.Button btnStimOff;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblCurrentLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnEnterValue;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusStimOn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusRampLevel;
    }
}