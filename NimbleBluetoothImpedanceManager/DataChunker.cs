using System;
using System.Linq;
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

        public const int Timeout = 50;
        public DataChunker()
        {
            tmr = new Timer(Timeout);
            tmr.Elapsed += tmr_Elapsed;
            tmr.AutoReset = false;
            logger.Info("Chunker started");
            sb = new StringBuilder(2000);
        }

        void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (mylock)
            {
                logger.Debug("Callback! {0}.{1}", DateTime.Now.Second, DateTime.Now.Millisecond);
                SendOffChunk(ChunkingReason.Timeout);
            }
        }

        private void SendOffChunk(ChunkingReason reason)
        {
            if (sb.Length == 0)
            {
                logger.Debug("Not sending 0 length chunk that was made ready due to {0}", reason);
                return;
            }

            string chunk = sb.ToString();
            logger.Debug("Chunk Ready because of {1}: {0}", chunk.EscapeWhiteSpace(), reason);
            if (ChunkReady != null)
                ChunkReady(this, new ChunkReadyEventArgs { Chunk = chunk, Reason = reason });
            sb.Clear();
        }

        public void AddChar(char c)
        {
            lock (mylock)
            {
                tmr.Stop();
                tmr.Start();
                //logger.Debug("Pause! {0}.{1}", DateTime.Now.Second, DateTime.Now.Millisecond);

                if (c == '\r' || c == '\n')
                {
                    sb.Append(c);
                    SendOffChunk(ChunkingReason.Newline);
                }
              else  if (c == '{')
                {
                    SendOffChunk(ChunkingReason.Newline);
                    sb.Append(c);
                }
                else 
                      sb.Append(c);
            }
        }

        public enum ChunkingReason
        {
            Timeout,
            Newline
        }

        public delegate void ChunkReadyEventHandler(object sender, ChunkReadyEventArgs e);

        public event ChunkReadyEventHandler ChunkReady;

        public class ChunkReadyEventArgs : EventArgs
        {
            public string Chunk { get; set; }
            public ChunkingReason Reason;
        }

        public void Count()
        {
            logger.Info("Number of handlers: " + ChunkReady?.GetInvocationList().Count());
            string msg = string.Join(", ", ChunkReady?.GetInvocationList().Select(x => x.Target.ToString()));
            logger.Info(msg);
        }
    }


}