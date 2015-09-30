using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;


namespace NimbleBluetoothImpedanceManager
{
    class NimbleCommsManager : INimbleCommsManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private BluetoothCommsDriver btDongle;

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

        private event BluetoothCommsDriver.ConnectionLostEventHandler DisconnectedFromNimble
        {
            add { btDongle.ConnectionLost += value; }
            remove { btDongle.ConnectionLost -= value; }
        }

        public delegate void ConnectionLostEventHandler(object sender, EventArgs e);

        public EventWaitHandle NimbleCmdRx_GenGUID_WaitHandle = new AutoResetEvent(false);
        public EventWaitHandle NimbleCmdRx_Name_WaitHandle = new AutoResetEvent(false);
        private EventWaitHandle NimbleCmdRx_xmitTelemStart_WaitHandle = new AutoResetEvent(false);
        private EventWaitHandle NimbleCmdRx_xmitTelemFin_WaitHandle = new AutoResetEvent(false);
        private EventWaitHandle NimbleCmdRx_WDTO_WaitHandle = new AutoResetEvent(false);

        public string RemoteDeviceId
        {
            get { return btDongle.RemoteDeviceAddr; }
        }

        private string _NimbleName = "";
        public string NimbleName
        {
            get { return _NimbleName; }
        }

        public NimbleProcessor RemoteNimbleProcessor
        {
            get { return new NimbleProcessor() { Name = NimbleName, BluetoothAddress = RemoteDeviceId }; }
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

        public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
        public event StateChangedEventHandler StateChanged;
        public class StateChangedEventArgs : EventArgs
        {
            public NimbleState NewState { get; set; }
        }

        private object stateLock = new object();
        private NimbleState _State;
        public NimbleState State
        {
            get { return _State; }
            set
            {
                if (value == _State) //do nothing
                    return;
                logger.Info("State changed from {0} to {1}", _State, value);
                _State = value;
                if (StateChanged != null)
                    StateChanged(this, new StateChangedEventArgs() { NewState = _State });
            }
        }

        public NimbleCommsManager()
        {
            btDongle = new BluetoothCommsDriver();
            btDongle.DataReceivedFromRemoteDevice += new BluetoothCommsDriver.DataReceivedEventHandler(btDongle_DataReceived);
            btDongle.ConnectionLost += btDongle_ConnectionLost;
            State = NimbleState.Disconnected;
        }

        void btDongle_ConnectionLost(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            lock (stateLock)
            {
                State = NimbleState.ConnectedToDongle;
            }
        }

        /// <summary>
        /// Connects to a nimble processor. Returns true if bluetooth connection successful and nimble is responding
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool ConnectToNimble(string address)
        {
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToDongle)
                {
                    State = NimbleState.ConnectingToNimble;
                }
                else
                    return false;
            }

