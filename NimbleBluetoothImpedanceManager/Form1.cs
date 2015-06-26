using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NimbleBluetoothImpedanceManager.Properties;
using NimbleBluetoothImpedanceManager.Sequences;
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
        private bool AutomaticOperation = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshComPorts();
            nimble = new NimbleCommsManager();
            filemanager = new SequenceFileManager();
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
            logger.Debug("State changed to {0}", e.NewState);
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
            NimbleCommsManager.NimbleState state = nimble.State;

            string textCon = "";
            string textStatus = "";
            switch (state)
            {
                case NimbleCommsManager.NimbleState.Disconnected:
                    textCon = "Not connected to dongle";
                    textStatus = "Not ready";
                    break;
                case NimbleCommsManager.NimbleState.ConnectingToDongle:
                    textCon = "Connecting to dongle ({0}), nimble.Comport";
                    textStatus = "Not ready";
                    break;
                case NimbleCommsManager.NimbleState.ConnectedToDongle:
                    textCon = string.Format("Connected to dongle ({0}), not connected to remote device", nimble.Comport);
                    textStatus = "Not ready";
                    break;
                case NimbleCommsManager.NimbleState.ConnectedToDongleAndBusy:
                    textCon = string.Format("Connected to dongle ({0}), not connected to remote device", nimble.Comport);
                    textStatus = "Busy";
                    break;
                case NimbleCommsManager.NimbleState.ConnectingToNimble:
                    textCon = "Connecting to nimble processor " + nimble.RemoteDeviceId;
                    textStatus = "Not ready";
                    break;
                case NimbleCommsManager.NimbleState.ConnectedToNimbleAndReady:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Ready";
                    break;
                case NimbleCommsManager.NimbleState.ConnectedToNimbleAndError:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Error";
                    break;
                case NimbleCommsManager.NimbleState.ConnectedToNimbleAndWorking:
                    textCon = string.Format("Connected to nimble processor {0}({1}) via {2}", nimble.NimbleName, nimble.RemoteDeviceId, nimble.Comport);
                    textStatus = "Working...";
                    break;
                default:
                    break;
            }

            lblRemoteDeviceStatus.Text = textStatus;
            lblDongleStatus.Text = textCon;
            lblAutoStatus.Text = "Automatic Impedance Collection is " + (AutomaticOperation ? "On" : "Off");


            if (this.InvokeRequired)
                this.BeginInvoke((Action)(() =>
                {
                    UpdateUI(state, AutomaticOperation);
                    logger.Trace("Update ui from {0}", Thread.CurrentThread.Name);
                }));
            else
                UpdateUI(state, AutomaticOperation);
        }

        private void UpdateUI(NimbleCommsManager.NimbleState state, bool automaticControl)
        {
            grpManualControl.Enabled = (state == NimbleCommsManager.NimbleState.ConnectedToNimbleAndReady ||
                                       state == NimbleCommsManager.NimbleState.ConnectedToDongle) && !automaticControl;
            grpManualActions.Enabled = state == NimbleCommsManager.NimbleState.ConnectedToNimbleAndReady;
            btnConnectToNimble.Enabled = state == NimbleCommsManager.NimbleState.ConnectedToDongle;
            btnStartScan.Enabled = state == NimbleCommsManager.NimbleState.ConnectingToDongle;

            grpSettings.Enabled = !AutomaticOperation && state <= NimbleCommsManager.NimbleState.ConnectedToDongle;
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
            string port = cmbCOMPorts.Text;
            var res = nimble.Initialise(port);
            ConnectedToDongle(res);
            if (res)
            {
                Properties.Settings.Default.DefaultComDevice = port;
                Properties.Settings.Default.Save();
                AutomaticOperation = false;
            }
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
            lblDevicesRefreshTime.Text = "Last refresh at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            var devs = nimble.DiscoverDevices();
            cklFoundDevices.Items.Clear();
            //txtAddresses.Text = "";
            if (devs == null)
                return;
            foreach (string s in devs)
            {
                cklFoundDevices.Items.Add(s);
                cklFoundDevices.SetItemCheckState(0, CheckState.Indeterminate);
                //txtAddresses.Text = s + "\r\n";
            }
        }

        private void bntCheckCurrent_Click(object sender, EventArgs e)
        {
            DoImpedanceCheck();
        }

        private void DoImpedanceCheck()
        {
            var guid = nimble.GetSequenceGUID();
            logger.Debug("got sequence id: {0}", guid);

            if (filemanager.FilesByGenGUID.ContainsKey(guid))
            {
                logger.Info("Collecting telem data for sequence {0} on {1}({2})", guid, nimble.NimbleName, nimble.RemoteDeviceId);
                FilesForGenerationGUID x = filemanager.FilesByGenGUID[guid];
                var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);
                foreach (KeyValuePair<int, string> kvp in x.sequenceFile.MeasurementSegments)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string[] telemData;
                        bool res = nimble.CollectTelemetryData(kvp.Key, out telemData);
                        if (!res)
                        {
                            if (telemData == null)
                                telemData = new string[] {"Collection failed. no data collected"};
                        }
                        SaveTelemData(telemData, kvp.Value, fullSavePath);
                    }
                }
            }
        }

        private void SaveTelemData(string[] telemData, string measurementName, string folder)
        {
            int file_counter = 0;
            string rawDataFileName = string.Format("{0}_{1}.txt", measurementName, file_counter);
            while (File.Exists(Path.Combine(folder, rawDataFileName)))
            {
                file_counter++;
                rawDataFileName = string.Format("{0}_{1}.txt", measurementName, file_counter);
            }

            string rawDataPath = Path.Combine(folder, rawDataFileName);

            FileStream fs = new FileStream(rawDataPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            if (telemData != null)
                foreach (string s in telemData)
                {
                    sw.WriteLine(s);
                }
            sw.Flush();
            fs.Close();
        }

        private string GetTelemSavePath(string RemoteAddr, string NimbleName, string GenGuid)
        {
            string outputDir = txtOutputDir.Text;

            string saveFolder = string.Format("{0}-{1}-{2}-{3}", NimbleName, RemoteAddr, GenGuid,
                DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt"));

            string fullSavePath = Path.Combine(outputDir, saveFolder);
            if (Directory.Exists(fullSavePath))
            {
                logger.Error("Output folder for this impedance measurement already exists. {0}", fullSavePath);
            }
            else
            {
                Directory.CreateDirectory(fullSavePath);
            }
            return fullSavePath;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            nimble.DisconnectFromNimble();
        }

        private void btnConnectToNimble_Click(object sender, EventArgs e)
        {
            if (cklFoundDevices.SelectedIndex >= 0)
            {
                string line = cklFoundDevices.Items[cklFoundDevices.SelectedIndex].ToString();
                nimble.ConnectToNimble(line);
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
            filemanager.ScanDirectory("");
        }

        private void btnAutoOperation_Click(object sender, EventArgs e)
        {
            AutomaticOperation = !AutomaticOperation;
            UpdateStatusStrip();
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


    }
}
