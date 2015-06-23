namespace NimbleBluetoothImpedanceManager
{
    partial class Form1
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
            this.cmbCOMPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCycle = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblDongleStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRemoteDeviceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnStartScan = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cklFoundDevices = new System.Windows.Forms.CheckedListBox();
            this.lblDevicesRefreshTime = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.bntCheckCurrent = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCOMPorts
            // 
            this.cmbCOMPorts.FormattingEnabled = true;
            this.cmbCOMPorts.Location = new System.Drawing.Point(13, 13);
            this.cmbCOMPorts.Name = "cmbCOMPorts";
            this.cmbCOMPorts.Size = new System.Drawing.Size(121, 21);
            this.cmbCOMPorts.TabIndex = 0;
            this.cmbCOMPorts.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbCOMPorts_MouseClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(13, 41);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCycle
            // 
            this.btnCycle.Enabled = false;
            this.btnCycle.Location = new System.Drawing.Point(247, 40);
            this.btnCycle.Name = "btnCycle";
            this.btnCycle.Size = new System.Drawing.Size(75, 23);
            this.btnCycle.TabIndex = 3;
            this.btnCycle.Text = "Check All Devices";
            this.btnCycle.UseVisualStyleBackColor = true;
            this.btnCycle.Click += new System.EventHandler(this.btnCycle_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblDongleStatus,
            this.lblRemoteDeviceStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 330);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(888, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblDongleStatus
            // 
            this.lblDongleStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblDongleStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblDongleStatus.Name = "lblDongleStatus";
            this.lblDongleStatus.Size = new System.Drawing.Size(122, 19);
            this.lblDongleStatus.Text = "toolStripStatusLabel1";
            // 
            // lblRemoteDeviceStatus
            // 
            this.lblRemoteDeviceStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblRemoteDeviceStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblRemoteDeviceStatus.Name = "lblRemoteDeviceStatus";
            this.lblRemoteDeviceStatus.Size = new System.Drawing.Size(122, 19);
            this.lblRemoteDeviceStatus.Text = "toolStripStatusLabel1";
            // 
            // btnStartScan
            // 
            this.btnStartScan.Enabled = false;
            this.btnStartScan.Location = new System.Drawing.Point(12, 280);
            this.btnStartScan.Name = "btnStartScan";
            this.btnStartScan.Size = new System.Drawing.Size(159, 23);
            this.btnStartScan.TabIndex = 5;
            this.btnStartScan.Text = "Scan for Nimble Processors";
            this.btnStartScan.UseVisualStyleBackColor = true;
            this.btnStartScan.Click += new System.EventHandler(this.btnStartScan_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(403, 133);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Toggle Ping";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(403, 104);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "connect";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cklFoundDevices
            // 
            this.cklFoundDevices.FormattingEnabled = true;
            this.cklFoundDevices.Items.AddRange(new object[] {
            "F4B85EB48907"});
            this.cklFoundDevices.Location = new System.Drawing.Point(13, 93);
            this.cklFoundDevices.Name = "cklFoundDevices";
            this.cklFoundDevices.Size = new System.Drawing.Size(177, 139);
            this.cklFoundDevices.TabIndex = 8;
            // 
            // lblDevicesRefreshTime
            // 
            this.lblDevicesRefreshTime.AutoSize = true;
            this.lblDevicesRefreshTime.Location = new System.Drawing.Point(12, 235);
            this.lblDevicesRefreshTime.Name = "lblDevicesRefreshTime";
            this.lblDevicesRefreshTime.Size = new System.Drawing.Size(35, 13);
            this.lblDevicesRefreshTime.TabIndex = 9;
            this.lblDevicesRefreshTime.Text = "label1";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(575, 255);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(180, 23);
            this.btnScan.TabIndex = 10;
            this.btnScan.Text = "Scan for data files";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // bntCheckCurrent
            // 
            this.bntCheckCurrent.Location = new System.Drawing.Point(280, 235);
            this.bntCheckCurrent.Name = "bntCheckCurrent";
            this.bntCheckCurrent.Size = new System.Drawing.Size(142, 24);
            this.bntCheckCurrent.TabIndex = 11;
            this.bntCheckCurrent.Text = "Check current";
            this.bntCheckCurrent.UseVisualStyleBackColor = true;
            this.bntCheckCurrent.Click += new System.EventHandler(this.bntCheckCurrent_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 354);
            this.Controls.Add(this.bntCheckCurrent);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.lblDevicesRefreshTime);
            this.Controls.Add(this.cklFoundDevices);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStartScan);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCycle);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbCOMPorts);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDoubleClick);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCOMPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCycle;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblDongleStatus;
        private System.Windows.Forms.Button btnStartScan;
        private System.Windows.Forms.ToolStripStatusLabel lblRemoteDeviceStatus;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckedListBox cklFoundDevices;
        private System.Windows.Forms.Label lblDevicesRefreshTime;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button bntCheckCurrent;
    }
}

