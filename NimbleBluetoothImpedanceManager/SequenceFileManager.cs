using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;


namespace NimbleBluetoothImpedanceManager.Sequences
{
    class SequenceFileManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Dictionary<string, FilesForGenerationGUID> FilesByGenGUID;

        Regex regex_guid = new Regex("{GenerationGUID: ([A-Za-z0-9-]+)}");
        public SequenceFileManager()
        {
            FilesByGenGUID = new Dictionary<string, FilesForGenerationGUID>();
        }

        public void ScanDirectory(string path)
        {
            if (path == "")
                path = @"\\prometheus\user$\thienp\VisionProcessingHardware\DualImplants";

            DoScanDirectory(path);

            foreach (KeyValuePair<string, FilesForGenerationGUID> keyValuePair in FilesByGenGUID)
            {
                FilesForGenerationGUID filesforguid = keyValuePair.Value;
                if (filesforguid.AllFilesReferenced)
                {
                    FilesByGenGUID[keyValuePair.Key].GenerateSequenceFile();
                    logger.Info("Built sequence for: {0}", keyValuePair.Key);
                }

            }
        }
        public void DoScanDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                TestIfValidFile(Path.Combine(path,f));
            }
            try
            {
                foreach (string d in Directory.GetDirectories(path))
                {
                    DoScanDirectory(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }


        }

        public void TestIfValidFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);

            //FileStream fs = new FileStream(filepath, FileMode.Open);
            string alltext = File.ReadAllText(filepath);

            var match = regex_guid.Match(alltext);
            if (match.Success)
            {
                string guid = match.Groups[1].Value;

                FilesForGenerationGUID files;
                if (FilesByGenGUID.ContainsKey(guid))
                {
                    files = FilesByGenGUID[guid];
                }
                else
                {
                    files = new FilesForGenerationGUID();
                    logger.Info("Found new sequence: {0}", guid);
                }
                switch (filename)
                {
                    case "PulseData.c":
                        files.PulseData_c = filepath;
                        break;
                    case "PulseData.h":
                        files.PulseData_h = filepath;
                        break;
                    case "Sequence.c":
                        files.Sequence_c = filepath;
                        break;
                    case "Sequence.h":
                        files.Sequence_h = filepath;
                        break;
                }
                FilesByGenGUID[guid] = files;
            }
        }
    }

    public class FilesForGenerationGUID
    {
        public string PulseData_c, PulseData_h, Sequence_c, Sequence_h;

        public bool AllFilesReferenced
        {
            get { return PulseData_c != "" && PulseData_h != "" && Sequence_c != "" && Sequence_h != ""; }
        }
        public SequenceFile sequenceFile;
        public PulseDataFile pulseDataFile;

        public void GenerateSequenceFile()
        {
            sequenceFile = new SequenceFile(Sequence_h, Sequence_c);
        }
    }
}
