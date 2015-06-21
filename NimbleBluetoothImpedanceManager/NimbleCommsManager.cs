using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using System.Text.RegularExpressions;


namespace NimbleBluetoothImpedanceManager
{
    class NimbleCommsManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public BluetoothCommsDriver btDongle;

        private string comport;

        public string Comport
        {
            get { return comport; }
        }
        private bool initialised = false;

        public bool Initialised
        {
            get { return initialised; }
        }

    

        public bool ConnectedToRemoteDevice
        {
            get { return btDongle.ConnectedToRemoteDevice; }
        }

        public event BluetoothCommsDriver.ConnectionEstablishedEventHandler ConnectedToNimble
        {
            add { btDongle.ConnectionEstablished += value; }
            remove { btDongle.ConnectionEstablished -= value; }
        }

        public event BluetoothCommsDriver.ConnectionLostEventHandler DisconnectedFromNimble
        {
            add { btDongle.ConnectionLost += value; }
            remove { btDongle.ConnectionLost -= value; }
        }

        public delegate void ConnectionLostEventHandler(object sender, EventArgs e);

        public string RemoteDeviceId
        {
            get { return btDongle.RemoteDeviceId; }
        }

        public NimbleCommsManager()
        {
            btDongle = new BluetoothCommsDriver();
            btDongle.DataReceivedFromRemoteDevice += new BluetoothCommsDriver.DataReceivedEventHandler(btDongle_DataReceived);
            btDongle.ConnectionLost += btDongle_ConnectionLost;
        }

        void btDongle_ConnectionLost(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            
        }

        public void ConnectToNimble(string address)
        {
            btDongle.ConnectToRemoteDevice(address);
        }

        void btDongle_DataReceived(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            logger.Info("Received nimble command response: {0}", e.RecievedData);
            //throw new NotImplementedException();
            ParseCommandResponse(e.RecievedData);
        }

        static Regex regex_ID = new Regex(@"{([A-Za-z0-9]+):([ A-Z0-9a-z-:|()]+)}");
        private void ParseCommandResponse(string p)
        {
            if (regex_ID.IsMatch(p))
            {
                var matches = regex_ID.Match(p);
                if (matches.Success)
                {
                    string command = matches.Groups[1].Value;
                    string data = matches.Groups[2].Value;
                    //_RemoteDeviceID = command;

                    ProcessData(command, data);
                }
            }
        }

        private void ProcessData(string command, string data)
        {
            switch (command)
            {
                case "ID":
                    break;
                case "Telem":
                    break;
            }
            //throw new NotImplementedException();
        }

        public bool Initialise(string COMPort)
        {
            logger.Info("Initialising...");
            comport = COMPort;
            bool connected = btDongle.ConnectToDongle(COMPort);
            if (!connected)
            {
                logger.Warn("Initialisation failed, could not connect to dongle");
                return false;
            }
            if (!btDongle.IsDongleOK())
            {

                if (btDongle.Dongle_ConnectionLost_WaitHandle.WaitOne(1))
                {
                    logger.Warn("Dongle already connected to slave. Reattempting OK Check");
                    if (!btDongle.IsDongleOK())
                        return false;
                }
                else
                {
                    logger.Warn("Initialisation failed, dongle not ok");
                    return false;
                }

            }

            logger.Info("Initialisation successful");
            initialised = true;
            return true;
        }

        public string[] DiscoverDevices()
        {
            return btDongle.DiscoverDevices();
        }


        public void CollectTelemetryData()
        {

        }

        public bool GetNimbleName()
        {
            try
            {
                btDongle.TransmitToRemoteDevice("\nGetID\n");
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
           
            return true;
        }

        public void StartTelemCapture()
        {
            btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
        }

        public void EndTelemCapture()
        {
            btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
        }

        public bool DisconnectFromNimble()
        {
            if (!btDongle.ConnectedToRemoteDevice)
                return false;

            btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
            Thread.Sleep(500);
            //btDongle.TransmitAndLog("endSession\n");
            btDongle.TransmitAndLog("AT");
            if (btDongle.Dongle_ConnectionLost_WaitHandle.WaitOne(10000))
            {
                logger.Info("Successfully disconnected from Nimble processor");
            }
            else
            {
                logger.Info("Failed to disconnect");
            }

            return true;
        }


    }
}
