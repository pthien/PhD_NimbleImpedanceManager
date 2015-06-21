using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NimbleBluetoothImpedanceManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //BluetoothCommsDriver bcm = new BluetoothCommsDriver();
        private NimbleCommsManager nimble;// = new NimbleCommsManager("COM42");
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshComPorts();
            nimble = new NimbleCommsManager();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           
        }


        private void RefreshComPorts()
        {
            cmbCOMPorts.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();
            cmbCOMPorts.Items.AddRange(portNames);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string port = cmbCOMPorts.Text;
            nimble.Initialise(port);
        }

        private void btnCycle_Click(object sender, EventArgs e)
        {
            string[] lines = txtAddresses.Text.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                nimble.ConnectToNimble(line);
                nimble.StartTelemCapture();
                System.Threading.Thread.Sleep(10000);
                nimble.EndTelemCapture();
                nimble.DisconnectFromNimble();
            }
        }

        private void btnStartScan_Click(object sender, EventArgs e)
        {

        }

        private void cmbCOMPorts_MouseClick(object sender, MouseEventArgs e)
        {
            RefreshComPorts();
        }
    }
}
