using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NimbleBluetoothImpedanceManager.Properties;
using NLog;

namespace NimbleBluetoothImpedanceManager
{
    public partial class AliveDevicesViewer : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public AliveDevicesViewer()
        {
            InitializeComponent();
        }

        private void AliveDevicesViewer_Load(object sender, EventArgs e)
        {

            try
            {
                string folder = Path.Combine(Settings.Default.ImpedanceOutputFolder, "ReallyAliveDevices.txt");
                string alltext = File.ReadAllText(folder);
                string[] lines = alltext.Split('\n');

                chart1.Series.Clear();
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
                chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart1.ChartAreas[0].CursorX.Interval = 5 / 24.0 / 60.0; // 1 minute
                chart1.MouseWheel += chData_MouseWheel;
                Dictionary<string, Series> serieses = new Dictionary<string, Series>();

                foreach (string line in lines)
                {

                    string[] parts = line.Trim().Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;
                    DateTime timestamp;
                    DateTime.TryParse(parts[0], out timestamp);

                    List<string> unaccountedforDevices = parts.ToList();
                    unaccountedforDevices.RemoveAt(0);
                    for (int i = 0; i < unaccountedforDevices.Count; i++)
                        unaccountedforDevices[i] = unaccountedforDevices[i].Trim();
                    
                    List<string> accountedForDevices = new List<string>();
                    accountedForDevices = serieses.Keys.ToList();
                    foreach (string unaccountedforDevice in unaccountedforDevices)
                    {
                        if (!serieses.ContainsKey(unaccountedforDevice))
                        {
                            serieses[unaccountedforDevice] = new Series
                            {
                                ChartType = SeriesChartType.FastLine,
                                XValueType = ChartValueType.DateTime,
                                Name = unaccountedforDevice
                            };
                        }
                        serieses[unaccountedforDevice].Points.Add(new DataPoint(timestamp.ToOADate(), 1));
                    }
                    foreach (string accountedForDevice in accountedForDevices)
                    {
                        if (!unaccountedforDevices.Contains(accountedForDevice))
                            serieses[accountedForDevice].Points.Add(new DataPoint(timestamp.ToOADate(), -1));
                    }
                    // if (serieses.ContainsKey())
                }

                foreach (KeyValuePair<string, Series> kvp in serieses)
                {
                    chart1.Series.Add(kvp.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Something when wrong when trying to view alive devices. {0}", ex);
               // throw;
            }

        }
        private void chData_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }

                if (e.Delta > 0)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2;
                    double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2;
                    // double posYStart = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 1;
                    //double posYFinish = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 1;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    //chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch
            {

            }
        }

    }
}
