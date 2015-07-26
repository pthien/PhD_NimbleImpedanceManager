using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NLog;
using PIC_Sequence;

namespace Nimble.Sequences
{
    public partial class Viewer : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        private string _sequenceDirectory;

        public string SequenceDirectory
        {
            get { return _sequenceDirectory; }
            set
            {
                _sequenceDirectory = value;
                txtWorkingDir.Text = value;
            }
        }

        private string _impedanceDirectory;

        public string ImpedanceDirectory
        {
            get { return _impedanceDirectory; }
            set
            {
                _impedanceDirectory = value;
                txtOutputDir.Text = value;
            }
        }

        private SequenceFileManager fileman;

        public Viewer()
        {
            InitializeComponent();
            fileman = new SequenceFileManager();
        }

        private void Viewer_Load(object sender, EventArgs e)
        {

        }

        private void btnSetWorkingDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = txtWorkingDir.Text;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                SequenceDirectory = fbd.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = txtOutputDir.Text;
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                ImpedanceDirectory = fbd.SelectedPath;
            }
        }

        private List<NimbleMeasurementRecord> records;

        private void ScanForLogs(string impedanceDir)
        {
            records = SequenceFileManager.GetTelemetryRecords(impedanceDir);

            var subjects = records.GroupBy(m => m.SubjectName).Select(g => g.First()).Select(x => x.SubjectName).ToArray();

            logger.Info("Found 3 subjects: {0}", string.Join(", ", subjects));

            cmbSubject.Items.Clear();
            cmbSubject.Items.AddRange(subjects);

        }

        private void cmbSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSubject.SelectedIndex >= 0)
            {
                string subject = cmbSubject.Text;

                var dates = records.Where(x => x.SubjectName == subject).ToArray();//.Select(x => x.Timestamp);

                cmbMeasurementRecord.Items.Clear();
                foreach (var dateTime in dates)
                {
                    cmbMeasurementRecord.Items.Add(dateTime);
                }
            }
        }


        private void cmbMeasurementRecord_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMeasurementRecord.SelectedIndex >= 0)
            {
                lstSpecificMeasurements.Items.Clear();
                object o = cmbMeasurementRecord.SelectedItem;
                NimbleMeasurementRecord r = (NimbleMeasurementRecord)o;

                foreach (var m in r.GetMeasurments())
                {
                    lstSpecificMeasurements.Items.Add(string.Format("{0} #{1}", m.SegmentName, m.RepeateCount));
                }

                FileStream fs = new FileStream("test.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(r.RecordDirectory);
                NimbleImpedanceRecord impedanceRecord = fileman.ProcessSequenceResponse(r);
                foreach (NimbleSegmentImpedance m in impedanceRecord.SegmentImpedances)
                {
                    sw.Write(m + ",");

                    for (int i = 1; i < 7; i++)
                    {
                        var one = m.Impedances.Where(x => x._Implant == Implant.ImplantA && x._Electrode == i);
                        if (one.Any())
                        {
                            var first = one.First();
                            sw.Write(first._Impedance_ohms+",");
                        }
                        else
                        {
                            sw.Write(" ,");
                        }

                    }
                    for (int i = 1; i < 7; i++)
                    {
                        var one = m.Impedances.Where(x => x._Implant == Implant.ImplantB && x._Electrode == i);
                        if (one.Any())
                        {
                            var first = one.First();
                            sw.Write(first._Impedance_ohms + ",");
                        }
                        else
                        {
                            sw.Write(" ,");
                        }
                    }
                    sw.WriteLine();
                }
                sw.Flush();
                fs.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fileman.ScanDirectory(SequenceDirectory);
            ScanForLogs(ImpedanceDirectory);
        }

        private void lstSpecificMeasurements_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            object o = cmbMeasurementRecord.SelectedItem;
            NimbleMeasurementRecord r = (NimbleMeasurementRecord)o;
            NimbleImpedanceRecord imprec = fileman.ProcessSequenceResponse(r);
            List<NimbleSegmentMeasurment> measurments = r.GetMeasurments();


            foreach (var l in lstSpecificMeasurements.SelectedItems)
            {
                string whole = l.ToString();
                string name = whole.Split(' ')[0];

                IEnumerable<NimbleSegmentImpedance> theseSegments = imprec.SegmentImpedances.Where(x => x.SegmentName == name);

                foreach (NimbleSegmentImpedance segmentImpedance in theseSegments)
                {
                    Series segSeries = new Series(segmentImpedance.ToString());

                    foreach (ImpedanceResult impedanceResult in segmentImpedance.Impedances)
                    {
                        int x = impedanceResult._Electrode + (impedanceResult._Implant == Implant.ImplantA ? 0 : 6);
                        double y = impedanceResult._Impedance_ohms;
                        DataPoint dp = new DataPoint(x, y);
                        segSeries.Points.Add(dp);
                    }
                    chart1.Series.Add(segSeries);
                    //segSeries.Points.Add()
                    //segmentImpedance.Impedances.
                }
                //foreach (NimbleSegmentMeasurment m in theseMeasures)
                //{
                //    //fileman.ProcessSequenceResponse()
                //}
            }
        }


    }
}
