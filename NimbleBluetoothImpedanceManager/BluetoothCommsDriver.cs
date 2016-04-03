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


        public DateTime TimeOfMostRecentlyReceivedChunk { get; private set; }
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

        private string _RemoteDeviceAddr = "";

        /// <summary>
        /// The bluetooth address of the remote device
        /// </summary>
        public string RemoteDeviceAddr
        {
            get { return _RemoteDeviceAddr; }
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
            //logger.Debug("Got a chunk {0}", e.Chunk.Replace("\n", "\\n").Replace("\r", "\\r"));
            dataLoggerRX.Info("[{0}] {1}", e.Reason.ToString(), e.Chunk.EscapeWhiteSpace());
            MostRecentlyRecievedData = ChunkParser.ParseChunk(e.Chunk).ToArray();
            TimeOfMostRecentlyReceivedChunk = DateTime.Now;
            ProcessData(MostRecentlyRecievedData);
        }

        public bool ConnectToRemoteDevice(string Address)
        {
            Dongle_ConnectionEstablished_WaitHandle.Reset();
            TransmitAndLog("AT+CON" + Address);
            _RemoteDeviceAddr = Address;
            if (Dongle_ConnectionEstablished_WaitHandle.WaitOne(DataChunker.Timeout + 20000))
            {
                logger.Info("Connected to {0}", Address);
                return true;
            }
            else
            {
                _RemoteDeviceAddr = "";
                logger.Info("Connection to {0} timed out", Address);
                return false;
            }
        }

        public void TransmitAndLog(string text)
        {
            if (currentlyScanningForDevices)
            {
                logger.Error("Cant transmit while scanning for devices. Discarding transmission: {0}", text.EscapeWhiteSpace());
                if (!System.Diagnostics.Debugger.IsAttached)
                    throw new AccessViolationException("Cant transmit while scanning for devices");
                return;
            }
            serialPort.Write(text);
            dataLoggerTX.Info(text.EscapeWhiteSpace());
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
            {
                logger.Error("Not connected to any nimble processor. Discarding transmission: {0}", command.EscapeWhiteSpace());
                //throw new Exception("Not connected to any nimble processor");
            }
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
                        logger.Info("Discovery started");
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

                        Dongle_DeviceDiscoveryComplete_WaitHandle.Set();
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
                    }
                }
                else if (s.StartsWith("OK+"))
                {
                    logger.Debug("handling of {0} not implemented", s);
                    //not yet implemented or not needed
                }
                else
                {
                    logger.Debug("data received from remote device: {0}", s);
                    if (DataReceivedFromRemoteDevice != null)
                        DataReceivedFromRemoteDevice(this, new DataRecievedEventArgs() { RecievedData = s });
                }
            }
        }

        public string[] DiscoverDevices()
        {
            Dongle_DeviceDiscoveryComplete_WaitHandle.Reset();
            TransmitAndLog("AT+DISC?");
            if (Dongle_DeviceDiscoveryComplete_WaitHandle.WaitOne(DataChunker.Timeout + 10000))
            {
                logger.Info("Scan for devices completed");
                return KnownDevices;
            }
            else
            {
                lock (knownDevicesLock)
                {
                    logger.Warn("Scan for devices timed out");
                    currentlyScanningForDevices = false;
                    _knownDevices = buildingKnownDevices.ToArray();
                }
            }
            return null;
        }

        public bool IsDongleOK()
        {
            Dongle_ATOK_WaitHandle.Reset();
            TransmitAndLog("AT");
            if (Dongle_ATOK_WaitHandle.WaitOne(DataChunker.Timeout + 500))
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
