using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;



namespace Nimble.Sequences
{
    public class SequenceFileManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, FilesForGenerationGUID> FilesByGenGUID;
        public Dictionary<string, CompiledSequence> CompiledSequences;

        Regex regex_guid = new Regex("{GenerationGUID: ([A-Za-z0-9-]+)}");
        public SequenceFileManager()
        {
            FilesByGenGUID = new Dictionary<string, FilesForGenerationGUID>();
            CompiledSequences = new Dictionary<string, CompiledSequence>();
        }

        /// <summary>
        /// Scans a folder and all subfolders for Sequence or PulseData .c or .h files.
        /// </summary>
        /// <param name="path"></param>
        public void ScanDirectory(string path)
        {
            //if (path == "")
            //    path = @"\\prometheus\user$\thienp\VisionProcessingHardware\DualImplants";

            DoScanDirectory(path);

            foreach (KeyValuePair<string, FilesForGenerationGUID> kvp in FilesByGenGUID)
            {
                FilesForGenerationGUID filesforguid = kvp.Value;
                if (filesforguid.AllFilesReferenced && !CompiledSequences.ContainsKey(kvp.Key))
                {
                    CompiledSequence cs = new CompiledSequence(kvp.Value);
                    CompiledSequences.Add(kvp.Key, cs);
                    logger.Info("Built sequence for: {0}", kvp.Key);
                }

            }
        }
        private void DoScanDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                TestIfValidFile(Path.Combine(path, f));
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
            try
            {
                string alltext = File.ReadAllText(filepath);

                var match = regex_guid.Match(alltext);
                string guid = CompiledSequence.ExtractGuid(alltext).ToString();
                if (guid != Guid.Empty.ToString())
                {
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
            catch (Exception ex)
            {
                logger.Error(ex.Message);
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

    }
}