            if (btDongle.ConnectToRemoteDevice(address))
            {
                bool namesuccess = false;
                for (int i = 0; i < 3; i++)
                {
                    namesuccess = GetNimbleName();
                    if (!namesuccess)
                    {
                        logger.Warn("Attempt {0} to get name after connecting to nimble {1} failed", i + 1, address);
                        Thread.Sleep(2000);
                    }
                    else
                        break;
                }

                if (namesuccess)
                {
                    if (NimbleCmdRx_WDTO_WaitHandle.WaitOne(100))
                    {
                        logger.Warn("Not connecting to remote device due to watchdog timeout.");
                        State = NimbleState.ConnectedToDongle;
                        return false;
                    }
                    else
                    {
                        logger.Info("Successfully connected to {0}", address);
                        State = NimbleState.ConnectedToNimbleAndReady;
                        return true;
                    }
                }
                else
                {
                    logger.Error("Connected to remote device ({0}) but could not get its name. "
                        + "Will now attempt to disconnect", address);
                    State = NimbleState.ConnectedToNimbleAndError;
                    DisconnectFromNimble();
                    return false;
                }
            }
            logger.Info("Failed to connect to {0}", address);
            State = NimbleState.ConnectedToDongle;
            return false;
        }

        void btDongle_DataReceived(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            logger.Debug("Received nimble command response: {0}", e.RecievedData);
            ParseCommandResponse(e.RecievedData);
        }

        static Regex regex_ID = new Regex(@"{([A-Za-z0-9]+):([ A-Z0-9a-z-:_|()]+)}");
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
            if (receivingTelemData)
            {
                lock (receivingTelemDataLock)
                {
                    telemData_temp.Add(p);
                }
            }
        }

        private void ProcessData(string command, string data)
        {
            switch (command)
            {
                case "ID":
                    _NimbleName = data;
                    logger.Info("Nimble name set to {0}, btAddr={1}", NimbleName, btDongle.RemoteDeviceAddr);
                    NimbleCmdRx_Name_WaitHandle.Set();
                    break;
                case "GenGUID":
                    remoteDeviceGenGUID = data;
                    logger.Info("Got GenGUID {0} from {1}", data, btDongle.RemoteDeviceAddr);
                    NimbleCmdRx_GenGUID_WaitHandle.Set();
                    break;
                case "xmitTelem":
                    lock (receivingTelemDataLock)
                    {
                        if (data == "fin")
                        {
                            receivingTelemData = false;
                            logger.Info("Recieving telem data finished, btAddr={0}", btDongle.RemoteDeviceAddr);
                            _telemData = telemData_temp.ToArray();
                            NimbleCmdRx_xmitTelemFin_WaitHandle.Set();
                        }
                        if (data == "set")
                        {
                            receivingTelemData = true;
                            logger.Info("Recieving telem data started, btAddr={0}", btDongle.RemoteDeviceAddr);
                            telemData_temp = new List<string>();
                            _telemData = null;
                            NimbleCmdRx_xmitTelemStart_WaitHandle.Set();
                        }
                        if (data == "discon")
                        {
                            receivingTelemData = false;
                            telemData_temp = new List<string>();
                            _telemData = telemData_temp.ToArray(); 
                            logger.Info("Recieving telem failed because a coil is disconnected, {0}", RemoteNimbleProcessor);
                            NimbleCmdRx_xmitTelemFin_WaitHandle.Set();
                        }

                    }
                    break;
                case "BothConnected":
                    if(data=="n")
                        logger.Error("Nimble processor {0} has 1 or more coils detached", RemoteNimbleProcessor);
                    if (data == "y")
                        logger.Info("Nimble processor {0} has all coils attached", RemoteNimbleProcessor);
                    break;
                case "WDTO":
                    logger.Fatal("Watchdog timer expired on {0}. Disconnect processor from subject and do not reattach, potential for unexpected stimulation. Stimulation has been halted.", RemoteNimbleProcessor);
                    NimbleCmdRx_WDTO_WaitHandle.Set();
                    break;

            }
            //throw new NotImplementedException();
        }

        public bool Initialise(string COMPort)
        {
            State = NimbleState.ConnectingToDongle;
            logger.Info("Initialising...");
            comport = COMPort;
            bool connected = btDongle.ConnectToDongle(COMPort);
            if (!connected)
            {
                logger.Warn("Initialisation failed, could not connect to dongle");
                State = NimbleState.Disconnected;
                return false;
            }
            if (!btDongle.IsDongleOK())
            {

                if (btDongle.Dongle_ConnectionLost_WaitHandle.WaitOne(1))
                {
                    logger.Warn("Dongle already connected to slave. Reattempting OK Check");
                    if (!btDongle.IsDongleOK())
                    {
                        State = NimbleState.Disconnected;
                        return false;
                    }
                }
                else
                {
                    State = NimbleState.Disconnected;
                    logger.Warn("Initialisation failed, dongle not ok");
                    return false;
                }
            }

          

            logger.Info("Initialisation successful");
            State = NimbleState.ConnectedToDongle;
            initialised = true;
            return true;
        }

        public string[] DiscoverDevices()
        {
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToDongle)
                {
                    State = NimbleState.ConnectedToDongleAndBusy;
                }
                else
                    return null;
            }
            var result = btDongle.DiscoverDevices();
            State = NimbleState.ConnectedToDongle;
            return result;
        }


        //TODO: make this thread safe, add a lock around accessing receivngTelemData
        public bool CollectTelemetryData(int sequence, out string[] data)
        {
            data = null;
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndReady)
                {
                    State = NimbleState.ConnectedToNimbleAndWorking;
                }
                else
                    return false;
            }


            if (receivingTelemData)
            {
                logger.Error("Called collectTelemData when state was {0}", State);
                //State = NimbleState.ConnectingToNimble;
                return false;
           } 
            //receivingTelemData = true;
            string command = string.Format("\nsetXmitTelem {0}\n", sequence);
            NimbleCmdRx_xmitTelemFin_WaitHandle.Reset();
            lock (receivingTelemDataLock)
            {
                telemData_temp = new List<string>();
                _telemData = null;
            }
            btDongle.TransmitToRemoteDevice(command);
            if (NimbleCmdRx_xmitTelemFin_WaitHandle.WaitOne(DataChunker.Timeout + 10000))
            {
                logger.Info("Receive Telem data successful. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
                lock (stateLock)
                {
                    if (State == NimbleState.ConnectedToNimbleAndWorking)
                        State = NimbleState.ConnectedToNimbleAndReady;
                }
                data = (string[])TelemetryData.Clone();
                return true;
            }
            else
            {
                logger.Info("Receive Telem timed out. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
                ProcessData("xmitTelem", "fin");
                //receivingTelemData = false;
                lock (stateLock)
                {
                    if (State == NimbleState.ConnectedToNimbleAndWorking)
                        State = NimbleState.ConnectedToNimbleAndReady;
                }
                data = (string[])TelemetryData.Clone();
                return false;
            }
        }

        private bool GetNimbleName()
        {
            NimbleState entryState;
            lock (stateLock)
            {
                if (State == NimbleState.ConnectingToNimble || State == NimbleState.ConnectedToNimbleAndReady)
                {
                    entryState = State;
                    State = NimbleState.ConnectedToNimbleAndWorking;
                }
                else
                    return false;
            }

            try
            {
                NimbleCmdRx_Name_WaitHandle.Reset();
                btDongle.TransmitToRemoteDevice("\nGetID\n");
                if (NimbleCmdRx_Name_WaitHandle.WaitOne(DataChunker.Timeout + 1000))
                {
                    State = NimbleState.ConnectedToNimbleAndReady;
                    return true;
                }
                else
                {
                    State = entryState;
                    return false;
                }
            }
            catch (Exception ex)
            {
                State = entryState;
                logger.Error(ex.Message);
                //throw ex;
                return false;
            }

            //State = NimbleState.ConnectedToNimbleAndReady;
            //return true;
        }

        public string GetSequenceGUID()
        {
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndReady)
                {
                    State = NimbleState.ConnectedToNimbleAndWorking;
                }
                else
                    return "";
            }

            NimbleCmdRx_GenGUID_WaitHandle.Reset();
            btDongle.TransmitToRemoteDevice("\ngetGenGUID\n");
            if (NimbleCmdRx_GenGUID_WaitHandle.WaitOne(DataChunker.Timeout + 2000))
            {
                State = NimbleState.ConnectedToNimbleAndReady;
                return remoteDeviceGenGUID;
            }
            State = NimbleState.ConnectedToNimbleAndReady;
            return "";
        }

        //public void StartTelemCapture()
        //{
        //    btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
        //}

        //public void EndTelemCapture()
        //{
        //    btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
        //}

        public bool DisconnectFromNimble()
        {
            logger.Info("Disconnecting from nimble {0}", RemoteNimbleProcessor);
            if (btDongle.ConnectedToRemoteDevice)
            {
                //btDongle.TransmitToRemoteDevice("\nclearXmitTelem\n");
                //Thread.Sleep(500);
                //btDongle.TransmitAndLog("endSession\n");
                for (int i = 0; i < 10; i++)
                {
                    btDongle.Dongle_ConnectionLost_WaitHandle.Reset();
                    btDongle.TransmitAndLog("AT");
                    if (btDongle.Dongle_ConnectionLost_WaitHandle.WaitOne(DataChunker.Timeout + 1000))
                    {
                        logger.Info("Disconnected from Nimble processor on attempt {0}", i);
                        break;
                    }
                    else
                        logger.Info("Failed to disconnect on attempt {0}", i);
                }
            }
            bool res = false;
            logger.Info("Ensuring dongle is ok after disconnecting:");
            for (int i = 0; i < 100; i++)
            {
                logger.Info("Ensuring dongle is ok after disconnecting:");
                res = btDongle.IsDongleOK();
                if (!res)
                {
                    logger.Warn("After disconnecting logger not ok after {1} request(s)", i);
                    State = NimbleState.ConnectedToNimbleAndError;
                }
                else
                {
                    logger.Info("Disconnection completed successfuly", i);
                    State= NimbleState.ConnectedToDongle;
                    break;
                }
            }
            return res;

        }
    }

    internal enum NimbleState
    {
        Disconnected = 1,
        ConnectingToDongle = 2,
        ConnectedToDongle = 4,
        ConnectedToDongleAndBusy = 8,
        ConnectingToNimble = 16,
        ConnectedToNimbleAndReady = 32,
        ConnectedToNimbleAndWorking = 64,
        ConnectedToNimbleAndError = 128,
    }
}
