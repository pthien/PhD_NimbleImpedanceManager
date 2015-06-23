using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    class BluetoothCommsDriver
    {
        private Logger logger = LogManager.GetLogger("DataLoggerOT"); //other

        private Logger dataLoggerTX = LogManager.GetLogger("DataLoggerTX");
        private Logger dataLoggerRX = LogManager.GetLogger("DataLoggerRX");

        private SerialPort serialPort;
        private DataChunker dataChunker;

        public EventWaitHandle Dongle_ATOK_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_OkGet_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_OkSet_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_OkName_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_DeviceDiscoveryComplete_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionAttempt_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionEstablished_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionLost_WaitHandle = new AutoResetEvent(false);

        const string OK_CONN_ATTEMPT = "OK+CONNA";
        const string OK_CONN_LOST = "OK+LOST";
        const string OK_CONN_ESTABLISHED = "OK+CONN";
        const string OK_GET = "OK+Get";
        const string OK_SET = "OK+Set";
        const string OK_NAME = "OK+NAME";
        const string AT_OK = "OK";
        const string OK_DISCOVERYSTART = "OK+DISCS";
        const string OK_DISCOVERYEND = "OK+DISCE";
        const string OK_DEVICEDISCOVERED = "OK+DISC:";


        public delegate void ConnectionEstablishedEventHandler(object sender, DataRecievedEventArgs e);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionLostEventHandler(object sender, DataRecievedEventArgs e);
        public event ConnectionLostEventHandler ConnectionLost;

        public delegate void DataReceivedEventHandler(object sender, DataRecievedEventArgs e);
        public event DataReceivedEventHandler DataReceivedFromRemoteDevice;

        //public delegate void OKGetEventHandler(object sender, DataRecievedEventArgs e);
        //public event OKGetEventHandler OKGet;

        //public delegate void OKSetEventHandler(object sender, DataRecievedEventArgs e);
        //public event OKSetEventHandler OKSet;

        //public delegate void OKNameEventHandler(object sender, DataRecievedEventArgs e);
        //public event OKNameEventHandler OKName;

        public class DataRecievedEventArgs : EventArgs
        {
            public string RecievedData { get; set; }
        }


        private string[] MostRecentlyRecievedData = new string[] { };

        private object knownDevicesLock = new object();
        private string[] _knownDevices = new string[10];
        private List<string> buildingKnownDevices = new List<string>();
        public string[] KnownDevices
        {
            get
            {
                lock (knownDevicesLock)
                {
                    string[] copy = (string[])_knownDevices.Clone();
                    return copy;
                }
            }
        }

        public bool ConnectedToRemoteDevice { get { return _connectedToRemoteDevice; } }

        private bool _connectedToRemoteDevice = false;

        private string _RemoteDeviceID = "";
        public string RemoteDeviceId
        {
            get { return _RemoteDeviceID; }
        }

        private bool currentlyScanningForDevices = false;

        public BluetoothCommsDriver()
        {
            dataChunker = new DataChunker();
            dataChunker.ChunkReady += dataChunker_ChunkReady;

            _connectedToRemoteDevice = false;
            logger.Info("*BluetoothCommsDriver initialised*");
        }

        /// <summary>
        /// Opens the the specified serial port. 
        /// </summary>
        /// <param name="CommPort"></param>
        /// <param name="BaudRate"></param>
        /// <returns></returns>
        public bool ConnectToDongle(string CommPort, int BaudRate = 57600)
        {
            try
            {
                serialPort = new SerialPort(CommPort, BaudRate);
                serialPort.Open();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                logger.Info("*Connection to dongle on {0} established*", CommPort);
            }
            catch (Exception ex)
            {
                logger.Warn("*Could not connect to port: {0}. {1}*", CommPort, ex.Message);
                return false;
            }
            return true;
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead > 0)
            {
                dataChunker.AddChar(Convert.ToChar(serialPort.ReadChar()));
            }
        }

        void dataChunker_ChunkReady(object sender, DataChunker.ChunkReadyEventArgs e)
        {
            logger.Debug("Got a chunk {0}", e.Chunk);
            dataLoggerRX.Info(e.Reason.ToString() + " " + e.Chunk.Replace("\n", "\\n").Replace("\r", "\\r"));
            MostRecentlyRecievedData = ChunkParser.ParseChunk(e.Chunk).ToArray();

            ProcessData(MostRecentlyRecievedData);
        }


        public void ConnectToRemoteDevice(string Address)
        {
            TransmitAndLog("AT+CON" + Address);
            _RemoteDeviceID = Address;
            if (Dongle_ConnectionEstablished_WaitHandle.WaitOne(20000))
            {
                logger.Info("Connected to {0}", Address);

            }
            else
            {
                logger.Info("Connection to {0} timed out", Address);
            }
        }

        public void TransmitAndLog(string text)
        {
            if (currentlyScanningForDevices)
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                    throw new AccessViolationException("Cant transmit while scanning for devices");
                return;
            }
            serialPort.Write(text);
            dataLoggerTX.Info(text.Replace("\r", "\\r").Replace("\n", "\\n"));
        }

        /// <summary>
        /// Sends the specified text plus '\n'
        /// </summary>
        /// <param name="text"></param>
        public void TransmitAndLogLine(string text)
        {
            TransmitAndLog(text + "\n");
        }

        public void TransmitToRemoteDevice(string command)
        {
            if (!ConnectedToRemoteDevice)
                throw new Exception("Not connected to any nimble processor");
            TransmitAndLog(command);
        }

        private void ProcessData(string[] mostRecentlyRecievedData)
        {
            foreach (string s in mostRecentlyRecievedData)
            {
                //dataLoggerRX.Info(s);
                if (s == OK_CONN_ESTABLISHED)
                {
                    _connectedToRemoteDevice = true;
                    Dongle_ConnectionEstablished_WaitHandle.Set();
                    if (ConnectionEstablished != null)
                        ConnectionEstablished(this, new DataRecievedEventArgs() { RecievedData = s });
                }
                else if (s == OK_CONN_LOST)
                {
                    _connectedToRemoteDevice = false;
                    Dongle_ConnectionLost_WaitHandle.Set();
                    if (ConnectionLost != null)
                        ConnectionLost(this, new DataRecievedEventArgs() { RecievedData = s });
                }
                else if (s == AT_OK)
                {
                    Dongle_ATOK_WaitHandle.Set();
                }
                else if (s.StartsWith(OK_GET))
                {
                    Dongle_OkGet_WaitHandle.Set();
                }
                else if (s.StartsWith(OK_SET))
                {
                    Dongle_OkSet_WaitHandle.Set();
                }
                else if (s.StartsWith(OK_NAME))
                {
                    Dongle_OkName_WaitHandle.Set();
                }
                else if (s == OK_DISCOVERYSTART)
                {
                    lock (knownDevicesLock)
                    {
                        currentlyScanningForDevices = true;
                        logger.Info("discovery started");
                        buildingKnownDevices.Clear();
                    }
                }
                else if (s == OK_DISCOVERYEND)
                {
                    lock (knownDevicesLock)
                    {
                        currentlyScanningForDevices = false;
                        logger.Info("Discovery finished");
                        _knownDevices = buildingKnownDevices.ToArray();
                    }
                }
                else if (s.StartsWith(OK_DEVICEDISCOVERED))
                {
                    lock (knownDevicesLock)
                    {

                        string addr = s.Split(new char[] { ':' })[1];
                        buildingKnownDevices.Add(addr);
                        _knownDevices = buildingKnownDevices.ToArray();
                        logger.Info("device found: {0}", addr);
                        Dongle_DeviceDiscoveryComplete_WaitHandle.Set();
                    }
                }
                else if (s.StartsWith("OK+"))
                {
                    logger.Warn("handling of {0} not implemented", s);
                    //not yet implemented or not needed
                }
                else
                {
                    if (DataReceivedFromRemoteDevice != null)
                        DataReceivedFromRemoteDevice(this, new DataRecievedEventArgs() { RecievedData = s });
                }
            }
        }

        public string[] DiscoverDevices()
        {
            TransmitAndLog("AT+DISC?");
            if (Dongle_DeviceDiscoveryComplete_WaitHandle.WaitOne(10000))
            {
                return KnownDevices;
            }
            else
            {
                logger.Warn("Scan for devices timed out");
                currentlyScanningForDevices = false;
            }
            return null;
        }

        public bool IsDongleOK()
        {
            TransmitAndLog("AT");
            if (Dongle_ATOK_WaitHandle.WaitOne(5000))
            {
                logger.Debug("Dongle is OK!");
                return true;
            }
            else
            {
                logger.Debug("Dongle not OK :(");
                return false;
            }
        }
    }
}
