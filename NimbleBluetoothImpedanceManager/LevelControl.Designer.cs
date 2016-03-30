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
            this.label3 = new System.Windows.Forms.Label();
            this.btnEnterValue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStimOn
            // 
            this.btnStimOn.Location = new System.Drawing.Point(48, 12);
            this.btnStimOn.Name = "btnStimOn";
            this.btnStimOn.Size = new System.Drawing.Size(75, 23);
            this.btnStimOn.TabIndex = 0;
            this.btnStimOn.Text = "Stim On";
            this.btnStimOn.UseVisualStyleBackColor = true;
            this.btnStimOn.Click += new System.EventHandler(this.btnStimOn_Click);
            // 
            // btnStimOff
            // 
            this.btnStimOff.Location = new System.Drawing.Point(276, 12);
            this.btnStimOff.Name = "btnStimOff";
            this.btnStimOff.Size = new System.Drawing.Size(75, 23);
            this.btnStimOff.TabIndex = 1;
            this.btnStimOff.Text = "Stim Off";
            this.btnStimOff.UseVisualStyleBackColor = true;
            this.btnStimOff.Click += new System.EventHandler(this.btnStimOff_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(291, 69);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(46, 47);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "/\\";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(291, 166);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(46, 47);
            this.btnDown.TabIndex = 3;
            this.btnDown.Text = "\\/";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(180, 103);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(25, 13);
            this.lblTarget.TabIndex = 4;
            this.lblTarget.Text = "100";
            // 
            // lblCurrentLevel
            // 
            this.lblCurrentLevel.AutoSize = true;
            this.lblCurrentLevel.Location = new System.Drawing.Point(180, 173);
            this.lblCurrentLevel.Name = "lblCurrentLevel";
            this.lblCurrentLevel.Size = new System.Drawing.Size(19, 13);
            this.lblCurrentLevel.TabIndex = 5;
            this.lblCurrentLevel.Text = "20";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Target Level";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Current Level";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(184, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "On";
            // 
            // btnEnterValue
            // 
            this.btnEnterValue.Location = new System.Drawing.Point(291, 123);
            this.btnEnterValue.Name = "btnEnterValue";
            this.btnEnterValue.Size = new System.Drawing.Size(46, 37);
            this.btnEnterValue.TabIndex = 9;
            this.btnEnterValue.Text = "Enter Value";
            this.btnEnterValue.UseVisualStyleBackColor = true;
            this.btnEnterValue.Click += new System.EventHandler(this.btnEnterValue_Click);
            // 
            // LevelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 271);
            this.Controls.Add(this.btnEnterValue);
            this.Controls.Add(this.label3);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnEnterValue;
    }
}