using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NimbleBluetoothImpedanceManager.Properties;
using NimbleBluetoothImpedanceManager.Sequences;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    class AutomaticNimbleController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private object automaticActionLock = new object();

        private NimbleCommsManager nimble;
        private SequenceFileManager fileManager;

        private Timer tmrAliveScan;
        private Timer tmrImpedance;

        private TimeSpan _AliveScanPeriod;
        private TimeSpan _ImpedanceMeasurePeriod;
        public TimeSpan AliveScanPeriod
        {
            get { return _AliveScanPeriod; }
        }
        public TimeSpan ImpedanceMeasurePeriod
        {
            get { return _ImpedanceMeasurePeriod; }
        }

        private DateTime NextDueAliveScan = DateTime.MinValue;
        private DateTime NextDueImpedanceMeasure = DateTime.MinValue;

        private bool _AutomaticControlEnabled = false;
        public bool AutomaticControlEnabled
        {
            get { return _AutomaticControlEnabled; }
        }

        private readonly TimeSpan MIN_ALIVESCAN_PERIOD = new TimeSpan(0, 0, 3, 0);
        private readonly TimeSpan MIN_IMPEDANCE_PERIOD = new TimeSpan(0, 0, 5, 0);

        private List<NimbleProcessor> processorsToMeasure = new List<NimbleProcessor>();

        public AutomaticNimbleController(NimbleCommsManager Nimble, SequenceFileManager FileManager)
        {
            nimble = Nimble;
            fileManager = FileManager;
        }

        /// <summary>
        /// Uses default periods of 5 min and 60 min for alive scan and impedance scan respectively.
        /// </summary>
        public void StartAutomaticControl()
        {
            StartAutomaticControl(MIN_ALIVESCAN_PERIOD, MIN_IMPEDANCE_PERIOD);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliveScanPeriod">Minimum 1 minutes</param>
        /// <param name="impedanceMeasurePeriod">Minimum 20 minutes</param>
        public void StartAutomaticControl(TimeSpan aliveScanPeriod, TimeSpan impedanceMeasurePeriod)
        {
            _AliveScanPeriod = aliveScanPeriod > MIN_ALIVESCAN_PERIOD ? aliveScanPeriod : MIN_ALIVESCAN_PERIOD;
            _ImpedanceMeasurePeriod = impedanceMeasurePeriod > MIN_IMPEDANCE_PERIOD ? impedanceMeasurePeriod : MIN_IMPEDANCE_PERIOD;

            long gcd = GCD((long)_AliveScanPeriod.TotalMilliseconds, (long)_ImpedanceMeasurePeriod.TotalMilliseconds);
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

                tmrAliveScan = new Timer(AutoAction_AliveScan, null, TimeSpan.FromSeconds(1), _AliveScanPeriod);
                tmrImpedance = new Timer(AutoAction_ImpedanceMeasurements,
                    null, aliveScanOffset, _ImpedanceMeasurePeriod);
                _AutomaticControlEnabled = true;
            }
            logger.Info("Automatic control started. AliveScanPeriod={0}, ImpedanceMeasurePeriod={1}", AliveScanPeriod, ImpedanceMeasurePeriod);
        }

        public void StopAutomaticControl()
        {
            tmrImpedance.Change(Timeout.Infinite, -1);
            tmrAliveScan.Change(Timeout.Infinite, -1);

            lock (automaticActionLock)
            {
                _AutomaticControlEnabled = false;
            }
            logger.Info("Automatic control stopped");
        }

        public void AutoAction_AliveScan(object state)
        {
            DateTime start = DateTime.Now;
            lock (automaticActionLock)
            {
                NextDueAliveScan = DateTime.Now + AliveScanPeriod;
                logger.Info("Alive scan started");
                logger.Trace("lock time: {0}s", (start - DateTime.Now).TotalSeconds);
                string[] found = nimble.DiscoverDevices();
                if (found == null)
                    found = new string[0];
                logger.Info("Found {1} device(s) {0}", string.Join(", ", found), found.Length);
                RecordFoundDevices(found);
                logger.Info("Alive scan finished. Took {0}s", (DateTime.Now - start).TotalSeconds);
            }
        }

        public void AutoAction_ImpedanceMeasurements(object state)
        {
            DateTime start = DateTime.Now; ;
            lock (automaticActionLock)
            {
                NextDueImpedanceMeasure = DateTime.Now + ImpedanceMeasurePeriod;
                logger.Info("Automatic impedance measurements started");
                logger.Trace("lock time: {0}s", (DateTime.Now - start).TotalSeconds);
                //string[] found = nimble.DiscoverDevices();

                foreach (NimbleProcessor nimbleProcessor in processorsToMeasure)
                {
                    logger.Info("Starting impedance measurement for {0}", nimbleProcessor);
                    if (nimble.ConnectToNimble(nimbleProcessor.BluetoothAddress))
                    {
                        DoImpedanceCheck();
                        nimble.DisconnectFromNimble();
                    }
                }
                logger.Info("Automatic impedance finished. Took {0}s", (DateTime.Now - start).TotalSeconds);

            }
        }

        public List<NimbleProcessor> AutoAction_DeepScanForProcessors()
        {
            DateTime start = DateTime.Now; ;
            lock (automaticActionLock)
            {
                logger.Info("Deep scan started");
                logger.Trace("lock time: {0}s", (start - DateTime.Now).TotalSeconds);
                string[] found = nimble.DiscoverDevices();

                logger.Info("Deep scan: now trying to connect to each processor");
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
        }

        public void SetProcessorsToMonitor(IEnumerable<NimbleProcessor> processors)
        {
            lock (automaticActionLock)
            {
                processorsToMeasure.Clear();
                foreach (NimbleProcessor p in processors)
                {
                    logger.Info("Set processor to monitor: {0}", p);
                    processorsToMeasure.Add(p);
                }
            }
        }

        private void RecordFoundDevices(string[] found)
        {
            string filename = "AliveDevices.txt";
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

        public void DoImpedanceCheck()
        {
            lock (automaticActionLock)
            {
                var guid = nimble.GetSequenceGUID();
                logger.Debug("got sequence id: {0}", guid);

                if (fileManager.FilesByGenGUID.ContainsKey(guid))
                {
                    logger.Info("Collecting telem data for sequence {0} on {1}({2})", guid, nimble.NimbleName,
                        nimble.RemoteDeviceId);
                    FilesForGenerationGUID x = fileManager.FilesByGenGUID[guid];
                    var fullSavePath = GetTelemSavePath(nimble.RemoteDeviceId, nimble.NimbleName, guid);
                    foreach (KeyValuePair<int, string> kvp in x.sequenceFile.MeasurementSegments)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            string[] telemData;
                            bool res = nimble.CollectTelemetryData(kvp.Key, out telemData);
                            if (!res)
                            {
                                if (telemData == null)
                                    telemData = new string[] { "Collection failed. no data collected" };
                            }
                            SaveTelemData(telemData, kvp.Value, fullSavePath);
                        }
                    }
                }
                else
                {
                    logger.Warn("Guid not found {0}", guid);
                }
            }
        }

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
