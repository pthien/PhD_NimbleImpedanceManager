using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace NimbleDataProcessor
{
    class CmdLineOptions
    {
        [Option('d', "datafolder", Required=false, HelpText="Path of the folder containg the raw impedance measurements")]
        public string DataFolder { get; set; }

        [Option('s', "sequencefolder", Required = false, HelpText = "Path of the folder containg the sequence generator outputs")]
        public string SequenceFolder { get; set; }

        [Option('o', "output", Required = false, HelpText = "Path of the folder containg to store the output")]
        public string OutputFolder { get; set; }

        [Option('v', "visualMode", Required = false)]
        public bool VisualMode { get; set; }
    }
}
