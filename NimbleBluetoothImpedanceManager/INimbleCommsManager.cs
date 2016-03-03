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
        bool IsStimOn();
        bool SetStimActivity();
        int GetRampLevel();
        int GetRampProgress();
        void SetRampLevel();
    }
}