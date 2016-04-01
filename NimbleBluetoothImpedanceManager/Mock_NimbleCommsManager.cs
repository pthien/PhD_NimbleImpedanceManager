using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using Nimble.Sequences;

namespace NimbleBluetoothImpedanceManager
{
    class Mock_NimbleCommsManager : INimbleCommsManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string Comport
        {
            get { return "COM4"; }
        }

        public bool Initialised { get; private set; }
        public bool ConnectedToRemoteDevice { get; private set; }
        public string RemoteDeviceId { get { return "84EB1877AF2A"; } }
        public string NimbleName { get; private set; }
        public NimbleProcessor RemoteNimbleProcessor { get; private set; }

        public event NimbleCommsManager.StateChangedEventHandler StateChanged;
        public delegate void StateChangedEventHandler(object sender, NimbleCommsManager.StateChangedEventArgs e);
        //protected virtual void OnStateChanged(NimbleCommsManager.StateChangedEventArgs e)
        //{
        //    if (StateChanged != null)
        //        StateChanged(this, e);
        //}

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
                    StateChanged(this, new NimbleCommsManager.StateChangedEventArgs() { NewState = _State });
            }
        }

        public bool ConnectToNimble(string address)
        {
            //throw new NotImplementedException();
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToDongle)
                {
                    State = NimbleState.ConnectingToNimble;
                }
                else
                    return false;
            }
            ConnectedToRemoteDevice = true;
            NimbleName = "MOCK_DEVICE";
            State = NimbleState.ConnectedToNimbleAndReady;
            return true;
        }

        public bool Initialise(string COMPort)
        {
            State = NimbleState.ConnectingToDongle;
            Thread.Sleep(500);
            State = NimbleState.ConnectedToDongle;
            Initialised = true;
            return true;
        }

        public string[] DiscoverDevices()
        {
            State = NimbleState.ConnectedToDongleAndBusy;
            Thread.Sleep(500);
            State = NimbleState.ConnectedToDongle;
            return new string[] { "84EB1877AF2A" };
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

            string filename = "";
            switch (sequence)
            {
                case 21: filename = "IMPEDANCE_MP400_V10_1.txt"; break;
                case 22: filename = "IMPEDANCE_MP400_V2_1.txt"; break;
                case 23: filename = "IMPEDANCE_MP400_V1_1.txt"; break;
                case 24: filename = "IMPEDANCE_MP25_V10_1.txt"; break;
                case 25: filename = "IMPEDANCE_CG25_V10_1.txt"; break;

                case 27: filename = "COMPLIANCEON_A_1.txt"; break;
                case 28: filename = "COMPLIANCEON_B_1.txt"; break;
            }
            string root = @"C:\Users\Patrick\Dropbox\Preitaly\NimbleBluetoothImpedanceManager\TelemParserTests\Data\14_514_PCB1-84EB1877AF2A-a968c4bc-5de2-44e1-837c-319e0875c6db-2015-08-07_01-28-02-PM";
            string fullpath = Path.Combine(root, filename);
            string alltext = File.ReadAllText(fullpath);

            Thread.Sleep(100);
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndWorking)
                    State = NimbleState.ConnectedToNimbleAndReady;
            }
            data = alltext.Replace("\r\n", "\r").Split('\r');
            return true;
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
            State = NimbleState.ConnectedToNimbleAndReady;
            return "a968c4bc-5de2-44e1-837c-319e0875c6db";
            //throw new NotImplementedException();
        }

        public bool DisconnectFromNimble()
        {
            FakeWork();
            ConnectedToRemoteDevice = false;
            State = NimbleState.ConnectedToDongle;
            return true;
            //throw new NotImplementedException();
        }


        //Stim State Vars
        bool StimOn = false;
        int CurrentRampProgress = 0;
        int RampMax;
        int RampMin;



        public bool IsStimOn(out bool StimOn)
        {
            StimOn = true;
            FakeWork();
            return true;
        }
        public bool SetStimActivity(bool stimOn)
        {
            FakeWork();
            return true;
        }
        public int GetRampLevel()
        {
            FakeWork();
            return 0;
        }
        public int GetRampProgress()
        {
            FakeWork();
            return 0;
        }
        public bool SetRampLevel(int RampLevel)
        {
            FakeWork();
            return true;
        }

        public int GetMaxRampLevel(SequenceFileManager sfm)
        {
            return 10;
        }

        bool FakeWork()
        {
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndReady)
                {
                    State = NimbleState.ConnectedToNimbleAndWorking;
                }
                else
                    return false;
            }
            System.Threading.Thread.Sleep(100);
            lock (stateLock)
            {
                if (State == NimbleState.ConnectedToNimbleAndWorking)
                    State = NimbleState.ConnectedToNimbleAndReady;
            }
            return true;
        }
    }
}
