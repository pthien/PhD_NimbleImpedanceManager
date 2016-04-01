using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using Nimble.Sequences;

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

        private EventWaitHandle NimbleCmdRx_dataReceived_WaitHandle = new AutoResetEvent(false);

        public string RemoteDeviceId
        {
            get { return btDongle.RemoteDeviceAddr; }
        }

        public string NimbleName
        {
            get
            {
                if (RemoteNimbleProcessor == null)
                {
                    logger.Fatal("Someone asked for the nimble name, but we arent connected");
                    return "ERROR!!!! (not connected)";
                }
                return RemoteNimbleProcessor.Name;
            }
        }

        public NimbleProcessor RemoteNimbleProcessor { get; private set; }

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
                    if (_telemData == null)
                    {
                        logger.Error("Hey, there's a bug! Don't trust this data. Restart Me!");
                        _telemData = new string[0];
                    }
                    string[] copy = (string[])_telemData.Clone();
                    return copy;
                }
            }
        }

        Queue<string> receivedData = new Queue<string>();

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

        void btDongle_DataReceived(object sender, BluetoothCommsDriver.DataRecievedEventArgs e)
        {
            logger.Debug("Received nimble command response: {0}", e.RecievedData);
            ParseCommandResponse(e.RecievedData);
            receivedData.Enqueue(e.RecievedData);
            NimbleCmdRx_dataReceived_WaitHandle.Set();
        }





        static Regex regex_ID = new Regex(@"{([A-Za-z0-9]+):([ A-Z0-9a-z-:_|()]+)}");

        /// <summary>
        /// Depreciated.
        /// </summary>
        /// <param name="p"></param>
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

        /// <summary>
        /// Parses a command response and returns the command and any associated data
        /// </summary>
        /// <param name="p"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        private bool ParseCommandResponse(string receivedResponse, out string command, out string data)
        {
            if (regex_ID.IsMatch(receivedResponse))
            {
                var matches = regex_ID.Match(receivedResponse);
                if (matches.Success)
                {
                    string _command = matches.Groups[1].Value;
                    string _data = matches.Groups[2].Value;
                    //_RemoteDeviceID = command;
                    //ProcessData(command, data);
                    command = _command;
                    data = _data;
                    return true;
                }
            }
            command = null;
            data = null;
            return false;
        }

        private void ProcessData(string command, string data)
        {
            switch (command)
            {
                //case "ID":
                //    _NimbleName = data;
                //    logger.Info("Nimble name set to {0}, btAddr={1}", NimbleName, btDongle.RemoteDeviceAddr);
                //    NimbleCmdRx_Name_WaitHandle.Set();
                //    break;
                //case "GenGUID":
                //    remoteDeviceGenGUID = data;
                //    logger.Info("Got GenGUID {0} from {1}", data, btDongle.RemoteDeviceAddr);
                //    NimbleCmdRx_GenGUID_WaitHandle.Set();
                //    break;
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
                //case "BothConnected":
                //    if (data == "n")
                //        logger.Error("Nimble processor {0} has 1 or more coils detached", RemoteNimbleProcessor);
                //    if (data == "y")
                //        logger.Info("Nimble processor {0} has all coils attached", RemoteNimbleProcessor);
                //    break;
                //case "WDTO":
                //    logger.Fatal("Watchdog timer expired on {0}. Disconnect processor from subject and do not reattach, potential for unexpected stimulation. Stimulation has been halted.", RemoteNimbleProcessor);
                //    NimbleCmdRx_WDTO_WaitHandle.Set();
                //    break;

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
            RemoteNimbleProcessor = null; //clear any previously connected processor
            if (btDongle.ConnectToRemoteDevice(address))
            {
                //Get name of the nimble we are connecting to
                bool namesuccess = false;
                string nimbleName = "";
                for (int i = 0; i < 3; i++)
                {
                    namesuccess = GetNimbleName(out nimbleName);
                    if (!namesuccess)
                    {
                        logger.Warn("Attempt {0} to get name after connecting to nimble {1} failed", i + 1, address);
                        Thread.Sleep(2000);
                    }
                    else
                        break;
                }

                //Get watchdog status
                bool watchdogSuccess = false;
                bool watchdogOK = false;
                for (int i = 0; i < 3; i++)
                {
                    watchdogSuccess = GetNimbleWatchdogStatus(out watchdogOK);
                    if (!watchdogSuccess)
                    {
                        logger.Warn("Attempt {0} to get WDTO after connecting to nimble {1} failed", i + 1, address);
                        Thread.Sleep(2000);
                    }
                    else
                        break;
                }

                //Get coil status status
                bool coilSuccess = false;
                bool coilsOK = false; 
                for (int i = 0; i < 3; i++)
                {
                    coilSuccess = GetNimbleCoilStatus(out coilsOK);
                    if (!coilSuccess)
                    {
                        logger.Warn("Attempt {0} to get coil status after connecting to nimble {1} failed", i + 1, address);
                        Thread.Sleep(2000);
                    }
                    else
                        break;
                }

                //get sequence guid
                string genGUID = "";
                bool genGUIDSuccess = false;
                for (int i = 0; i < 3; i++)
                {
                    genGUIDSuccess = GetGenGUID(out genGUID);
                    if (!genGUIDSuccess)
                    {
                        logger.Warn("Attempt {0} to get gen guid after connecting to nimble {1} failed", i + 1, address);
                        Thread.Sleep(2000);
                    }
                    else
                        break;
                }

                //check if watchdog timer has tripped
                if (!watchdogOK)
                {
                    logger.Fatal("Watchdog timer expired on {0}. Disconnect processor from subject and do not reattach, potential for unexpected stimulation. Stimulation has been halted.", RemoteNimbleProcessor);
                    State = NimbleState.ConnectedToNimbleAndError;
                    DisconnectFromNimble();
                    return false;
                }

                //make sure we got a name
                if (!namesuccess)
                {
                    logger.Error("Connected to remote device ({0}) but could not get its name. "
                       + "Will now attempt to disconnect", address);
                    State = NimbleState.ConnectedToNimbleAndError;
                    DisconnectFromNimble();
                    return false;
                }

                //ensure coils are connected
                if (!coilsOK)
                {
                    logger.Error("Connected to {1} ({0}) but one or more coils were disconnected. "
                      + "Will now attempt to disconnect", address, nimbleName);
                    State = NimbleState.ConnectedToNimbleAndError;
                    DisconnectFromNimble();
                    return false;
                }

                if (!genGUIDSuccess)
                {
                    logger.Error("Connected to {1} ({0}) but one or could not get sequence guid. "
                      + "Will now attempt to disconnect", address, nimbleName);
                    State = NimbleState.ConnectedToNimbleAndError;
                    DisconnectFromNimble();
                    return false;
                }

                //if everything is ok, finalise the connection
                if (namesuccess && coilsOK && watchdogOK && genGUIDSuccess)
                {
                    logger.Info("Successfully connected to {1} ({0})", address, nimbleName);
                    RemoteNimbleProcessor = new NimbleProcessor()
                    {
                        GenGUID = genGUID,
                        BluetoothAddress = btDongle.RemoteDeviceAddr,
                        Name = nimbleName
                    };
                    State = NimbleState.ConnectedToNimbleAndReady;
                    return true;
                }

                logger.Error("Connected to ({0}) but something went horribly wrong", address);
                State = NimbleState.ConnectedToNimbleAndError;
                DisconnectFromNimble();
                return false;
            }

            logger.Info("Failed to connect to {0}", address);
            State = NimbleState.ConnectedToDongle;
            return false;
        }

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

            DateTime collectionStartedTime = DateTime.Now;  //ensure a minimum timeout if the 
            btDongle.TransmitToRemoteDevice(command);

            while ((DateTime.Now - btDongle.TimeOfMostRecentlyReceivedChunk).TotalSeconds < 5 || (DateTime.Now - collectionStartedTime).TotalSeconds < 5)
            {
                if (NimbleCmdRx_xmitTelemFin_WaitHandle.WaitOne(100))
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
            }

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

            //if (NimbleCmdRx_xmitTelemFin_WaitHandle.WaitOne(DataChunker.Timeout + 10000))
            //{
            //    logger.Info("Receive Telem data successful. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
            //    lock (stateLock)
            //    {
            //        if (State == NimbleState.ConnectedToNimbleAndWorking)
            //            State = NimbleState.ConnectedToNimbleAndReady;
            //    }
            //    data = (string[])TelemetryData.Clone();
            //    return true;
            //}
            //else
            //{
            //    logger.Info("Receive Telem timed out. Sequence {0}, Device {1}", sequence, RemoteDeviceId);
            //    ProcessData("xmitTelem", "fin");
            //    //receivingTelemData = false;
            //    lock (stateLock)
            //    {
            //        if (State == NimbleState.ConnectedToNimbleAndWorking)
            //            State = NimbleState.ConnectedToNimbleAndReady;
            //    }
            //    data = (string[])TelemetryData.Clone();
            //    return false;
            //}
        }

        //private bool GetNimbleName()
        //{
        //    NimbleState entryState;
        //    lock (stateLock)
        //    {
        //        if (State == NimbleState.ConnectingToNimble || State == NimbleState.ConnectedToNimbleAndReady)
        //        {
        //            entryState = State;
        //            State = NimbleState.ConnectedToNimbleAndWorking;
        //        }
        //        else
        //            return false;
        //    }

        //    try
        //    {
        //        NimbleCmdRx_Name_WaitHandle.Reset();
        //        btDongle.TransmitToRemoteDevice("\nGetID\n");
        //        if (NimbleCmdRx_Name_WaitHandle.WaitOne(DataChunker.Timeout + 1000))
        //        {
        //            State = NimbleState.ConnectedToNimbleAndReady;
        //            return true;
        //        }
        //        else
        //        {
        //            State = entryState;
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        State = entryState;
        //        logger.Error(ex.Message);
        //        //throw ex;
        //        return false;
        //    }

        //    //State = NimbleState.ConnectedToNimbleAndReady;
        //    //return true;
        //}

        private bool GetNimbleName(out string Name)
        {
            string response;
            if (NimbleGet(out response, "ID", null))
            {
                Name = response;
                logger.Info("Nimble name set to {0}, btAddr={1}", Name, btDongle.RemoteDeviceAddr);
                return true;
            }
            Name = null;
            return false;
        }

        private bool GetGenGUID(out string GenGUID)
        {
            string response;
            if (NimbleGet(out response, "GenGUID", null))
            {
                GenGUID = response;
                remoteDeviceGenGUID = response;
                logger.Info("Got GenGUID {0} from {1}", response, btDongle.RemoteDeviceAddr);
                return true;
            }
            GenGUID = null;
            return false;
        }

        /// <summary>
        /// ProcessorWorkingCorrectly is false if the watchdog timer has tripped.
        /// </summary>
        /// <param name="ProcessorWorkingCorrectly"></param>
        /// <returns></returns>
        private bool GetNimbleWatchdogStatus(out bool ProcessorWorkingCorrectly)
        {
            string response;
            if (NimbleGet(out response, "WDTO", null))
            {
                if (response == "y")
                    ProcessorWorkingCorrectly = false;
                else if (response == "n")
                    ProcessorWorkingCorrectly = true;
                else
                {
                    logger.Info("Got bad WDTO response: {0} from {1}", response, btDongle.RemoteDeviceAddr);
                    ProcessorWorkingCorrectly = false;
                    return false;
                }
                logger.Info("Got WDTO '{0}' from {1}", response, btDongle.RemoteDeviceAddr);
                return true;
            }
            ProcessorWorkingCorrectly = false;
            return false;
        }

        private bool GetNimbleCoilStatus(out bool BothCoilsConnected)
        {
            string response;
            if (NimbleGet(out response, "BothConnected", null))
            {
                if (response == "y")
                    BothCoilsConnected = true;
                else if (response == "n")
                    BothCoilsConnected = false;
                else
                {
                    logger.Info("Got bad coil status response: {0} from {1}", response, btDongle.RemoteDeviceAddr);
                    BothCoilsConnected = true;
                    return false;
                }
                logger.Info("Got coil status '{0}' from {1}", response, btDongle.RemoteDeviceAddr);
                return true;
            }
            BothCoilsConnected = false;
            return false;
        }

        public bool IsStimOn(out bool StimOn)
        {
            StimOn = false;
            string response;
            if (NimbleGet(out response, "StimOn", null))
            {
                logger.Info("Got StimOn response {0} from {1}", response, btDongle.RemoteDeviceAddr);
                switch (response)
                {
                    case "on": StimOn = true; break;
                    case "off": StimOn = false; break;
                    default: StimOn = false;
                        logger.Error("invalid StimOn response {0} from {1}", response, btDongle.RemoteDeviceAddr);
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the segment the processor is currently presenting.
        /// </summary>
        /// <param name="CurrentSegment"></param>
        /// <returns></returns>
        private bool GetCurrentSegment(out int CurrentSegment)
        {
            string response;
            if (NimbleGet(out response, "CurrSeg", null))
            {
                logger.Info("Got CurrSeg response {0} from {1}", response, btDongle.RemoteDeviceAddr);
                bool res = int.TryParse(response, out CurrentSegment);
                return res;
            }
            CurrentSegment = 0;
            return false;
        }

        /// <summary>
        /// Gets the start and end loop segments
        /// </summary>
        /// <param name="StartLoopSegment"></param>
        /// <param name="EndLoopSegment"></param>
        /// <returns></returns>
        private bool GetLoopSegments(out int StartLoopSegment, out int EndLoopSegment)
        {
            StartLoopSegment = 0;
            EndLoopSegment = 0;
            string response;
            if (NimbleGet(out response, "Level", null))
            {
                logger.Info("Got level response {0} from {1}", response, btDongle.RemoteDeviceAddr);

                string[] parts = response.Split(',');

                if (parts.Length == 2)
                {
                    bool res1 = int.TryParse(parts[0], out StartLoopSegment);
                    bool res2 = int.TryParse(parts[1], out EndLoopSegment);
                    return res1 & res2;
                }
                return false;
            }
            return false;
        }

        private bool SetLoopSegments(int StartLoopSegment, int EndLoopSegment)
        {
            string response;
            bool res = NimbleSet(out response, "Level",
                new string[3] {
                    StartLoopSegment.ToString(), 
                    EndLoopSegment.ToString(),
                    ((StartLoopSegment+EndLoopSegment)%256).ToString() }
                    );
            return false;
        }

        /// <summary>
        /// Gets the current level of the ramp. (i.e. converts current segment to a level)
        /// </summary>
        /// <returns></returns>
        public int GetRampProgress()
        {
            int currentSeg;
            bool res = GetCurrentSegment(out currentSeg);

            return 0;
        }

        public int GetRampLevel()
        {
            return 0;
        }

        public bool SetStimActivity(bool stimOn) { return true; }


        public bool SetRampLevel(int RampLevel) { return true; }


        private bool NimbleGet(out string[] response, string ParamToGet, string[] args)
        {
            string[] responses;
            bool result = NimbleTransact(out responses, TransactionTypes.Get, ParamToGet, args);
            response = responses;// = responses.Length > 0 ? responses[0] : "";
            return result;
        }

        /// <summary>
        /// Get a single response from a nimble processor. 
        /// </summary>
        /// <param name="responseData">The important information in the response</param>
        /// <param name="ParamToGet"></param>
        /// <param name="args"></param>
        /// <returns>True if successful. False if no response, the response could not be parsed, or if it did not match the command that was sent</returns>
        private bool NimbleGet(out string responseData, string ParamToGet, string[] args)
        {
            return NimbleTransactInterface(TransactionTypes.Get,
                out responseData, ParamToGet, args);
        }

        private bool NimbleSet(out string responseData, string ParamToSet, string[] args)
        {
            return NimbleTransactInterface(TransactionTypes.Set,
                out responseData, ParamToSet, args);
        }

        private bool NimbleTransactInterface(TransactionTypes type, out string responseData, string ParamToGet, string[] args)
        {
            string response;
            bool result = NimbleTransact(out response, type, ParamToGet, args);

            string command, command_data;
            if (ParseCommandResponse(response, out command, out command_data))
            {
                if (command == ParamToGet)
                {
                    responseData = command_data;
                    return true;
                }
            }
            responseData = "";
            return false;
        }

        private bool NimbleTransact(out string response, TransactionTypes TransactionType, string Method, string[] args)
        {
            var start = DateTime.Now;
            response = string.Empty;
            List<string> responses = new List<string>();
            NimbleState entryState;
            args = args == null ? new string[0] : args; //make sure args is not null.
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndReady)
                {
                    entryState = State;
                    State = NimbleState.ConnectedToNimbleAndWorking;
                }
                else if (State == NimbleState.ConnectingToNimble)
                {
                    entryState = State;
                }
                else
                    return false;
            }

            try
            {
                string transactionType = TransactionType2String(TransactionType);

                string command = string.Format("\n{0}{1} {2}\n", transactionType, Method, string.Join(" ", args));


                string expectedResponseStart;
                if (TransactionType == TransactionTypes.Get)
                    expectedResponseStart = string.Format("{{{1}:", transactionType, Method);
                else
                    expectedResponseStart = string.Format("{{{0}{1}:", transactionType, Method);

                receivedData.Clear();
                NimbleCmdRx_dataReceived_WaitHandle.Reset();

                btDongle.TransmitToRemoteDevice(command);

                DateTime requestStart = DateTime.Now;
                while (DateTime.Now < requestStart + TimeSpan.FromMilliseconds(DataChunker.Timeout + 1000))
                {
                    NimbleCmdRx_dataReceived_WaitHandle.WaitOne(100);
                    while (receivedData.Count > 0)
                    {
                        string line = receivedData.Dequeue();
                        responses.Add(line);
                        if (line.StartsWith(expectedResponseStart))
                        {
                            response = line;
                            State = entryState;
                            logger.Debug(string.Format("Transact single took {0}ms and returned true.", (DateTime.Now - start).TotalMilliseconds));
                            return true;
                        }
                    }
                }

                State = entryState;
                logger.Debug(string.Format("Transact single took {0}ms and returned false.", (DateTime.Now - start).TotalMilliseconds));
                return false;
            }
            catch (Exception ex)
            {
                State = entryState;
                logger.Error(ex.Message);
                logger.Debug(string.Format("Transact single took {0}ms and returned false.", (DateTime.Now - start).TotalMilliseconds));
                return false;
            }

        }

        private bool NimbleTransact(out string[] response, TransactionTypes TransactionType, string Method, string[] args)
        {
            var start = DateTime.Now;
            response = new string[0];
            List<string> responses = new List<string>();
            NimbleState entryState;
            args = args == null ? new string[0] : args; //make sure args is not null.
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
                string transactionType = TransactionType2String(TransactionType);

                string command = string.Format("\n{0}{1} {2}\n", transactionType, Method, string.Join(" ", args));
                string expectedResponseStart = string.Format("{{{0}{1}: ", transactionType, Method);

                receivedData.Clear();
                NimbleCmdRx_dataReceived_WaitHandle.Reset();

                btDongle.TransmitToRemoteDevice(command);

                DateTime requestStart = DateTime.Now;
                while (DateTime.Now < requestStart + TimeSpan.FromMilliseconds(DataChunker.Timeout + 1000))
                {
                    NimbleCmdRx_dataReceived_WaitHandle.WaitOne(100);
                    while (receivedData.Count > 0)
                    {
                        string line = receivedData.Dequeue();
                        responses.Add(line);
                    }
                }

                if (responses.Count > 0)
                {
                    response = responses.ToArray();
                    State = entryState;
                    logger.Debug(string.Format("Transact multi took {0}s and returned true.", (DateTime.Now - start).TotalMilliseconds));
                    return true;
                }

                State = entryState;
                logger.Debug(string.Format("Transact multi took {0}s and returned false.", (DateTime.Now - start).TotalMilliseconds));
                return false;
            }
            catch (Exception ex)
            {
                State = entryState;
                logger.Error(ex.Message);
                logger.Debug(string.Format("Transact multi took {0}s and returned false.", (DateTime.Now - start).TotalMilliseconds));
                return false;
            }
        }



        public int GetMaxRampLevel(SequenceFileManager sfm)
        {
            if (sfm.CompiledSequences.ContainsKey(RemoteNimbleProcessor.GenGUID))
            {
                var compiledSequence = sfm.CompiledSequences[RemoteNimbleProcessor.GenGUID];

            }
            return 0;
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
            RemoteNimbleProcessor = null;
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
                    State = NimbleState.ConnectedToDongle;
                    break;
                }
            }
            return res;

        }

        private enum TransactionTypes
        {
            Get,
            Set,
            Do
        }

        private static string TransactionType2String(TransactionTypes TransactionType)
        {
            string transactionType;
            switch (TransactionType)
            {
                case TransactionTypes.Get:
                    transactionType = "Get";
                    break;
                case TransactionTypes.Set:
                    transactionType = "Set";
                    break;
                case TransactionTypes.Do:
                    transactionType = "Do";
                    break;
                default:
                    transactionType = "";
                    break;
            }
            return transactionType;
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
