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

        public EventWaitHandle NimbleCmdRx_GenGUID_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle NimbleCmdRx_ID_WaitHandle = new AutoResetEvent(false);
        private EventWaitHandle NimbleCmdRx_xmitTelemStart_WaitHandle = new AutoResetEvent(false);
        private EventWaitHandle NimbleCmdRx_xmitTelemFin_WaitHandle = new AutoResetEvent(false);

        public string RemoteDeviceId
        {
            get { return btDongle.RemoteDeviceId; }
        }

        private string remoteDeviceGenGUID;

        private object receivingTelemDataLock = new object();
        private string[] _telemData = new string[10];
        private List<string> telemData_temp = new List<string>();
        private bool receivingTelemData = false;
        private string[] TelemetryData
        {
            get
            {
                lock (receivingTelemDataLock)
                {
                    string[] copy = (string[])_telemData.Clone();
                    return copy;
                }
            }
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
            if (receivingTelemData)
            {
                lock (receivingTelemDataLock)
                {
                    telemData_temp.Add(e.RecievedData);
                }
            }
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
                    NimbleCmdRx_ID_WaitHandle.Set();
                    break;
                case "Telem":
                    break;
                case "GenGUID":
                    remoteDeviceGenGUID = data;
                    NimbleCmdRx_GenGUID_WaitHandle.Set();
                    break;
                case "xmitTelem":
                    lock (receivingTelemDataLock)
                    {
                        if (data == "fin")
                        {
                            receivingTelemData = false;
                            logger.Info("Recieving telem data finished");
                            _telemData = telemData_temp.ToArray();
                            NimbleCmdRx_xmitTelemFin_WaitHandle.Set();
                        }
                        if (data == "set")
                        {
                            receivingTelemData = true;
                            NimbleCmdRx_xmitTelemStart_WaitHandle.Set();
                            logger.Info("Recieving telem data started");
                            telemData_temp = new List<string>();
                        }
                    }
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


        //TODO: make this thread safe, add a lock around accessing receivngTelemData
        public string[] CollectTelemetryData(int sequence)
        {
            if(receivingTelemData)
                return null;
            //receivingTelemData = true;
            string command = string.Format("\nsetXmitTelem {0}\n", sequence);
            btDongle.TransmitToRemoteDevice(command);
            if (NimbleCmdRx_xmitTelemFin_WaitHandle.WaitOne(20000))
            {
                logger.Info("Receive Telem data successful. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
                return TelemetryData;
            }
            else
            {
                logger.Warn("Receive Telem timed out. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
                receivingTelemData = false;
                return null;
            }
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

        public string GetSequenceGUID()
        {
            btDongle.TransmitToRemoteDevice("\ngetGenGUID\n");
            if (NimbleCmdRx_GenGUID_WaitHandle.WaitOne(1000))
            {
                return remoteDeviceGenGUID;
            }
            return "";
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
