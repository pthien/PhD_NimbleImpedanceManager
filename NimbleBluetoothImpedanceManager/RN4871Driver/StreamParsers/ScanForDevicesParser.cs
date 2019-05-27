using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NimbleBluetoothImpedanceManager.RN4871Driver.Model;
using NLog;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.StreamParsers
{
    /// <summary>
    /// Scans data chunks for possible bluetooth devices to connect to.
    /// 
    /// Example console output: 
    /// Scanning
    /// %D88039FBF3A1,0,BLE-F3A1,,CD%
    /// [Keyboard input: X]
    /// AOK
    /// CMD>
    /// </summary>
    class ScanForDevicesParser
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private ScannerState _state;

        private Regex regex_Device = new Regex(@"%([A-F0-9]{12}),0,([A-Z0-9\-]+),,");

        private List<DeviceScanResult> Results = new List<DeviceScanResult>();

        public void processChunk(object sender, DataChunker.ChunkReadyEventArgs e)
        {
            if (e.Chunk.Trim().Length == 0)
                return;
            //_logger.Debug(e.Chunk);
            switch (_state)
            {
                case ScannerState.ExpectScanning:
                    if (e.Chunk.Trim() == "Scanning" || e.Chunk.Trim() == "CMD> Scanning")
                    {
                        _state = ScannerState.Listening;
                    }
                    break;
                case ScannerState.Listening:
                    Match m = regex_Device.Match(e.Chunk);
                    if (m.Success)
                    {
                        _logger.Debug("Device name: " + m.Groups[2].Value);
                        _logger.Debug("Device addr: " + m.Groups[1].Value);
                        DeviceScanResult result = new DeviceScanResult(m.Groups[1].Value, m.Groups[2].Value);
                        Results.Add(result);
                    }
                    else if (e.Chunk == "AOK")
                    {
                        _state = ScannerState.Finished;
                    }
                    else
                    {
                        _logger.Debug("Ignoring match: " + e.Chunk);
                    }
                    break;
            }


            //_logger.Debug("scanner state is now " + _state);
        }

        /// <summary>
        /// Clears the list of found devices
        /// </summary>
        public void Reset()
        {
            Results.Clear();
        }

        /// <summary>
        /// Waits the specified amount of time and then returns the list of found devices
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public List<DeviceScanResult> ScanForDevices(int timeout = 5000)
        {
            Thread.Sleep(timeout);
            return Results;
        }

        enum ScannerState
        {
            ExpectScanning,
            Listening,
            Finished,
        }

    }
}
