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

        public Viewer(SequenceFileManager fman)
        {
            InitializeComponent();
            fileman = fman;
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
                lstSpecificMeasurements.Items.Clear();
            }
        }


        private void cmbMeasurementRecord_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMeasurementRecord.SelectedIndex >= 0)
            {
                lstSpecificMeasurements.Items.Clear();
                object o = cmbMeasurementRecord.SelectedItem;
                NimbleMeasurementRecord r = (NimbleMeasurementRecord)o;

                List<NimbleSegmentResponse> segmentMeasurments = r.GetMeasurments();

                var uniqeSegments = segmentMeasurments.GroupBy(x => x.SegmentName).Select(g => g.First()).ToList();

                foreach (NimbleSegmentResponse segment in uniqeSegments)
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
                int maxRange = 22;
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.ChartAreas[0].AxisX.Maximum = 2*maxRange+1;
                chart1.ChartAreas[0].AxisX.Minimum = 0;


                object o = cmbMeasurementRecord.SelectedItem;
                NimbleMeasurementRecord r = (NimbleMeasurementRecord)o;
                NimbleImpedanceRecord imprec = fileman.ProcessSequenceResponse(r);
                List<NimbleSegmentResponse> measurments = r.GetMeasurments();

                bool lockedToImpedance = false;
                bool lockedToCompliance = false;
                int count = 0;

                //chart1.DataManipulator.IsEmptyPointIgnored = true;

                foreach (var l in lstSpecificMeasurements.SelectedItems)
                {
                    string whole = l.ToString();
                    string name = whole.Split(' ')[0];

                    IEnumerable<NimbleSegmentTelemetry> theseSegments = imprec.SegmentImpedances.Where(x => x.SegmentName == name);

                    foreach (NimbleSegmentTelemetry segmentImpedance in theseSegments)
                    {
                        Series segSeries = new Series(segmentImpedance.ToString());
                        //segSeries.IsXValueIndexed = true;

                        for (int i = 1; i <=maxRange; i++)
                        {
                            var query = segmentImpedance.Impedances.Where(x => x.Implant == Implant.ImplantA && x.Electrode == i);
                            if (query.Any())
                            {
                                TelemetryResult result = query.First();
                                if (result is ImpedanceResult)
                                {
                                    if (lockedToCompliance)
                                        continue;
                                    lockedToImpedance = true;
                                    int x = result.Electrode + (result.Implant == Implant.ImplantA ? 0 : maxRange);
                                    double y = ((ImpedanceResult)result)._Impedance_ohms;
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
                                    int x = result.Electrode + (result.Implant == Implant.ImplantA ? 0 : maxRange);
                                    double y = ((ComplianceResult)result).InCompliance ? 1 : -1;
                                    DataPoint dp = new DataPoint(x, y);
                                    segSeries.Points.Add(dp);
                                    count++;
                                }
                            }
                            else
                            {
                                //DataPoint dp = new DataPoint(i, 0);
                                //dp.IsEmpty = true;
                                //segSeries.Points.Add(dp);
                            }
                        }

                        for (int i = 1; i <= maxRange; i++)
                        {
                            var query = segmentImpedance.Impedances.Where(x => x.Implant == Implant.ImplantB && x.Electrode == i);
                            if (query.Any())
                            {
                                TelemetryResult result = query.First();
                                if (result is ImpedanceResult)
                                {
                                    if (lockedToCompliance)
                                        continue;
                                    lockedToImpedance = true;
                                    int x = result.Electrode + (result.Implant == Implant.ImplantA ? 0 : maxRange);
                                    double y = ((ImpedanceResult)result)._Impedance_ohms;
                                   
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
                                    int x = result.Electrode + (result.Implant == Implant.ImplantA ? 0 : maxRange);
                                  
                                    double y = ((ComplianceResult)result).InCompliance ? 1 : -1;
                                    DataPoint dp = new DataPoint(x, y);
                                    segSeries.Points.Add(dp);
                                    count++;
                                }
                            }
                            else
                            {
                                //DataPoint dp = new DataPoint(i+22, 0);
                                //dp.IsEmpty = true;
                                //segSeries.Points.Add(dp);
                            }
                        }
                        //foreach (TelemetryResult impedanceResult in segmentImpedance.Impedances)
                        //{
                        //    if (impedanceResult is ImpedanceResult)
                        //    {
                        //        if (lockedToCompliance)
                        //            continue;
                        //        lockedToImpedance = true;
                        //        int x = impedanceResult.Electrode + (impedanceResult.Implant == Implant.ImplantA ? 0 : 100);
                        //        double y = ((ImpedanceResult)impedanceResult)._Impedance_ohms;
                        //        DataPoint dp = new DataPoint(x, y);
                        //        dp.MarkerStyle = MarkerStyle.Cross;
                        //        segSeries.Points.Add(dp);
                        //        count++;
                        //    }
                        //    else
                        //    {
                        //        if (lockedToImpedance)
                        //            continue;
                        //        lockedToCompliance = true;
                        //        int x = impedanceResult.Electrode + (impedanceResult.Implant == Implant.ImplantA ? 0 : 100);
                        //        double y = ((ComplianceResult)impedanceResult).InCompliance ? 1 : -1;
                        //        DataPoint dp = new DataPoint(x, y);
                        //        segSeries.Points.Add(dp);
                        //        count++;
                        //    }
                        //}
                        chart1.Series.Add(segSeries);
                       // break;
                    }
                }
                if (count == 0)
                {
                    chart1.Series.Clear();
                    chart1.Titles.Add("Error: No data");
                }
                else
                {
                    //chart1.ChartAreas[0].AxisX.Maximum = 140;
                    //chart1.ChartAreas[0].AxisX.Maximum = 12;

                    chart1.ChartAreas[0].AxisX.CustomLabels.Clear();
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(0.51, 1.49, "A1");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(1.51, 2.49, "A2");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(2.51, 3.49, "A3");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(3.51, 4.49, "A4");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(4.51, 5.49, "A5");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(5.51, 6.49, "A6");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(6.51, 7.49, "A7");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(7.51, 8.49, "A8");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(8.51, 9.49, "A9");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(9.51, 10.49, "A10");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(10.51, 11.49, "A11");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(11.51, 12.49, "A12");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(12.51, 13.49, "A13");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(13.51, 14.49, "A14");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(14.51, 15.49, "A15");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(15.51, 16.49, "A16");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(16.51, 17.49, "A17");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(17.51, 18.49, "A18");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(18.51, 19.49, "A19");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(19.51, 20.49, "A20");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(20.51, 21.49, "A21");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(21.51, 22.49, "A22");

                    int offset = maxRange;
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 0.51, offset + 1.49, "B1");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 1.51, offset + 2.49, "B2");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 2.51, offset + 3.49, "B3");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 3.51, offset + 4.49, "B4");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 4.51, offset + 5.49, "B5");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 5.51, offset + 6.49, "B6");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 6.51, offset + 7.49, "B7");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 7.51, offset + 8.49, "B8");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 8.51, offset + 9.49, "B9");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 9.51, offset + 10.49, "B10");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 10.51, offset + 11.49, "B11");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 11.51, offset + 12.49, "B12");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 12.51, offset + 13.49, "B13");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 13.51, offset + 14.49, "B14");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 14.51, offset + 15.49, "B15");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 15.51, offset + 16.49, "B16");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 16.51, offset + 17.49, "B17");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 17.51, offset + 18.49, "B18");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 18.51, offset + 19.49, "B19");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 19.51, offset + 20.49, "B20");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 20.51, offset + 21.49, "B21");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 21.51, offset + 22.49, "B22");
                    chart1.ChartAreas[0].AxisX.CustomLabels.Add(offset + 22.51, offset + 23.49, "B23");


                    //chart1.ChartAreas[0].AxisX.la
                    chart1.Legends[0].LegendStyle = LegendStyle.Row;
                    chart1.Legends[0].Docking = Docking.Top;
                    chart1.Legends[0].Enabled= false;
                    if (lockedToImpedance)
                    {
                        chart1.ChartAreas[0].AxisY.Title = "Impedance (ohms)";
                        chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Auto;
                        chart1.ChartAreas[0].AxisY.CustomLabels.Clear();

                        //chart1.ChartAreas[0].AxisY.Maximum = 130000;
                        //chart1.ChartAreas[0].AxisY.Minimum = 0;

                        chart1.ChartAreas[0].RecalculateAxesScale();
                    }
                    else
                    {
                        chart1.ChartAreas[0].AxisY.Title = "";
                        chart1.ChartAreas[0].AxisY.CustomLabels.Add(0.9, 1.1, "In compliance");
                        chart1.ChartAreas[0].AxisY.CustomLabels.Add(-1.1, -.9, "Out of compliance");
                        chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Auto;

                        //chart1.ChartAreas[0].AxisY.Maximum = 2;
                        //chart1.ChartAreas[0].AxisY.Minimum = -2;
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
            try
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
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }


    }
}
