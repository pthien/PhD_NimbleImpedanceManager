using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NLog;
using System.Text.RegularExpressions;
using NimbleBluetoothImpedanceManager.RN4871Driver.Model;
using NimbleBluetoothImpedanceManager.RN4871Driver.StreamParsers;

namespace NimbleBluetoothImpedanceManager
{
    class BluetoothCommsDriver
    {
        private Logger logger = LogManager.GetLogger("DataLoggerOT"); //other

        private Logger dataLoggerTX = LogManager.GetLogger("DataLoggerTX");
        private Logger dataLoggerRX = LogManager.GetLogger("DataLoggerRX");

        private SerialPort serialPort;
        private DataChunker dataChunker;

        public EventWaitHandle Dongle_DeviceDiscoveryComplete_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionAttempt_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionEstablished_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle Dongle_ConnectionLost_WaitHandle = new AutoResetEvent(false);


        public delegate void ConnectionEstablishedEventHandler(object sender, DataRecievedEventArgs e);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionLostEventHandler(object sender, DataRecievedEventArgs e);
        public event ConnectionLostEventHandler ConnectionLost;

        public delegate void DataReceivedEventHandler(object sender, DataRecievedEventArgs e);
        public event DataReceivedEventHandler DataReceivedFromRemoteDevice;

        public class DataRecievedEventArgs : EventArgs
        {
            public string RecievedData { get; set; }
        }


        public DateTime TimeOfMostRecentlyReceivedChunk { get; private set; }
        private string[] MostRecentlyRecievedData = new string[] { };

        private object knownDevicesLock = new object();
        public string[] KnownDevices
        {
            get
            {
                lock (knownDevicesLock)
                {
                    string[] copy = (string[])Status.KnownDevices.ToArray().Clone();
                    return copy;
                }
            }
        }

        public Status Status = new Status() { DeviceState = DeviceState.Unknown, RemoteDeviceAddress = null, KnownDevices = new List<string>() };

        public bool ConnectedToRemoteDevice { get { return Status.DeviceState == DeviceState.Connected; } }


        /// <summary>
        /// The bluetooth address of the remote device
        /// </summary>
        public string RemoteDeviceAddr => Status.RemoteDeviceAddress;

        private bool currentlyScanningForDevices = false;

        private DataDumpParser dataDumpParser = new DataDumpParser();
        private OKParser okParser = new OKParser();
        private ScanForDevicesParser scanForDevicesParser = new ScanForDevicesParser();

        public BluetoothCommsDriver()
        {
            dataChunker = new DataChunker();
            dataChunker.ChunkReady += dataChunker_ChunkReady;

            dataChunker.ChunkReady += dataDumpParser.ScanChunksForDongleDataDump;
            dataChunker.ChunkReady += okParser.processChunk;
            dataChunker.ChunkReady += scanForDevicesParser.processChunk;

            logger.Info("*BluetoothCommsDriver initialised*");
        }

