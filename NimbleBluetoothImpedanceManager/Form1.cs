using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NimbleBluetoothImpedanceManager.Properties;
using Nimble.Sequences;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    public partial class Form1 : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Form1()
        {
            InitializeComponent();
        }
        //BluetoothCommsDriver bcm = new BluetoothCommsDriver();
        private NimbleCommsManager nimble;
        private SequenceFileManager filemanager;
        private AutomaticNimbleController autoNimble;


        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshComPorts();
            nimble = new NimbleCommsManager();
            filemanager = new SequenceFileManager();
            autoNimble = new AutomaticNimbleController(nimble, filemanager);

            UpdateStatusStrip();
            nimble.ConnectedToNimble += nimble_ConnectedToNimble;
            nimble.DisconnectedFromNimble += nimble_ConnectedToNimble;
            nimble.StateChanged += new NimbleCommsManager.StateChangedEventHandler(nimble_StateChanged);

            if (Settings.Default.ImpedanceOutputFolder == "")
                Settings.Default.ImpedanceOutputFolder = Directory.GetCurrentDirectory();
            txtOutputDir.Text = Settings.Default.ImpedanceOutputFolder;

            if (Settings.Default.SequenceScanFolder == "")
                Settings.Default.SequenceScanFolder = Directory.GetCurrentDirectory();
            txtWorkingDir.Text = Settings.Default.SequenceScanFolder;

        }

        void nimble_StateChanged(object sender, NimbleCommsManager.StateChangedEventArgs e)
        {
            UpdateStatusStrip();
        }

        void nimble_ConnectedToNimble(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            //UpdateStatusStrip();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void UpdateStatusStrip()
        {
            NimbleState state = nimble.State;

            string textCon = "";
            Color clrReady = Color.LimeGreen;
            Color clrWorking = Color.Gold;
            Color clrNotReady = Color.Firebrick;

            Color colourCon = SystemColors.ControlLight;
            Color colourStatus = SystemColors.ControlLight;
            string textStatus = "";
            switch (state)
            {
                case NimbleState.Disconnected:
                    textCon = "Not connected to dongle";
                    textStatus = "Not ready";
                    colourCon = clrNotReady;
                    colourStatus = clrNotReady;
                    break;
                case NimbleState.ConnectingToDongle:
                    textCon = "Connecting to dongle ({0}), nimble.Comport";
                    textStatus = "Not ready";
                    colourCon = clrWorking;
                    colourStatus = clrNotReady;
                    break;
                case NimbleState.ConnectedToDongle:
                    textCon = string.Format("Connected to dongle ({0}), not connected to remote device", nimble.Comport);
                    textStatus = "Idle";
                    colourCon = clrReady;
                    colourStatus = clrReady;
                    break;
                case NimbleState.ConnectedToDongleAndBusy:
                    textCon = string.Format("Connected to dongle ({0}), not connected to remote device", nimble.Comport);
                    textStatus = "Busy";
                    colourCon = clrReady;
                    colourStatus = clrWorking;
                    break;
                case NimbleState.ConnectingToNimble:
                    textCon = "Connecting to nimble processor " + nimble.RemoteDeviceId;
                    textStatus = "Not ready";
                    colourCon = clrWorking;
                    colourStatus = clrNotReady;
                    break;
                case NimbleState.ConnectedToNimbleAndReady:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Ready";
                    colourCon = clrReady;
                    colourStatus = clrReady;
                    break;
                case NimbleState.ConnectedToNimbleAndError:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Error";
                    colourCon = clrReady;
                    colourStatus = clrNotReady;
                    break;
                case NimbleState.ConnectedToNimbleAndWorking:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Working...";
                    colourCon = clrReady;
                    colourStatus = clrWorking;
                    break;
                default:
                    break;
            }



            if (this.InvokeRequired)
                this.BeginInvoke((Action)(() =>
                {
                    lblRemoteDeviceStatus.Text = textStatus;
                    lblRemoteDeviceStatus.BackColor = colourStatus;
                    lblDongleStatus.Text = textCon;
                    lblDongleStatus.BackColor = colourCon;
                    lblAutoStatus.Text = "Automatic Impedance Collection is " + (autoNimble.AutomaticControlEnabled ? "On" : "Off");

                    UpdateUI(state, autoNimble.AutomaticControlEnabled);
                    logger.Trace("Update ui from {0}", Thread.CurrentThread.Name);
                }));
            else
                UpdateUI(state, autoNimble.AutomaticControlEnabled);
        }

        private void UpdateUI(NimbleState state, bool automaticControl)
        {
            grpManualControl.Enabled = (state == NimbleState.ConnectedToNimbleAndReady ||
                                       state == NimbleState.ConnectedToDongle) && !automaticControl;
            grpManualActions.Enabled = state == NimbleState.ConnectedToNimbleAndReady;
            pannel_FoundProcessors.Enabled = grpManualControl.Enabled;
            btnConnectToNimble.Enabled = state == NimbleState.ConnectedToDongle;
            btnScanForProcessors.Enabled = state == NimbleState.ConnectedToDongle;

            grpSettings.Enabled = !autoNimble.AutomaticControlEnabled && state <= NimbleState.ConnectedToDongle;
        }


        private void RefreshComPorts()
        {
            cmbCOMPorts.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();
            cmbCOMPorts.Items.AddRange(portNames);
            if (Settings.Default.DefaultComDevice != "" && portNames.Contains(Settings.Default.DefaultComDevice))
                cmbCOMPorts.Text = Settings.Default.DefaultComDevice;
            else
                cmbCOMPorts.Text = portNames.FirstOrDefault();
        }

        private void btnConnectToDongle_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            string port = cmbCOMPorts.Text;
            var res = nimble.Initialise(port);
            if (res)
            {
                Properties.Settings.Default.DefaultComDevice = port;
                Properties.Settings.Default.Save();
                btnStartScan_Click(null, null);
            }
            btnConnect.Enabled = true;
            ConnectedToDongle(res);
        }

        private void ConnectedToDongle(bool connected)
        {
            btnConnect.Enabled = !connected;
            //btnCycle.Enabled = connected;
            btnAutoOperation.Enabled = connected;
            //btnStartScan.Enabled = connected;
        }

        private void btnCycle_Click(object sender, EventArgs e)
        {
            //string[] lines = txtAddresses.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //foreach (object o in cklFoundDevices.Items)
            //{
            //    string line = o.ToString();
            //    nimble.ConnectToNimble(line);
            //    nimble.StartTelemCapture();
            //    System.Threading.Thread.Sleep(10000);
            //    nimble.EndTelemCapture();
            //    nimble.DisconnectFromNimble();
            //}
        }

        #region ManualControl

        private void btnStartScan_Click(object sender, EventArgs e)
        {
            var items = autoNimble.AutoAction_DeepScanForProcessors();
            cklFoundDevices.Items.Clear();
            foreach (NimbleProcessor nimbleProcessor in items)
            {
                cklFoundDevices.Items.Add(nimbleProcessor);
            }
            lblDevicesRefreshTime.Text = "Last refresh at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                + string.Format("\r\nPress '{0}' to refresh.", btnScanForProcessors.Text);
            //var devs = nimble.DiscoverDevices();
            //cklFoundDevices.Items.Clear();
            ////txtAddresses.Text = "";
            //if (devs == null)
            //    return;
            //foreach (string s in devs)
            //{
            //    cklFoundDevices.Items.Add(s);
            //    cklFoundDevices.SetItemCheckState(0, CheckState.Indeterminate);
            //    //txtAddresses.Text = s + "\r\n";
            //}
        }

        private void bntCheckCurrent_Click(object sender, EventArgs e)
        {
            autoNimble.DoImpedanceCheck();
        }



        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            nimble.DisconnectFromNimble();
        }

        private void btnConnectToNimble_Click(object sender, EventArgs e)
        {
            if (cklFoundDevices.SelectedIndex >= 0)
            {
                var line = cklFoundDevices.Items[cklFoundDevices.SelectedIndex];
                if (line is NimbleProcessor)
                {
                    string addr = ((NimbleProcessor)line).BluetoothAddress;
                    nimble.ConnectToNimble(addr);
                }
            }
            else
            {
                MessageBox.Show("Please select a device");
            }
        }
        #endregion



        private void cmbCOMPorts_MouseClick(object sender, MouseEventArgs e)
        {
            RefreshComPorts();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            filemanager.ScanDirectory(Settings.Default.SequenceScanFolder);
        }

        private void btnAutoOperation_Click(object sender, EventArgs e)
        {
            btnAutoOperation.Enabled = false;
            if (autoNimble.AutomaticControlEnabled)
                autoNimble.StopAutomaticControl();
            else
            {
                if (cklFoundDevices.CheckedItems.Count == 0)
                {
                    var res =
                        MessageBox.Show(
                            "No processor has been selected for automatic impedance monitoring. Do you want to continue?",
                            "Warning", MessageBoxButtons.OKCancel);
                    if (res == DialogResult.Cancel)
                    {
                        btnAutoOperation.Enabled = true;
                        return;
                    }
                }
                filemanager.ScanDirectory(Settings.Default.SequenceScanFolder);
                List<NimbleProcessor> tmp = new List<NimbleProcessor>();
                foreach (object o in cklFoundDevices.CheckedItems)
                {
                    if (o is NimbleProcessor)
                        tmp.Add((NimbleProcessor)o);
                }
                autoNimble.SetProcessorsToMonitor(tmp);
                autoNimble.StartAutomaticControl();
            }

            UpdateStatusStrip();
            btnAutoOperation.Enabled = true;

        }

        private void btnSetWorkingDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = txtWorkingDir.Text;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtWorkingDir.Text = fbd.SelectedPath;
                Settings.Default.SequenceScanFolder = fbd.SelectedPath;
                Settings.Default.Save();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = txtOutputDir.Text;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtOutputDir.Text = fbd.SelectedPath;
                Settings.Default.ImpedanceOutputFolder = fbd.SelectedPath;
                Settings.Default.Save();
            }
        }

        private void lblDevicesRefreshTime_Click(object sender, EventArgs e)
        {

        }

        private Nimble.Sequences.Viewer x;
        private void button2_Click(object sender, EventArgs e)
        {
            x = new Viewer();
            x.ImpedanceDirectory = Settings.Default.ImpedanceOutputFolder;
            x.SequenceDirectory = Settings.Default.SequenceScanFolder;
            x.Show();
        }


    }
}
