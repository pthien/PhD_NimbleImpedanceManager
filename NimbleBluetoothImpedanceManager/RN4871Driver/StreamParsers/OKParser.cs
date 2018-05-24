using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace NimbleBluetoothImpedanceManager.RN4871Driver.StreamParsers
{
    class OKParser
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private EventWaitHandle AOKReceived { get; } = new AutoResetEvent(false);

        public void processChunk(object sender, DataChunker.ChunkReadyEventArgs e)
        {
            if (e.Chunk.Trim() == "AOK" || e.Chunk.Trim() == "CMD> AOK")
            {
                _logger.Info("Ok processed");
                AOKReceived.Set();
            }
        }

        public bool OKFound(int timeout = 500)
        {
            return AOKReceived.WaitOne(timeout);
        }
    }
}
