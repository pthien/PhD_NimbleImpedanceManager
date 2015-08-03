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

                List<NimbleSegmentMeasurment> segmentMeasurments = r.GetMeasurments();

                var uniqeSegments = segmentMeasurments.GroupBy(x => x.SegmentName).Select(g=>g.First()).ToList();

                foreach (NimbleSegmentMeasurment segment in uniqeSegments)
                {
                    lstSpecificMeasurements.Items.Add(segment.SegmentName);
                }

                //foreach (var m in r.GetMeasurments())
                //{
                //    lstSpecificMeasurements.Items.Add(string.Format("{0} #{1}", m.SegmentName, m.RepeatCount));
                //}

                //FileStream fs = new FileStream("test.txt", FileMode.Create);
                //StreamWriter sw = new StreamWriter(fs);
                //sw.WriteLine(r.RecordDirectory);
                //NimbleImpedanceRecord impedanceRecord = fileman.ProcessSequenceResponse(r);
                //foreach (NimbleSegmentImpedance m in impedanceRecord.SegmentImpedances)
                //{
                //    sw.Write(m + ",");

                //    for (int i = 1; i < 7; i++)
                //    {
                //        var one = m.Impedances.Where(x => x.Implant == Implant.ImplantA && x.Electrode == i);
                //        if (one.Any())
                //        {
                //            var first = one.First();
                //            sw.Write(first._Impedance_ohms+",");
                //        }
                //        else
                //        {
                //            sw.Write(" ,");
                //        }

                //    }
                //    for (int i = 1; i < 7; i++)
                //    {
                //        var one = m.Impedances.Where(x => x.Implant == Implant.ImplantB && x.Electrode == i);
                //        if (one.Any())
                //        {
                //            var first = one.First();
                //            sw.Write(first._Impedance_ohms + ",");
                //        }
                //        else
                //        {
                //            sw.Write(" ,");
                //        }
                //    }
                //    sw.WriteLine();
                //}
                //sw.Flush();
                //fs.Close();
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

                IEnumerable<NimbleSegmentTelemetry> theseSegments = imprec.SegmentImpedances.Where(x => x.SegmentName == name);

                foreach (NimbleSegmentTelemetry segmentImpedance in theseSegments)
                {
                    Series segSeries = new Series(segmentImpedance.ToString());

                    foreach (ImpedanceResult impedanceResult in segmentImpedance.Impedances)
                    {
                        int x = impedanceResult.Electrode + (impedanceResult.Implant == Implant.ImplantA ? 0 : 6);
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
        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = chart1.HitTest(pos.X, pos.Y, false,
                                            ChartElementType.DataPoint);
            foreach (var result in results)
            {
                
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    var prop = result.Object as DataPoint;
                    if (prop != null)
                    {
                        var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around the point)
                        //if (Math.Abs(pos.X - pointXPixel) < 3 /*&&
                        //    Math.Abs(pos.Y - pointYPixel) < 2*/)
                        {
                            tooltip.Show("X=" + prop.XValue + ", Y=" + prop.YValues[0] + "  " + result.Series, this.chart1,
                                            pos.X, pos.Y - 15);
                            logger.Debug("yay!");
                        }
                        logger.Debug("({0},{1}) - ({2},{3})", pos.X, pos.Y, pointXPixel, pointYPixel );

                    }
                }
            }
        }


    }
}
