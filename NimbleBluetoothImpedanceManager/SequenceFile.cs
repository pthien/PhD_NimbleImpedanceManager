using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NimbleBluetoothImpedanceManager.Sequences
{
    public class SequenceFile
    {
        public Dictionary<int, string> MeasurementSegments; 
        public SequenceFile(string hpath, string cpath)
        {
            if (Path.GetFileNameWithoutExtension(hpath) != "Sequence")
            {
                throw new ArgumentException("All sequence files are named Sequence.c or Sequence.h");
            }

            string alltext = File.ReadAllText(hpath);
            ExtractSequenceComments(alltext);
        }

        private void ExtractSequenceComments(string alltext)
        {
            MeasurementSegments = new Dictionary<int, string>();
            Regex r = new Regex(@"{SegmentComments: ([ A-Z0-9a-z-_,|()]+)}");
            var match = r.Match(alltext);
            if (match.Success)
            {
                string allcomments = match.Groups[1].Value;
                string[] commentsSplit = allcomments.Split(',');

                for (int i = 0; i < commentsSplit.Length; i++)
                {
                    if (commentsSplit[i].Trim().StartsWith("IMPEDANCE"))
                    {
                        MeasurementSegments.Add(i, commentsSplit[i].ToString());
                    }
                }
            }
        }
    }

    public class PulseDataFile
    {
        public PulseDataFile(string path)
        {

        }
    }
}
