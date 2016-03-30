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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cmbCOMPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCycle = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblDongleStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRemoteDeviceStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAutoStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnScanForProcessors = new System.Windows.Forms.Button();
            this.btnConnectToNimble = new System.Windows.Forms.Button();
            this.cklFoundDevices = new System.Windows.Forms.CheckedListBox();
            this.lblDevicesRefreshTime = new System.Windows.Forms.Label();
            this.btnScanFiles = new System.Windows.Forms.Button();
            this.bntCheckCurrent = new System.Windows.Forms.Button();
            this.grpManualControl = new System.Windows.Forms.GroupBox();
            this.grpManualActions = new System.Windows.Forms.GroupBox();
            this.chkCheckSegments = new System.Windows.Forms.CheckedListBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnAutoOperation = new System.Windows.Forms.Button();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSetWorkingDir = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.txtWorkingDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pannel_FoundProcessors = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.btnUptime = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblAutoStatus_Measure = new System.Windows.Forms.Label();
            this.lblAutoStatus_alive = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lstFoundSequences = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.rtbWarnings = new System.Windows.Forms.RichTextBox();
            this.tmrAuto = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.btnSetLevel = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.grpManualControl.SuspendLayout();
            this.grpManualActions.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.pannel_FoundProcessors.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbCOMPorts
            // 
            this.cmbCOMPorts.FormattingEnabled = true;
            this.cmbCOMPorts.Location = new System.Drawing.Point(19, 116);
            this.cmbCOMPorts.Name = "cmbCOMPorts";
            this.cmbCOMPorts.Size = new System.Drawing.Size(158, 21);
            this.cmbCOMPorts.TabIndex = 0;
            this.cmbCOMPorts.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbCOMPorts_MouseClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(183, 114);
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
            this.btnCycle.Size = new System.Drawing.Size(151, 23);
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
            this.statusStrip1.Size = new System.Drawing.Size(915, 24);
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
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(654, 19);
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
            this.btnScanForProcessors.Size = new System.Drawing.Size(151, 23);
            this.btnScanForProcessors.TabIndex = 5;
            this.btnScanForProcessors.Text = "Scan for Nimble Processors";
            this.btnScanForProcessors.UseVisualStyleBackColor = true;
            this.btnScanForProcessors.Click += new System.EventHandler(this.btnStartScan_Click);
            // 
            // btnConnectToNimble
            // 
            this.btnConnectToNimble.Location = new System.Drawing.Point(6, 27);
            this.btnConnectToNimble.Name = "btnConnectToNimble";
            this.btnConnectToNimble.Size = new System.Drawing.Size(151, 23);
            this.btnConnectToNimble.TabIndex = 7;
            this.btnConnectToNimble.Text = "Connect to Nimble";
            this.btnConnectToNimble.UseVisualStyleBackColor = true;
            this.btnConnectToNimble.Click += new System.EventHandler(this.btnConnectToNimble_Click);
            // 
            // cklFoundDevices
            // 
            this.cklFoundDevices.FormattingEnabled = true;
            this.cklFoundDevices.Location = new System.Drawing.Point(3, 24);
            this.cklFoundDevices.Name = "cklFoundDevices";
            this.cklFoundDevices.Size = new System.Drawing.Size(223, 109);
            this.cklFoundDevices.TabIndex = 8;
            // 
            // lblDevicesRefreshTime
            // 
            this.lblDevicesRefreshTime.Location = new System.Drawing.Point(3, 136);
            this.lblDevicesRefreshTime.Name = "lblDevicesRefreshTime";
            this.lblDevicesRefreshTime.Size = new System.Drawing.Size(223, 40);
            this.lblDevicesRefreshTime.TabIndex = 9;
            this.lblDevicesRefreshTime.Click += new System.EventHandler(this.lblDevicesRefreshTime_Click);
            // 
            // btnScanFiles
            // 
            this.btnScanFiles.Location = new System.Drawing.Point(6, 84);
            this.btnScanFiles.Name = "btnScanFiles";
            this.btnScanFiles.Size = new System.Drawing.Size(151, 23);
            this.btnScanFiles.TabIndex = 10;
            this.btnScanFiles.Text = "Scan for data files";
            this.btnScanFiles.UseVisualStyleBackColor = true;
            this.btnScanFiles.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // bntCheckCurrent
            // 
            this.bntCheckCurrent.Location = new System.Drawing.Point(6, 169);
            this.bntCheckCurrent.Name = "bntCheckCurrent";
            this.bntCheckCurrent.Size = new System.Drawing.Size(113, 24);
            this.bntCheckCurrent.TabIndex = 11;
            this.bntCheckCurrent.Text = "Check Impedances";
            this.bntCheckCurrent.UseVisualStyleBackColor = true;
            this.bntCheckCurrent.Click += new System.EventHandler(this.bntCheckCurrent_Click);
            // 
            // grpManualControl
            // 
            this.grpManualControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpManualControl.Controls.Add(this.grpManualActions);
            this.grpManualControl.Controls.Add(this.btnScanForProcessors);
            this.grpManualControl.Controls.Add(this.btnScanFiles);
            this.grpManualControl.Controls.Add(this.btnConnectToNimble);
            this.grpManualControl.Controls.Add(this.btnCycle);
            this.grpManualControl.Location = new System.Drawing.Point(502, 9);
            this.grpManualControl.Name = "grpManualControl";
            this.grpManualControl.Size = new System.Drawing.Size(405, 307);
            this.grpManualControl.TabIndex = 12;
            this.grpManualControl.TabStop = false;
            this.grpManualControl.Text = "Manual Control";
            // 
            // grpManualActions
            // 
            this.grpManualActions.Controls.Add(this.btnSetLevel);
            this.grpManualActions.Controls.Add(this.chkCheckSegments);
            this.grpManualActions.Controls.Add(this.bntCheckCurrent);
            this.grpManualActions.Controls.Add(this.btnDisconnect);
            this.grpManualActions.Location = new System.Drawing.Point(163, 17);
            this.grpManualActions.Name = "grpManualActions";
            this.grpManualActions.Size = new System.Drawing.Size(236, 273);
            this.grpManualActions.TabIndex = 14;
            this.grpManualActions.TabStop = false;
            this.grpManualActions.Text = "Actions";
            // 
            // chkCheckSegments
            // 
            this.chkCheckSegments.CheckOnClick = true;
            this.chkCheckSegments.FormattingEnabled = true;
            this.chkCheckSegments.Location = new System.Drawing.Point(6, 19);
            this.chkCheckSegments.Name = "chkCheckSegments";
            this.chkCheckSegments.Size = new System.Drawing.Size(224, 139);
            this.chkCheckSegments.TabIndex = 14;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(125, 170);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(105, 23);
            this.btnDisconnect.TabIndex = 13;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnAutoOperation
            // 
            this.btnAutoOperation.Enabled = false;
            this.btnAutoOperation.Location = new System.Drawing.Point(19, 155);
            this.btnAutoOperation.Name = "btnAutoOperation";
            this.btnAutoOperation.Size = new System.Drawing.Size(239, 23);
            this.btnAutoOperation.TabIndex = 13;
            this.btnAutoOperation.Text = "Toggle Automatic Operation";
            this.btnAutoOperation.UseVisualStyleBackColor = true;
            this.btnAutoOperation.Click += new System.EventHandler(this.btnAutoOperation_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSettings.Controls.Add(this.button1);
            this.grpSettings.Controls.Add(this.btnSetWorkingDir);
            this.grpSettings.Controls.Add(this.txtOutputDir);
            this.grpSettings.Controls.Add(this.txtWorkingDir);
            this.grpSettings.Location = new System.Drawing.Point(12, 9);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(484, 92);
            this.grpSettings.TabIndex = 14;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(366, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Set output directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSetWorkingDir
            // 
            this.btnSetWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetWorkingDir.Location = new System.Drawing.Point(366, 20);
            this.btnSetWorkingDir.Name = "btnSetWorkingDir";
            this.btnSetWorkingDir.Size = new System.Drawing.Size(115, 23);
            this.btnSetWorkingDir.TabIndex = 2;
            this.btnSetWorkingDir.Text = "Set working directory";
            this.btnSetWorkingDir.UseVisualStyleBackColor = true;
            this.btnSetWorkingDir.Click += new System.EventHandler(this.btnSetWorkingDir_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutputDir.Location = new System.Drawing.Point(7, 47);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.ReadOnly = true;
            this.txtOutputDir.Size = new System.Drawing.Size(353, 20);
            this.txtOutputDir.TabIndex = 1;
            this.txtOutputDir.Text = "txtOutputDir";
            // 
            // txtWorkingDir
            // 
            this.txtWorkingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWorkingDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtWorkingDir.Location = new System.Drawing.Point(7, 20);
            this.txtWorkingDir.Name = "txtWorkingDir";
            this.txtWorkingDir.ReadOnly = true;
            this.txtWorkingDir.Size = new System.Drawing.Size(353, 20);
            this.txtWorkingDir.TabIndex = 0;
            this.txtWorkingDir.Text = "txtWorkingDir";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 16);
            this.label1.TabIndex = 15;
            this.label1.Text = "Automatically monitor impedance for:";
            // 
            // pannel_FoundProcessors
            // 
            this.pannel_FoundProcessors.Controls.Add(this.cklFoundDevices);
            this.pannel_FoundProcessors.Controls.Add(this.lblDevicesRefreshTime);
            this.pannel_FoundProcessors.Controls.Add(this.label1);
            this.pannel_FoundProcessors.Location = new System.Drawing.Point(264, 112);
            this.pannel_FoundProcessors.Name = "pannel_FoundProcessors";
            this.pannel_FoundProcessors.Size = new System.Drawing.Size(232, 192);
            this.pannel_FoundProcessors.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(19, 196);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(239, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "Measurement Viewer";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnUptime
            // 
            this.btnUptime.Location = new System.Drawing.Point(19, 243);
            this.btnUptime.Name = "btnUptime";
            this.btnUptime.Size = new System.Drawing.Size(239, 23);
            this.btnUptime.TabIndex = 21;
            this.btnUptime.Text = "Uptime Viewer";
            this.btnUptime.UseVisualStyleBackColor = true;
            this.btnUptime.Click += new System.EventHandler(this.btnUptime_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblTime);
            this.tabPage2.Controls.Add(this.lblAutoStatus_Measure);
            this.tabPage2.Controls.Add(this.lblAutoStatus_alive);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(907, 155);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Timers";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblTime
            // 
            this.lblTime.Location = new System.Drawing.Point(36, 13);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(378, 48);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "label4";
            // 
            // lblAutoStatus_Measure
            // 
            this.lblAutoStatus_Measure.Location = new System.Drawing.Point(36, 61);
            this.lblAutoStatus_Measure.Name = "lblAutoStatus_Measure";
            this.lblAutoStatus_Measure.Size = new System.Drawing.Size(302, 72);
            this.lblAutoStatus_Measure.TabIndex = 1;
            this.lblAutoStatus_Measure.Text = "...";
            this.lblAutoStatus_Measure.Click += new System.EventHandler(this.lblAutoStatus_Measure_Click);
            // 
            // lblAutoStatus_alive
            // 
            this.lblAutoStatus_alive.Location = new System.Drawing.Point(549, 61);
            this.lblAutoStatus_alive.Name = "lblAutoStatus_alive";
            this.lblAutoStatus_alive.Size = new System.Drawing.Size(274, 72);
            this.lblAutoStatus_alive.TabIndex = 0;
            this.lblAutoStatus_alive.Text = "...";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(907, 155);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Program Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(907, 155);
            this.richTextBox1.TabIndex = 19;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 328);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(915, 181);
            this.tabControl1.TabIndex = 22;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lstFoundSequences);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(907, 155);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Sequences";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lstFoundSequences
            // 
            this.lstFoundSequences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFoundSequences.FormattingEnabled = true;
            this.lstFoundSequences.Location = new System.Drawing.Point(3, 3);
            this.lstFoundSequences.Name = "lstFoundSequences";
            this.lstFoundSequences.Size = new System.Drawing.Size(845, 147);
            this.lstFoundSequences.TabIndex = 23;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.rtbWarnings);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(907, 155);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Warnings";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // rtbWarnings
            // 
            this.rtbWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbWarnings.Location = new System.Drawing.Point(3, 3);
            this.rtbWarnings.Name = "rtbWarnings";
            this.rtbWarnings.Size = new System.Drawing.Size(845, 149);
            this.rtbWarnings.TabIndex = 0;
            this.rtbWarnings.Text = "";
            // 
            // tmrAuto
            // 
            this.tmrAuto.Enabled = true;
            this.tmrAuto.Interval = 1000;
            this.tmrAuto.Tick += new System.EventHandler(this.tmrAuto_Tick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(362, 310);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(66, 25);
            this.button3.TabIndex = 23;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnSetLevel
            // 
            this.btnSetLevel.Location = new System.Drawing.Point(125, 239);
            this.btnSetLevel.Name = "btnSetLevel";
            this.btnSetLevel.Size = new System.Drawing.Size(105, 23);
            this.btnSetLevel.TabIndex = 15;
            this.btnSetLevel.Text = "Set Level";
            this.btnSetLevel.UseVisualStyleBackColor = true;
            this.btnSetLevel.Click += new System.EventHandler(this.btnSetLevel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 536);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnUptime);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pannel_FoundProcessors);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.btnAutoOperation);
            this.Controls.Add(this.grpManualControl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbCOMPorts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.pannel_FoundProcessors.ResumeLayout(false);
            this.pannel_FoundProcessors.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pannel_FoundProcessors;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnUptime;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox lstFoundSequences;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox rtbWarnings;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblAutoStatus_Measure;
        private System.Windows.Forms.Label lblAutoStatus_alive;
        private System.Windows.Forms.Timer tmrAuto;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckedListBox chkCheckSegments;
        private System.Windows.Forms.Button btnSetLevel;
    }
}

