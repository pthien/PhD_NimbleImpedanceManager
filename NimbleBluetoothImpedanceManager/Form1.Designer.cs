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
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAutoStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnScanForProcessors = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnConnectToNimble = new System.Windows.Forms.Button();
            this.cklFoundDevices = new System.Windows.Forms.CheckedListBox();
            this.lblDevicesRefreshTime = new System.Windows.Forms.Label();
            this.btnScanFiles = new System.Windows.Forms.Button();
            this.bntCheckCurrent = new System.Windows.Forms.Button();
            this.grpManualControl = new System.Windows.Forms.GroupBox();
            this.grpManualActions = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnAutoOperation = new System.Windows.Forms.Button();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSetWorkingDir = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.txtWorkingDir = new System.Windows.Forms.TextBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pannel_FoundProcessors = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.grpManualControl.SuspendLayout();
            this.grpManualActions.SuspendLayout();
            this.grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.pannel_FoundProcessors.SuspendLayout();
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
            this.btnConnect.Click += new System.EventHandler(this.btnConnectToDongle_Click);
            // 
            // btnCycle
            // 
            this.btnCycle.Enabled = false;
            this.btnCycle.Location = new System.Drawing.Point(6, 56);
            this.btnCycle.Name = "btnCycle";
            this.btnCycle.Size = new System.Drawing.Size(132, 23);
            this.btnCycle.TabIndex = 3;
            this.btnCycle.Text = "Check All Devices";
            this.btnCycle.UseVisualStyleBackColor = true;
            this.btnCycle.Click += new System.EventHandler(this.btnCycle_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblDongleStatus,
            this.lblRemoteDeviceStatus,
            this.toolStripStatusLabel1,
            this.lblAutoStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(964, 24);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblDongleStatus
            // 
            this.lblDongleStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblDongleStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblDongleStatus.Name = "lblDongleStatus";
            this.lblDongleStatus.Size = new System.Drawing.Size(105, 19);
            this.lblDongleStatus.Text = "connection status";
            // 
            // lblRemoteDeviceStatus
            // 
            this.lblRemoteDeviceStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.lblRemoteDeviceStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.lblRemoteDeviceStatus.Name = "lblRemoteDeviceStatus";
            this.lblRemoteDeviceStatus.Size = new System.Drawing.Size(79, 19);
            this.lblRemoteDeviceStatus.Text = "device status";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(703, 19);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // lblAutoStatus
            // 
            this.lblAutoStatus.Name = "lblAutoStatus";
            this.lblAutoStatus.Size = new System.Drawing.Size(62, 19);
            this.lblAutoStatus.Text = "autostatus";
            // 
            // btnScanForProcessors
            // 
            this.btnScanForProcessors.Enabled = false;
            this.btnScanForProcessors.Location = new System.Drawing.Point(6, 113);
            this.btnScanForProcessors.Name = "btnScanForProcessors";
            this.btnScanForProcessors.Size = new System.Drawing.Size(159, 23);
            this.btnScanForProcessors.TabIndex = 5;
            this.btnScanForProcessors.Text = "Scan for Nimble Processors";
            this.btnScanForProcessors.UseVisualStyleBackColor = true;
            this.btnScanForProcessors.Click += new System.EventHandler(this.btnStartScan_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnConnectToNimble
            // 
            this.btnConnectToNimble.Location = new System.Drawing.Point(6, 27);
            this.btnConnectToNimble.Name = "btnConnectToNimble";
            this.btnConnectToNimble.Size = new System.Drawing.Size(132, 23);
            this.btnConnectToNimble.TabIndex = 7;
            this.btnConnectToNimble.Text = "Connect to Nimble";
            this.btnConnectToNimble.UseVisualStyleBackColor = true;
            this.btnConnectToNimble.Click += new System.EventHandler(this.btnConnectToNimble_Click);
            // 
            // cklFoundDevices
            // 
            this.cklFoundDevices.FormattingEnabled = true;
            this.cklFoundDevices.Items.AddRange(new object[] {
            "F4B85EB48907"});
            this.cklFoundDevices.Location = new System.Drawing.Point(3, 21);
            this.cklFoundDevices.Name = "cklFoundDevices";
            this.cklFoundDevices.Size = new System.Drawing.Size(223, 109);
            this.cklFoundDevices.TabIndex = 8;
            // 
            // lblDevicesRefreshTime
            // 
            this.lblDevicesRefreshTime.AutoSize = true;
            this.lblDevicesRefreshTime.Location = new System.Drawing.Point(3, 133);
            this.lblDevicesRefreshTime.Name = "lblDevicesRefreshTime";
            this.lblDevicesRefreshTime.Size = new System.Drawing.Size(35, 13);
            this.lblDevicesRefreshTime.TabIndex = 9;
            this.lblDevicesRefreshTime.Text = "label1";
            // 
            // btnScanFiles
            // 
            this.btnScanFiles.Location = new System.Drawing.Point(6, 84);
            this.btnScanFiles.Name = "btnScanFiles";
            this.btnScanFiles.Size = new System.Drawing.Size(132, 23);
            this.btnScanFiles.TabIndex = 10;
            this.btnScanFiles.Text = "Scan for data files";
            this.btnScanFiles.UseVisualStyleBackColor = true;
            this.btnScanFiles.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // bntCheckCurrent
            // 
            this.bntCheckCurrent.Location = new System.Drawing.Point(6, 24);
            this.bntCheckCurrent.Name = "bntCheckCurrent";
            this.bntCheckCurrent.Size = new System.Drawing.Size(132, 24);
            this.bntCheckCurrent.TabIndex = 11;
            this.bntCheckCurrent.Text = "Check Impedances";
            this.bntCheckCurrent.UseVisualStyleBackColor = true;
            this.bntCheckCurrent.Click += new System.EventHandler(this.bntCheckCurrent_Click);
            // 
            // grpManualControl
            // 
            this.grpManualControl.Controls.Add(this.grpManualActions);
            this.grpManualControl.Controls.Add(this.btnScanForProcessors);
            this.grpManualControl.Controls.Add(this.btnScanFiles);
            this.grpManualControl.Controls.Add(this.btnConnectToNimble);
            this.grpManualControl.Controls.Add(this.btnCycle);
            this.grpManualControl.Location = new System.Drawing.Point(619, 110);
            this.grpManualControl.Name = "grpManualControl";
            this.grpManualControl.Size = new System.Drawing.Size(333, 191);
            this.grpManualControl.TabIndex = 12;
            this.grpManualControl.TabStop = false;
            this.grpManualControl.Text = "Manual Control";
            // 
            // grpManualActions
            // 
            this.grpManualActions.Controls.Add(this.bntCheckCurrent);
            this.grpManualActions.Controls.Add(this.btnDisconnect);
            this.grpManualActions.Location = new System.Drawing.Point(171, 19);
            this.grpManualActions.Name = "grpManualActions";
            this.grpManualActions.Size = new System.Drawing.Size(153, 137);
            this.grpManualActions.TabIndex = 14;
            this.grpManualActions.TabStop = false;
            this.grpManualActions.Text = "Actions";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(6, 54);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(132, 23);
            this.btnDisconnect.TabIndex = 13;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnAutoOperation
            // 
            this.btnAutoOperation.Enabled = false;
            this.btnAutoOperation.Location = new System.Drawing.Point(42, 153);
            this.btnAutoOperation.Name = "btnAutoOperation";
            this.btnAutoOperation.Size = new System.Drawing.Size(170, 23);
            this.btnAutoOperation.TabIndex = 13;
            this.btnAutoOperation.Text = "Toggle Automatic Operation";
            this.btnAutoOperation.UseVisualStyleBackColor = true;
            this.btnAutoOperation.Click += new System.EventHandler(this.btnAutoOperation_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.button1);
            this.grpSettings.Controls.Add(this.btnSetWorkingDir);
            this.grpSettings.Controls.Add(this.txtOutputDir);
            this.grpSettings.Controls.Add(this.txtWorkingDir);
            this.grpSettings.Location = new System.Drawing.Point(326, 12);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(626, 92);
            this.grpSettings.TabIndex = 14;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(505, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Set output directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSetWorkingDir
            // 
            this.btnSetWorkingDir.Location = new System.Drawing.Point(505, 20);
            this.btnSetWorkingDir.Name = "btnSetWorkingDir";
            this.btnSetWorkingDir.Size = new System.Drawing.Size(115, 23);
            this.btnSetWorkingDir.TabIndex = 2;
            this.btnSetWorkingDir.Text = "Set working directory";
            this.btnSetWorkingDir.UseVisualStyleBackColor = true;
            this.btnSetWorkingDir.Click += new System.EventHandler(this.btnSetWorkingDir_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutputDir.Location = new System.Drawing.Point(7, 47);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.ReadOnly = true;
            this.txtOutputDir.Size = new System.Drawing.Size(492, 20);
            this.txtOutputDir.TabIndex = 1;
            this.txtOutputDir.Text = "txtOutputDir";
            // 
            // txtWorkingDir
            // 
            this.txtWorkingDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtWorkingDir.Location = new System.Drawing.Point(7, 20);
            this.txtWorkingDir.Name = "txtWorkingDir";
            this.txtWorkingDir.ReadOnly = true;
            this.txtWorkingDir.Size = new System.Drawing.Size(492, 20);
            this.txtWorkingDir.TabIndex = 0;
            this.txtWorkingDir.Text = "txtWorkingDir";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Automatically get impedance for:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(537, 389);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(142, 99);
            this.textBox1.TabIndex = 16;
            // 
            // pannel_FoundProcessors
            // 
            this.pannel_FoundProcessors.Controls.Add(this.cklFoundDevices);
            this.pannel_FoundProcessors.Controls.Add(this.lblDevicesRefreshTime);
            this.pannel_FoundProcessors.Controls.Add(this.label1);
            this.pannel_FoundProcessors.Location = new System.Drawing.Point(326, 110);
            this.pannel_FoundProcessors.Name = "pannel_FoundProcessors";
            this.pannel_FoundProcessors.Size = new System.Drawing.Size(248, 192);
            this.pannel_FoundProcessors.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 536);
            this.Controls.Add(this.pannel_FoundProcessors);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.btnAutoOperation);
            this.Controls.Add(this.grpManualControl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbCOMPorts);
            this.Name = "Form1";
            this.Text = "Nimble Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDoubleClick);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grpManualControl.ResumeLayout(false);
            this.grpManualActions.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.pannel_FoundProcessors.ResumeLayout(false);
            this.pannel_FoundProcessors.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCOMPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCycle;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblDongleStatus;
        private System.Windows.Forms.Button btnScanForProcessors;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnConnectToNimble;
        private System.Windows.Forms.CheckedListBox cklFoundDevices;
        private System.Windows.Forms.Label lblDevicesRefreshTime;
        private System.Windows.Forms.Button btnScanFiles;
        private System.Windows.Forms.Button bntCheckCurrent;
        private System.Windows.Forms.GroupBox grpManualControl;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ToolStripStatusLabel lblRemoteDeviceStatus;
        private System.Windows.Forms.GroupBox grpManualActions;
        private System.Windows.Forms.Button btnAutoOperation;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblAutoStatus;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSetWorkingDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.TextBox txtWorkingDir;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pannel_FoundProcessors;
    }
}

