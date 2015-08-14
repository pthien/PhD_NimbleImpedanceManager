using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            btnRefresh_Click(null, null);
        }


        private void Bgw_DoWork(object e)
        {
            logger.Info("do work started {0}", e);
            Thread.Sleep(10000);
            logger.Info("do work wake {0}", e);
            //throw new NotImplementedException();
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

                var dates = records.Where(x => x.SubjectName == subject).OrderByDescending(x => x.Timestamp).ToArray();//.Select(x => x.Timestamp);

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

                var uniqeSegments = segmentMeasurments.GroupBy(x => x.SegmentName).Select(g => g.First()).ToList();

                foreach (NimbleSegmentMeasurment segment in uniqeSegments)
                {
                    lstSpecificMeasurements.Items.Add(segment.SegmentName);
                }


                //fs.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            fileman.ScanDirectory(SequenceDirectory);
            ScanForLogs(ImpedanceDirectory);
        }

        private void lstSpecificMeasurements_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.ChartAreas[0].AxisX.Maximum = 14;
                chart1.ChartAreas[0].AxisX.Minimum = 0;


                object o = cmbMeasurementRecord.SelectedItem;
                NimbleMeasurementRecord r = (NimbleMeasurementRecord)o;
                NimbleImpedanceRecord imprec = fileman.ProcessSequenceResponse(r);
                List<NimbleSegmentMeasurment> measurments = r.GetMeasurments();

                bool lockedToImpedance = false;
                bool lockedToCompliance = false;
                int count = 0;
                foreach (var l in lstSpecificMeasurements.SelectedItems)
                {
                    string whole = l.ToString();
                    string name = whole.Split(' ')[0];

                    IEnumerable<NimbleSegmentTelemetry> theseSegments = imprec.SegmentImpedances.Where(x => x.SegmentName == name);

                    foreach (NimbleSegmentTelemetry segmentImpedance in theseSegments)
                    {
                        Series segSeries = new Series(segmentImpedance.ToString());

                        foreach (TelemetryResult impedanceResult in segmentImpedance.Impedances)
                        {
                            if (impedanceResult is ImpedanceResult)
                            {
                                if (lockedToCompliance)
                                    continue;
                                lockedToImpedance = true;
                                int x = impedanceResult.Electrode + (impedanceResult.Implant == Implant.ImplantA ? 0 : 6);
                                double y = ((ImpedanceResult)impedanceResult)._Impedance_ohms;
                                DataPoint dp = new DataPoint(x, y);
                                dp.MarkerStyle = MarkerStyle.Cross;
                                segSeries.Points.Add(dp);
                                count++;
                            }
                            else
                            {
                                if (lockedToImpedance)
                                    continue;
                                lockedToCompliance = true;
                                int x = impedanceResult.Electrode + (impedanceResult.Implant == Implant.ImplantA ? 0 : 6);
                                double y = ((ComplianceResult)impedanceResult).InCompliance ? 1 : -1;
                                DataPoint dp = new DataPoint(x, y);
                                segSeries.Points.Add(dp);
                                count++;
                            }
                        }
                        chart1.Series.Add(segSeries);
                    }
                }
                if (count == 0)
                {
                    chart1.Series.Clear();
                    chart1.Titles.Add("Error: No data");
                }
                else
                {
                    chart1.ChartAreas[0].AxisX.Maximum = 12;
                    chart1.ChartAreas[0].AxisX.Maximum = 12;

                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(0.51, 1.49, "A1");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(1.51, 2.49, "A2");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(2.51, 3.49, "A3");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(3.51, 4.49, "A4");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(4.51, 5.49, "A5");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(5.51, 6.49, "A6");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(6.51, 7.49, "B1");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(7.51, 8.49, "B2");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(8.51, 9.49, "B3");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(9.51, 10.49, "B4");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(10.51, 11.49, "B5");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(11.51, 12.49, "B6");

                    //chart1.ChartAreas[0].AxisX.la
                    chart1.Legends[0].LegendStyle = LegendStyle.Row;
                    chart1.Legends[0].Docking = Docking.Top;
                    if (lockedToImpedance)
                    {
                        chart1.ChartAreas[0].AxisY.Title = "Impedance (ohms)";
                        chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Auto;
                        chart1.ChartAreas[0].AxisY.CustomLabels.Clear();
                    }
                    else
                    {
                        chart1.ChartAreas[0].AxisY.Title = "";
                        chart1.ChartAreas[0].AxisY.CustomLabels.Add(0.9, 1.1, "In compliance");
                        chart1.ChartAreas[0].AxisY.CustomLabels.Add(-1.1, -.9, "Out of compliance");
                        chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Auto;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.ToString(), "An error has occured. It may be wise to restart the program");
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
                            string s = string.Format("X={0},Y={1:G7} {2}", prop.XValue, prop.YValues[0], result.Series);
                            tooltip.Show(s, this.chart1,
                                            pos.X, pos.Y - 15);
                            //logger.Debug("yay!");
                        }
                        //logger.Debug("({0},{1:G}) - ({2},{3})", pos.X, pos.Y, pointXPixel, pointYPixel);

                    }
                }
            }
        }


    }
}
