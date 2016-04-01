using Nimble.Sequences;

namespace NimbleBluetoothImpedanceManager
{
    internal interface INimbleCommsManager
    {
        string Comport { get; }
        bool Initialised { get; }
        bool ConnectedToRemoteDevice { get; }
        string RemoteDeviceId { get; }
        string NimbleName { get; }
        NimbleProcessor RemoteNimbleProcessor { get; }
        NimbleState State { get; set; }
        //event BluetoothCommsDriver.ConnectionEstablishedEventHandler ConnectedToNimble;
        //event BluetoothCommsDriver.ConnectionLostEventHandler DisconnectedFromNimble;
        event NimbleCommsManager.StateChangedEventHandler StateChanged;

        /// <summary>
        /// Connects to a nimble processor. Returns true if bluetooth connection successful and nimble is responding
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool ConnectToNimble(string address);

        bool Initialise(string COMPort);
        string[] DiscoverDevices();
        bool CollectTelemetryData(int sequence, out string[] data);
        string GetSequenceGUID();
        bool DisconnectFromNimble();

        //Wireless ramp
        bool IsStimOn(out bool StimOn);

        /// <summary>
        /// Enables or disables stimulation on the connected nimble processor
        /// </summary>
        /// <param name="stimOn">True to enable stimulation, false to disable it</param>
        /// <returns>Returns true if the command succeeded</returns>
        bool SetStimActivity(bool stimOn);

        int GetRampLevel();
        int GetRampProgress();

        /// <summary>
        /// Sets the upper ramp level of stimulation
        /// </summary>
        /// <param name="RampLevel"></param>
        /// <returns>Returns true if the command succeeded</returns>
        bool SetRampLevel(int RampLevel);

        int GetMaxRampLevel(SequenceFileManager sfm);
    }
}