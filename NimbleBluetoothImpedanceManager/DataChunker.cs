using System;
using System.Text;
using System.Timers;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    class DataChunker
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Timer tmr;
        private StringBuilder sb;

        private object mylock = new object();

        public DataChunker()
        {
            tmr = new Timer(50);
            tmr.Elapsed += tmr_Elapsed;
            tmr.AutoReset = false;
            logger.Info("Chunker started");
            sb = new StringBuilder(100);
        }

        void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (mylock)
            {
                //logger.Debug("Callback! {0}.{1}", DateTime.Now.Second, DateTime.Now.Millisecond);
                SendOffChunk();
            }
        }

        private void SendOffChunk()
        {
            string chunk = sb.ToString().Trim();
            logger.Info("Chunk Ready: {0}", chunk);
            if (ChunkReady != null)
                ChunkReady(this, new ChunkReadyEventArgs { Chunk = chunk });
            sb.Clear();
        }

        public void AddChar(char c)
        {
            lock (mylock)
            {
                tmr.Stop();
                tmr.Start();
                //logger.Debug("Pause! {0}.{1}", DateTime.Now.Second, DateTime.Now.Millisecond);
                if(c == '\r' || c=='\n')
                    SendOffChunk();
                else
                    sb.Append(c);
                
            }
        }


        public delegate void ChunkReadyEventHandler(object sender, ChunkReadyEventArgs e);

        public event ChunkReadyEventHandler ChunkReady;

        public class ChunkReadyEventArgs : EventArgs
        {
            public string Chunk { get; set; }
        }
    }


}