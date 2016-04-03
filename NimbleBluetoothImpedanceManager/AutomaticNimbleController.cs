using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NimbleBluetoothImpedanceManager.Properties;
using Nimble.Sequences;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    class AutomaticNimbleController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private object automaticActionLock = new object();

        private INimbleCommsManager nimble;
        private SequenceFileManager fileManager;

        private Timer tmrAliveScan;
        private Timer tmrImpedance;

        public TimeSpan AliveScanPeriod { get; private set; }

        public TimeSpan ImpedanceMeasurePeriod { get; private set; }

        public DateTime NextDueAliveScan { get; private set; }
        public DateTime NextDueImpedanceMeasure { get; private set; }

        public bool AutomaticControlEnabled { get; private set; } //= false;

        private readonly TimeSpan MIN_ALIVESCAN_PERIOD = new TimeSpan(0, 0, 1, 0);
        private readonly TimeSpan MIN_IMPEDANCE_PERIOD = new TimeSpan(0, 0, 20, 0);

        private List<NimbleProcessor> processorsToMeasure = new List<NimbleProcessor>();



        public delegate void AutomaticActionHappenedEventHandler(object sender, AutomaticActionEventArgs e);
        public event AutomaticActionHappenedEventHandler AutomaticActionHappened;

        public class AutomaticActionEventArgs : EventArgs
        {
            public List<string> SuccessfullyMeasuredDevices { get; set; }
            public int SuccessfullMeasurements { get; set; }
            public string[] AliveDevices { get; set; }

            //public DateTime NextDueAliveScan { get; set; }
            //public DateTime NextDueImpedanceMeasure { get; set; }
            public ActionType Type { get; set; }
            public enum ActionType
            {
                AliveScan,
                Measurements,
                DeepScan,
                AutoStarted,
            }
        }

        protected virtual void OnAutomaticAction(AutomaticActionEventArgs e)
        {
            if (AutomaticActionHappened != null)
                AutomaticActionHappened(this, e);
        }

        public AutomaticNimbleController(INimbleCommsManager Nimble, SequenceFileManager FileManager)
        {
            NextDueAliveScan = DateTime.MinValue;
            NextDueImpedanceMeasure = DateTime.MinValue;

            nimble = Nimble;
            fileManager = FileManager;
        }

        /// <summary>
        /// Uses default periods of 5 min and 60 min for alive scan and impedance scan respectively.
        /// </summary>
        public void StartAutomaticControl()
        {

            StartAutomaticControl(Settings.Default.AliveScanPeriod, Settings.Default.ImpedanceScanPeriod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliveScanPeriod">Minimum 1 minutes</param>
        /// <param name="impedanceMeasurePeriod">Minimum 20 minutes</param>
        public void StartAutomaticControl(TimeSpan aliveScanPeriod, TimeSpan impedanceMeasurePeriod)
        {
            AliveScanPeriod = aliveScanPeriod > MIN_ALIVESCAN_PERIOD ? aliveScanPeriod : MIN_ALIVESCAN_PERIOD;
            ImpedanceMeasurePeriod = impedanceMeasurePeriod > MIN_IMPEDANCE_PERIOD ? impedanceMeasurePeriod : MIN_IMPEDANCE_PERIOD;

            long gcd = GCD((long)AliveScanPeriod.TotalMilliseconds, (long)ImpedanceMeasurePeriod.TotalMilliseconds);
            long offset = gcd / 2;
            TimeSpan aliveScanOffset = TimeSpan.FromMilliseconds(offset);

            lock (automaticActionLock)
            {
                //if (NextDueAliveScan > DateTime.MinValue && NextDueImpedanceMeasure > DateTime.MinValue)
                //{
                //    TimeSpan AliveScanDueTime = NextDueAliveScan - DateTime.Now;
                //    TimeSpan ImpedanceDueTime = NextDueImpedanceMeasure - DateTime.Now;
                //}

                //NextDueAliveScan = DateTime.Now + TimeSpan.FromSeconds(1);
                //NextDueImpedanceMeasure = DateTime.Now + aliveScanOffset;
                if (NextDueAliveScan > DateTime.MinValue && NextDueImpedanceMeasure > DateTime.MinValue)
                {
                    TimeSpan nextAliveScan = NextDueAliveScan - DateTime.Now;
                    if (nextAliveScan.TotalSeconds < 0)
                        nextAliveScan = TimeSpan.FromSeconds(1);
                    TimeSpan nextImpedanceScan = NextDueImpedanceMeasure - DateTime.Now;
                    if (nextImpedanceScan.TotalSeconds < 0)
                        nextImpedanceScan = TimeSpan.FromSeconds(2);

                    tmrAliveScan = new Timer(AutoAction_AliveScan, null,
                        nextAliveScan, AliveScanPeriod);

                    tmrImpedance = new Timer(AutoAction_ImpedanceMeasurements, null,
                        nextImpedanceScan, ImpedanceMeasurePeriod);

                    logger.Info("Next alive scan in {0}", nextAliveScan);
                    logger.Info("Next impedance scan in {0}", nextImpedanceScan);
                }

                else
                {
                    tmrAliveScan = new Timer(AutoAction_AliveScan, null, TimeSpan.FromSeconds(1), AliveScanPeriod);
                    tmrImpedance = new Timer(AutoAction_ImpedanceMeasurements,
                        null, aliveScanOffset, ImpedanceMeasurePeriod);

                    NextDueImpedanceMeasure = DateTime.Now + aliveScanOffset;
                    NextDueAliveScan = DateTime.Now + TimeSpan.FromSeconds(1);

                    logger.Info("Next alive scan in {0}", TimeSpan.FromSeconds(1));
                    logger.Info("Next impedance scan in {0}", aliveScanOffset);
                }
                AutomaticControlEnabled = true;
            }
            logger.Info("Automatic control started. AliveScanPeriod={0}, ImpedanceMeasurePeriod={1}", AliveScanPeriod, ImpedanceMeasurePeriod);
        }

        public void StopAutomaticControl()
        {
            lock (automaticActionLock)
            {
                tmrImpedance.Change(Timeout.Infinite, -1);
                tmrAliveScan.Change(Timeout.Infinite, -1);

                AutomaticControlEnabled = false;
                logger.Info("Automatic control stopped");
            }

        }

        public void AutoAction_AliveScan(object state)
        {
            DateTime start = DateTime.Now;
            lock (automaticActionLock)
            {
                NextDueAliveScan = DateTime.Now + AliveScanPeriod;
                logger.Info("Alive scan started");
                logger.Debug("lock time: {0}s", (start - DateTime.Now).TotalSeconds);
                string[] found = nimble.DiscoverDevices();
                if (found == null)
                    found = new string[0];
                logger.Info("Found {1} device(s) {0}", string.Join(", ", found), found.Length);
                RecordFoundDevices(found, "AliveDevices.txt");
                logger.Info("Alive scan finished. Took {0}s", (DateTime.Now - start).TotalSeconds);

                OnAutomaticAction(new AutomaticActionEventArgs()
                {
                    AliveDevices = found,
                    Type = AutomaticActionEventArgs.ActionType.AliveScan
                });
            }
        }

        public void AutoAction_ImpedanceMeasurements(object state)
        {
            NextDueImpedanceMeasure = DateTime.Now + ImpedanceMeasurePeriod;
            logger.Info("Automatic impedance measurements started");

            List<NimbleProcessor> reallyAliveDevices = new List<NimbleProcessor>();

            fileManager.ScanDirectory(Settings.Default.SequenceScanFolder);

            List<string> devicesMeasured = new List<string>();
            int measurementsMade = 0;

            foreach (NimbleProcessor nimbleProcessor in processorsToMeasure)
            {
                DateTime start = DateTime.Now;
                lock (automaticActionLock)
                {
                    logger.Debug("lock time: {0}s", (DateTime.Now - start).TotalSeconds);
                    logger.Info("Starting impedance measurement for {0}", nimbleProcessor);
                    if (nimble.ConnectToNimble(nimbleProcessor.BluetoothAddress))
                    {
                        var tmp = new NimbleProcessor { Name = nimble.NimbleName, BluetoothAddress = nimbleProcessor.BluetoothAddress };
                        reallyAliveDevices.Add(tmp);
                        devicesMeasured.Add(tmp.ToString());

                        var guid = nimble.RemoteNimbleProcessor.GenGUID;
                        logger.Debug("got sequence id: {0}", guid);

                        measurementsMade += DoMeasurements(guid);
                        nimble.DisconnectFromNimble();
                    }
                    else
                    {
                        logger.Warn("Could not connect to {0} for imedance measurement", nimbleProcessor);
                    }
                    logger.Info("Automatic impedance finished. Took {0}s", (DateTime.Now - start).TotalSeconds);
                }
            }
            RecordFoundDevices(reallyAliveDevices, "ReallyAliveDevices.txt");

            OnAutomaticAction(new AutomaticActionEventArgs()
            {
                SuccessfullyMeasuredDevices = devicesMeasured,
                SuccessfullMeasurements = measurementsMade,
                Type = AutomaticActionEventArgs.ActionType.Measurements
            });

        }

        public List<NimbleProcessor> AutoAction_DeepScanForProcessors()
        {
            DateTime start = DateTime.Now; ;
            lock (automaticActionLock)
            {

                try
                {
                    logger.Info("Deep scan started");
                    logger.Trace("lock time: {0}s", (start - DateTime.Now).TotalSeconds);
                    string[] found = nimble.DiscoverDevices();

                    if (found == null)
                        found = new string[0];

                    logger.Info("Deep scan: now trying to connect to the following processors: {0}",
                        string.Join(", ", found));
                    List<NimbleProcessor> tmpProcessors = new List<NimbleProcessor>();
                    foreach (string address in found)
                    {
                        logger.Info("seeing if {0} is a nimble processor", address);
                        if (nimble.ConnectToNimble(address))
                        {
                            var tmp = new NimbleProcessor { Name = nimble.NimbleName, BluetoothAddress = address };
                            logger.Info("Found processor: {0}", tmp);
                            tmpProcessors.Add(tmp);
                            nimble.DisconnectFromNimble();
                        }
                        else
                            logger.Warn("Could not connect to {0}", address);
                    }
                    logger.Info("Deep scan finished. Took {0}s", (DateTime.Now - start).TotalSeconds);
                    return tmpProcessors;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
            return new List<NimbleProcessor>();
        }

        public void SetProcessorsToMonitor(List<NimbleProcessor> processors)
        {
            lock (automaticActionLock)
            {
                processorsToMeasure.Clear();
                foreach (NimbleProcessor p in processors)
                {
                    processorsToMeasure.Add(p);
                }
                NimbleProcessor[] plist = processors.ToArray();
                logger.Info("Set processor to monitor: {0}", string.Join<NimbleProcessor>(", ", plist));
            }
        }

        private void RecordFoundDevices(string[] found, string filename)
        {
            //string filename = "AliveDevices.txt";
            string fullpath = Path.Combine(Settings.Default.ImpedanceOutputFolder, filename);

            using (FileStream fs = new FileStream(fullpath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("{0}|{1}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.ffff"), string.Join(", ", found));
                    sw.Flush();
                }
            }
        }

        private void RecordFoundDevices(List<NimbleProcessor> processors, string filename)
        {
            string[] all = new string[processors.Count];
            for (int i = 0; i < processors.Count; i++)
            {
                all[i] = processors[i].ToString();
            }
            RecordFoundDevices(all, filename);
        }

        //public void DebugImpedanceCheck()
        //{
        //    lock (automaticActionLock)
        //    {
        //        var guid = nimble.GetSequenceGUID();
        //        logger.Debug("got sequence id: {0}", guid);

        //        if (fileManager.CompiledSequences.ContainsKey(guid))
        //        {
        //            logger.Info("Collecting telem data for sequence {0} on {1}({2})", guid, nimble.NimbleName,
        //                nimble.RemoteDeviceId);
        //            CompiledSequence compseq = fileManager.CompiledSequences[guid];

        //            var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);
        //            foreach (KeyValuePair<int, string> kvp in compseq.MeasurementSegments)
        //            {
        //                for (int i = 0; i < 1; i++)
        //                {
        //                    logger.Info("Collecting segment {0} repeat {1}", kvp.Value, i);
        //                    string[] telemData;
        //                    bool res = nimble.CollectTelemetryData(kvp.Key, out telemData);
        //                    if (!res)
        //                    {
        //                        if (telemData == null)
        //                            telemData = new string[] { "Collection failed. no data collected" };
        //                    }
        //                    SaveTelemData(telemData, kvp.Value, fullSavePath);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            logger.Warn("Guid not found {0}", guid);
        //        }
        //    }  
        //}

        /// <summary>
        /// Returns the number of measurements made
        /// </summary>
        /// <returns></returns>
        public int DoMeasurements(string sequenceGUID)
        {
            if (fileManager.CompiledSequences.ContainsKey(sequenceGUID))
            {
                var cs = fileManager.CompiledSequences[sequenceGUID];
                int[] segIDs = cs.MeasurementSegments.Keys.ToArray();
                return MeasureSubset(sequenceGUID, segIDs);
            }
            logger.Warn("Bad sequence guid: {0}", sequenceGUID);
            return 0;

            //int measurementsMade = 0;
            //DateTime start = DateTime.Now;
            //lock (automaticActionLock)
            //{
            //    var guid = nimble.GetSequenceGUID();
            //    logger.Debug("got sequence id: {0}", guid);

            //    if (fileManager.CompiledSequences.ContainsKey(guid))
            //    {
            //        logger.Info("Collecting telem data for sequence {0} on {1}({2})", guid, nimble.NimbleName,
            //            nimble.RemoteDeviceId);
            //        CompiledSequence compseq = fileManager.CompiledSequences[guid];

            //        var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);
            //        foreach (KeyValuePair<int, string> kvp in compseq.MeasurementSegments)
            //        {
            //            measurementsMade += MeasureSegment(kvp.Value, kvp.Key, fullSavePath);
            //        }
            //        ThreadPool.QueueUserWorkItem(Preprocess, fullSavePath);
            //    }
            //    else
            //    {
            //        logger.Warn("Guid not found {0}, perhaps try a scan?", guid);
            //    }
            //}
            //logger.Info("Doing measurements finished. Took {0}s", (DateTime.Now - start).TotalSeconds);
            //return measurementsMade;
        }

        private int MeasureSegment(string segmentName, int segmentNumber, string fullSavePath)
        {
            int measurementsMade = 0;
            for (int i = 0; i < 6; i++)
            {
                string[] telemData;
                bool res = nimble.CollectTelemetryData(segmentNumber, out telemData);
                if (!res)
                {
                    if (telemData == null || telemData.Length == 0)
                    {
                        telemData = new string[] { "Collection failed. no data collected" };
                        logger.Warn("Measurement failed. No data collected for sequence {0}({1}) on {2}({3})",
                            segmentName, segmentNumber, nimble.NimbleName, nimble.RemoteDeviceId);
                    }
                    logger.Info("Receive Timeout. Colleted {5} lines on segment {0}({1}), repeat {2} on {3}({4})",
                        segmentName, segmentNumber, i, nimble.NimbleName, nimble.RemoteDeviceId, telemData.Length);
                    measurementsMade++;
                }
                else
                {
                    logger.Info("Colleted {5} lines on segment {0}({1}), repeat {2} on {3}({4})",
                        segmentName, segmentNumber, i, nimble.NimbleName, nimble.RemoteDeviceId, telemData.Length);
                    measurementsMade++;
                }
                SaveTelemData(telemData, segmentName, fullSavePath);
            }
            return measurementsMade;
        }

        public int MeasureSubset(string sequenceGuid, int[] subsetToMeasure)
        {
            int measurementsMade = 0;
            DateTime start = DateTime.Now;
            lock (automaticActionLock)
            {
                var guid = sequenceGuid;
                //logger.Debug("subset got sequence id: {0}", guid);

                if (fileManager.CompiledSequences.ContainsKey(guid))
                {
                    logger.Info("Collecting telem data for sequence {0} on {1}({2})", guid, nimble.NimbleName,
                       nimble.RemoteDeviceId);
                    CompiledSequence compseq = fileManager.CompiledSequences[guid];
                    var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);

                    foreach (int segID in subsetToMeasure)
                    {
                        if (compseq.MeasurementSegments.ContainsKey(segID))
                        {
                            var segName = compseq.MeasurementSegments[segID];
                            measurementsMade += MeasureSegment(segName, segID, fullSavePath);
                        }
                        else
                        {
                            logger.Warn("Segment ID {0} not found in sequence with guid {1}", segID, guid);
                        }
                    }
                    ThreadPool.QueueUserWorkItem(Preprocess, fullSavePath);
                }
                else
                {
                    logger.Warn("Guid not found {0}, perhaps try a scan?", guid);
                }
            }
            logger.Info("Doing measurements finished. Took {0}s", (DateTime.Now - start).TotalSeconds);
            return measurementsMade;
        }

        private void Preprocess(object path)
        {
            try
            {
                DateTime start = DateTime.Now; ;
                logger.Info("Preprocessing measurement record: {0}", path);
                string s = path as string;
                if (s == null)
                    return;

                var recordMaybe = NimbleMeasurementRecord.OpenMeasurementRecord(s);
                if (!recordMaybe.HasValue)
                    return;

                NimbleMeasurementRecord record = recordMaybe.Value;
                fileManager.ProcessSequenceResponse(record);

                logger.Info("Preprocessing of {1} finished. Took {0}s", (DateTime.Now - start).TotalSeconds, Path.GetFileName(s));
            }
            catch (Exception ex)
            {
                logger.Error("Preprocessing Nimble measurement record failed {0}, {1}", path, ex);
            }

        }

        /// <summary>
        /// Does a compliance check on the connected nimble processor
        /// </summary>
        //public void DoComplianceCheck()
        //{
        //    lock (automaticActionLock)
        //    {
        //        var guid = nimble.GetSequenceGUID();
        //        logger.Debug("got sequence id: {0}", guid);


        //        if (fileManager.CompiledSequences.ContainsKey(guid))
        //        {
        //            logger.Info("Collecting compliance data for sequence {0} on {1}({2})", guid, nimble.NimbleName,
        //                nimble.RemoteDeviceId);
        //            CompiledSequence compseq = fileManager.CompiledSequences[guid];

        //            var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);
        //            foreach (KeyValuePair<int, string> kvp in compseq.MeasurementSegments)
        //            {
        //                for (int i = 0; i < 3; i++)
        //                {
        //                    logger.Info("Collecting segment {0} repeat {1}", kvp.Value, i);
        //                    string[] telemData;
        //                    bool res = nimble.CollectTelemetryData(27, out telemData);
        //                    if (!res)
        //                    {
        //                        if (telemData == null)
        //                            telemData = new string[] { "Collection failed. no data collected" };
        //                    }
        //                    SaveTelemData(telemData, kvp.Value, fullSavePath);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            logger.Warn("Guid not found {0}", guid);
        //        }
        //    }
        //}

        private void SaveTelemData(string[] telemData, string measurementName, string folder)
        {
            int file_counter = 0;
            string rawDataFileName = string.Format("{0}_{1}.txt", measurementName, file_counter);
            while (File.Exists(Path.Combine(folder, rawDataFileName)))
            {
                file_counter++;
                rawDataFileName = string.Format("{0}_{1}.txt", measurementName, file_counter);
            }

            string rawDataPath = Path.Combine(folder, rawDataFileName);

            FileStream fs = new FileStream(rawDataPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            if (telemData != null)
                foreach (string s in telemData)
                {
                    sw.WriteLine(s);
                }
            sw.Flush();
            fs.Close();
        }

        private string GetTelemSavePath(string RemoteAddr, string NimbleName, string GenGuid)
        {
            string outputDir = Settings.Default.ImpedanceOutputFolder;

            string saveFolder = string.Format("{0}-{1}-{2}-{3}", NimbleName, RemoteAddr, GenGuid,
                DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt"));

            string fullSavePath = Path.Combine(outputDir, saveFolder);
            if (Directory.Exists(fullSavePath))
            {
                logger.Error("Output folder for this impedance measurement already exists. {0}", fullSavePath);
            }
            else
            {
                Directory.CreateDirectory(fullSavePath);
            }
            return fullSavePath;
        }

        private static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long tmp = b;
                b = a % b;
                a = tmp;
            }

            return a;
        }


    }
}
