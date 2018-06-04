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
            Status.RemoteDeviceAddress = "UART";// = new Status() {DeviceState = DeviceState.Connected, RemoteDeviceAddress = "UART"};
            return true;
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
            foreach (char c in text)
            {
                serialPort.Write(c.ToString());
                Thread.Sleep(2);
            }
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
            TransmitAndLog(command);
        }

        private void ReinitialiseDongle()
        {
        }

        private bool InitialiseDongle()
        {
            return true;
        }

        public bool DisconnectFromRemoteDevice()
        {
            return true;
        }


        public string[] DiscoverDevices()
        {
            return new[] { "Directly Connected Device" };
        }

        public bool IsDongleOK()
        {
            logger.Debug("Dongle is OK!");

            return true;

        }

        private void ProcessData(string[] mostRecentlyRecievedData)
        {
            foreach (string s in mostRecentlyRecievedData)
            {
                if (DataReceivedFromRemoteDevice != null)
                    DataReceivedFromRemoteDevice(this, new DataRecievedEventArgs() { RecievedData = s });

            }
        }



    }
}
