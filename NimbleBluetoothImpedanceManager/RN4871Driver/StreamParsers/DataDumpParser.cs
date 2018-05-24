using NimbleBluetoothImpedanceManager.RN4871Driver.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.StreamParsers
{
    class DataDumpParser
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private string _bluetoothAddress;
        private string _name;
        private string _connected;
        private string _authen;
        private string _features;
        private string _services;

        private Regex regex_bluetoothAddress = new Regex("BTA=([A-F0-9]{12})");
        private Regex regex_Name = new Regex(@"Name=([A-Z0-9\-]+)");
        private Regex regex_Connected = new Regex("Connected=(.*)");
        private Regex regex_Authentication = new Regex("Authen=([0-9])");
        private Regex regex_Features = new Regex("Features=([A-F0-9]{4})");
        private Regex regex_Services = new Regex("Services=([A-F0-9]{2})");

        private ExpectedLine _state = ExpectedLine.BTA;

        private EventWaitHandle DongleCommandRequestDumpSettings { get; } = new AutoResetEvent(false);

        private DataDump Results { get; set; }

        public void ScanChunksForDongleDataDump(object sender, DataChunker.ChunkReadyEventArgs e)
        {
            if (e.Chunk.Trim().Length == 0)
                return;
            Match m = null;
            switch (_state)
            {
                case ExpectedLine.BTA:
                    m = regex_bluetoothAddress.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _bluetoothAddress = m.Groups[1].Value;
                        _state = ExpectedLine.Name;
                    }
                    break;
                case ExpectedLine.Name:
                    m = regex_Name.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _name = m.Groups[1].Value;
                        _state = ExpectedLine.Connected;
                    }
                    break;
                case ExpectedLine.Connected:
                    m = regex_Connected.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _connected = m.Groups[1].Value;
                        _state = ExpectedLine.Authen;
                    }
                    break;
                case ExpectedLine.Authen:
                    m = regex_Authentication.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _authen = m.Groups[1].Value;
                        _state = ExpectedLine.Features;
                    }
                    break;
                case ExpectedLine.Features:
                    m = regex_Features.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _features = m.Groups[1].Value;
                        _state = ExpectedLine.Services;
                    }
                    break;
                case ExpectedLine.Services:
                    m = regex_Services.Match(e.Chunk.Trim());
                    if (m.Success)
                    {
                        _services = m.Groups[1].Value;
                        _state = ExpectedLine.BTA;

                        Results = new DataDump(_bluetoothAddress, _name, _connected, _authen, _features, _services);

                        _logger.Info("Data dump parser sucssfully got dump: {0}", Results);

                        DongleCommandRequestDumpSettings.Set();
                    }
                    break;
                default:
                    break;
            }
            if (m == null || !m.Success)
            {
                _state = ExpectedLine.BTA;
            }

            //_logger.Debug("scanner state is now " + _state.ToString());
        }

        public DataDump? GetDataDump()
        {
            _bluetoothAddress = null;
            _name = null;
            _connected = null;
            _authen = null;
            _features = null;
            _services = null;
            _state = ExpectedLine.BTA;

            DongleCommandRequestDumpSettings.Reset();

            if (DongleCommandRequestDumpSettings.WaitOne(500))
                return Results;
            return null;
        }

        private enum ExpectedLine
        {
            BTA,
            Name,
            Connected,
            Authen,
            Features,
            Services
        }

    }
}