        /// <summary>
        /// Opens the the specified serial port. 
        /// </summary>
        /// <param name="CommPort"></param>
        /// <param name="BaudRate"></param>
        /// <returns></returns>
        public bool ConnectToDongle(string CommPort, int BaudRate = 115200)
        {
            try
            {
                serialPort = new SerialPort(CommPort, BaudRate);
                serialPort.DataReceived += serialPort_DataReceived;
                serialPort.Open();
                logger.Info("*Connection to dongle on {0} established*", CommPort);
                if (!InitialiseDongle())
                {
                    serialPort.DataReceived -= serialPort_DataReceived;
                    serialPort.Close();
                }
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

        public bool ConnectToRemoteDevice(string address)
        {
            Dongle_ConnectionAttempt_WaitHandle.Set();
            Status.DeviceState = DeviceState.Connecting;
            Status.RemoteDeviceAddress = address;

            ConnectionParser parser = new ConnectionParser();
            dataChunker.ChunkReady += parser.processChunk;

            TransmitAndLog("C,0," + address + "\n");

            bool connectionSuccess = parser.StreamOpenWaitHandle.WaitOne(10000);

            if (connectionSuccess)
            {
                Status.RemoteDeviceAddress = address;
                logger.Info("Connected to {0}", address);
            }
            else
            {
                Status.RemoteDeviceAddress = null;
                Status.DeviceState = DeviceState.Unknown;

                logger.Info("Connection to {0} timed out. Trying to reset dongle", address);
                ReinitialiseDongle();
            }
            return connectionSuccess;
        }

        protected void TransmitAndLog(string text)
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
        protected void TransmitAndLogLine(string text)
        {
            TransmitAndLog(text + "\n");
        }

        public void TransmitToRemoteDevice(string command)
        {
            if (Status.DeviceState != DeviceState.Connected)
            {
                logger.Error("Not connected to any nimble processor. Discarding transmission: {0}", command.EscapeWhiteSpace());
                //throw new Exception("Not connected to any nimble processor");
            }
            TransmitAndLog(command);
        }

        private void ReinitialiseDongle()
        {
            Thread.Sleep(200);
            TransmitAndLog("$$$"); //Transmit escape sequence
            Thread.Sleep(200);
            TransmitAndLog("R,1");
            Thread.Sleep(400);
            TransmitAndLog("$$$");
        }

        private bool InitialiseDongle()
        {
            logger.Info("Initialising dongle");
            TransmitAndLog("$$$"); //Transmit escape sequence
            Thread.Sleep(200);
            TransmitAndLog("X\n");
            Thread.Sleep(100);
            TransmitAndLog("Z\n");
            Thread.Sleep(100);
            TransmitAndLog("K,1\n");

            for (int i = 0; i < 3; i++)
            {
                if (IsDongleOK())
                {
                    DataDump? dataDump = GetDongleData();

                    if (!dataDump.HasValue)
                    {
                        logger.Info("Data dump did not have a value");
                        return false;
                    }

                    if (dataDump.Value.IsConnected)
                    {
                        logger.Info("When initialising dongle, it was already connected");
                        return DisconnectFromRemoteDevice();
                    }

                    Status.DeviceState = DeviceState.Disconnected;
                    return true;
                }
                Thread.Sleep(100);
            }
            return false;
        }

        private DataDump? GetDongleData()
        {
            DataDumpParser scanner = new DataDumpParser();
            dataChunker.ChunkReady += scanner.ScanChunksForDongleDataDump;
            TransmitAndLog("D\n");
            return dataDumpParser.GetDataDump();
        }

        public bool DisconnectFromRemoteDevice()
        {
            TransmitAndLog("$$$");
            Thread.Sleep(200);
            TransmitAndLog("K,1\n");
            bool success = Dongle_ConnectionLost_WaitHandle.WaitOne(DataChunker.Timeout + 500);
            if (success)
            {
                logger.Info("Disconnected from remote device");
            }
            else
            {
                logger.Info("Failed to disconnect from remote device");
            }
            return success;
        }


        public string[] DiscoverDevices()
        {
            scanForDevicesParser.Reset();
            TransmitAndLog("F\n");

            Status.KnownDevices.Clear();
            Status.KnownDevices.AddRange(scanForDevicesParser.ScanForDevices().Select(x => x.BluetoothAddress));
            logger.Info("Scan for devices completed. Found {0} devices", Status.KnownDevices.Count);

            TransmitAndLog("X\n");

            return KnownDevices;
        }

        public bool IsDongleOK()
        {
            TransmitAndLog("X\n");
            bool success = okParser.OKFound(DataChunker.Timeout + 500);

            if (success)
            {
                logger.Debug("Dongle is OK!");

                return true;
            }

            logger.Debug("Dongle not OK :(");
            return false;
        }

        private void ProcessData(string[] mostRecentlyRecievedData)
        {
            foreach (string s in mostRecentlyRecievedData)
            {
                //dataLoggerRX.Info(s);
                if (s.Contains("%DISCONNECT%"))
                {
                    Dongle_ConnectionLost_WaitHandle.Set();
                    logger.Info("Bluetooth disconnected");
                    Status.DeviceState = DeviceState.Disconnected;
                    Status.RemoteDeviceAddress = null;
                    if (ConnectionLost != null)
                        ConnectionLost(this, new DataRecievedEventArgs() { RecievedData = s });

                }
                if (s.Contains("%STREAM_OPEN%"))
                {
                    Dongle_ConnectionEstablished_WaitHandle.Set();
                    Status.DeviceState = DeviceState.Connected;
                    logger.Info("Connection established");
                    if (ConnectionEstablished != null)
                        ConnectionEstablished(this, new DataRecievedEventArgs { RecievedData = s });
                }
                else if (Status.DeviceState == DeviceState.Connected)
                {
                    //     logger.Debug("data received from remote device: {0}", s);
                    if (DataReceivedFromRemoteDevice != null)
                        DataReceivedFromRemoteDevice(this, new DataRecievedEventArgs() { RecievedData = s });
                }
            }
        }



    }
}
