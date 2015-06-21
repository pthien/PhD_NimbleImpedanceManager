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
        private bool initialised = false;

        private string _RemoteDeviceID="";
        public string RemoteDeviceId
        {
            get { return _RemoteDeviceID; }
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

        public NimbleCommsManager()
        {
            btDongle = new BluetoothCommsDriver();
            btDongle.DataReceived += new BluetoothCommsDriver.DataReceivedEventHandler(btDongle_DataReceived);
        }

        void btDongle_DataReceived(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            logger.Info("Received nimble command response: {0}", e.RecievedData);
            //throw new NotImplementedException();
            ParseCommandResponse(e.RecievedData);
        }

        static Regex regex_ID = new Regex(@"{ID:([A-Z0-9a-z]+)}");
        private void ParseCommandResponse(string p)
        {
            if (regex_ID.IsMatch(p))
            {
                var matches = regex_ID.Match(p);
                if (matches.Success)
                {
                    string name = matches.Groups[1].Value;
                    _RemoteDeviceID = name;
                }
            }
           // throw new NotImplementedException();
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
                    if(!btDongle.IsDongleOK())
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

        public void ConnectToNimble(string Address)
        {
            btDongle.TransmitAndLog("AT+CON" + Address);
            if (btDongle.Dongle_ConnectionEstablished_WaitHandle.WaitOne(10000))
            {
                logger.Info("Connected to {0}", Address);
                GetNimbleName();
            }
            else
            {
                logger.Info("Connection to {0} timed out", Address);
            }
        }

        public void GetNimbleName()
        {
            btDongle.TransmitToRemoteDevice("\nGetID\n");
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
