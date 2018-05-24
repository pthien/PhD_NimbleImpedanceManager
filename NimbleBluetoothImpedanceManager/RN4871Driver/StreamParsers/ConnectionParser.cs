using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NLog;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.StreamParsers
{
    class ConnectionParser
    {
        Regex regex_connected = new Regex("%CONNECT,0,([A-F0-9]{12})%");
        Regex regex_streamOpen = new Regex("%STREAM_OPEN%");

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public EventWaitHandle StreamOpenWaitHandle { get; } = new AutoResetEvent(false);


        public void processChunk(object sender, DataChunker.ChunkReadyEventArgs e)
        {
            string chunk = e.Chunk.Trim();

            if (e.Chunk == "Trying")
            {
                _logger.Info("Trying to connect to device");
            }
            if (regex_connected.IsMatch(chunk))
            {
                _logger.Info("Connected to device");
            }
            if (regex_streamOpen.IsMatch(chunk))
            {
                _logger.Info("Stream open");
                StreamOpenWaitHandle.Set();
            }
        }
    }
}
