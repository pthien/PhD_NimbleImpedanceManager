using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshComPorts();
            nimble = new NimbleCommsManager();
            filemanager = new SequenceFileManager();
            UpdateStatusStrip();
            nimble.ConnectedToNimble += nimble_ConnectedToNimble;
            nimble.DisconnectedFromNimble += nimble_ConnectedToNimble;
        }

        void nimble_ConnectedToNimble(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            UpdateStatusStrip();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void UpdateStatusStrip()
        {
            if (nimble.Initialised)
            {
                lblDongleStatus.Text = string.Format("Connected to dongle on {0}", nimble.Comport);
                if (nimble.ConnectedToRemoteDevice)
                {
                    lblRemoteDeviceStatus.Text = string.Format("Connected to {0}", nimble.RemoteDeviceId);
                }
                else
                    lblRemoteDeviceStatus.Text = "Not connected to any nimble processors";
            }
            else
                lblDongleStatus.Text = "Not connected to dongle";


        }


        private void RefreshComPorts()
        {
            cmbCOMPorts.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();
            cmbCOMPorts.Items.AddRange(portNames);
            cmbCOMPorts.Text = portNames.FirstOrDefault();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string port = cmbCOMPorts.Text;
            ConnectedToDongle(nimble.Initialise(port));

            UpdateStatusStrip();
        }

        private void ConnectedToDongle(bool connected)
        {
            btnConnect.Enabled = !connected;
            btnCycle.Enabled = connected;
            btnStartScan.Enabled = connected;
        }

        private void btnCycle_Click(object sender, EventArgs e)
        {
            //string[] lines = txtAddresses.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (object o in cklFoundDevices.Items)
            {
                string line = o.ToString();
                nimble.ConnectToNimble(line);
                nimble.StartTelemCapture();
                System.Threading.Thread.Sleep(10000);
                nimble.EndTelemCapture();
                nimble.DisconnectFromNimble();
            }
        }

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

        private void cmbCOMPorts_MouseClick(object sender, MouseEventArgs e)
        {
            RefreshComPorts();
        }

        private void txtAddresses_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string line = cklFoundDevices.Items[0].ToString();
            nimble.ConnectToNimble(line);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            nimble.GetNimbleName();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            filemanager.ScanDirectory("");
        }

        private void bntCheckCurrent_Click(object sender, EventArgs e)
        {
            var guid = nimble.GetSequenceGUID();
            logger.Debug("got sequence id: {0}", guid);

            if (filemanager.FilesByGenGUID.ContainsKey(guid))
            {
                logger.Debug("collecting telem data for sequence {0}", guid);
                FilesForGenerationGUID x = filemanager.FilesByGenGUID[guid];
                foreach (KeyValuePair<int, string> kvp in x.sequenceFile.MeasurementSegments)
                {
                    var z = nimble.CollectTelemetryData(kvp.Key);
                    Console.Write(z.ToString());
                }
            }
            
        }
    }
}
