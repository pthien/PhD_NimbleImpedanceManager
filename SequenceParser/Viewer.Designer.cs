namespace Nimble.Sequences
{
    partial class Viewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.cmbSubject = new System.Windows.Forms.ComboBox();
            this.cmbMeasurementRecord = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSetWorkingDir = new System.Windows.Forms.Button();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.txtWorkingDir = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.lstSpecificMeasurements = new System.Windows.Forms.ListBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSubject
            // 
            this.cmbSubject.FormattingEnabled = true;
            this.cmbSubject.Location = new System.Drawing.Point(12, 71);
            this.cmbSubject.Name = "cmbSubject";
            this.cmbSubject.Size = new System.Drawing.Size(298, 21);
            this.cmbSubject.TabIndex = 0;
            this.cmbSubject.SelectedIndexChanged += new System.EventHandler(this.cmbSubject_SelectedIndexChanged);
            // 
            // cmbMeasurementRecord
            // 
            this.cmbMeasurementRecord.FormattingEnabled = true;
            this.cmbMeasurementRecord.Location = new System.Drawing.Point(12, 98);
            this.cmbMeasurementRecord.Name = "cmbMeasurementRecord";
            this.cmbMeasurementRecord.Size = new System.Drawing.Size(298, 21);
            this.cmbMeasurementRecord.TabIndex = 1;
            this.cmbMeasurementRecord.SelectedIndexChanged += new System.EventHandler(this.cmbMeasurementRecord_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(195, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Set output directory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSetWorkingDir
            // 
            this.btnSetWorkingDir.Location = new System.Drawing.Point(195, 9);
            this.btnSetWorkingDir.Name = "btnSetWorkingDir";
            this.btnSetWorkingDir.Size = new System.Drawing.Size(115, 23);
            this.btnSetWorkingDir.TabIndex = 6;
            this.btnSetWorkingDir.Text = "Set working directory";
            this.btnSetWorkingDir.UseVisualStyleBackColor = true;
            this.btnSetWorkingDir.Click += new System.EventHandler(this.btnSetWorkingDir_Click);
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutputDir.Location = new System.Drawing.Point(12, 35);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.ReadOnly = true;
            this.txtOutputDir.Size = new System.Drawing.Size(177, 20);
            this.txtOutputDir.TabIndex = 5;
            this.txtOutputDir.Text = "txtOutputDir";
            // 
            // txtWorkingDir
            // 
            this.txtWorkingDir.BackColor = System.Drawing.SystemColors.Window;
            this.txtWorkingDir.Location = new System.Drawing.Point(12, 11);
            this.txtWorkingDir.Name = "txtWorkingDir";
            this.txtWorkingDir.ReadOnly = true;
            this.txtWorkingDir.Size = new System.Drawing.Size(177, 20);
            this.txtWorkingDir.TabIndex = 4;
            this.txtWorkingDir.Text = "txtWorkingDir";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(159, 337);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lstSpecificMeasurements
            // 
            this.lstSpecificMeasurements.FormattingEnabled = true;
            this.lstSpecificMeasurements.Location = new System.Drawing.Point(13, 126);
            this.lstSpecificMeasurements.Name = "lstSpecificMeasurements";
            this.lstSpecificMeasurements.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSpecificMeasurements.Size = new System.Drawing.Size(297, 108);
            this.lstSpecificMeasurements.TabIndex = 9;
            this.lstSpecificMeasurements.SelectedIndexChanged += new System.EventHandler(this.lstSpecificMeasurements_SelectedIndexChanged);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(316, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(627, 422);
            this.chart1.TabIndex = 10;
            this.chart1.Text = "chart1";
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 446);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.lstSpecificMeasurements);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSetWorkingDir);
            this.Controls.Add(this.txtOutputDir);
            this.Controls.Add(this.txtWorkingDir);
            this.Controls.Add(this.cmbMeasurementRecord);
            this.Controls.Add(this.cmbSubject);
            this.Name = "Viewer";
            this.Text = "Viewer";
            this.Load += new System.EventHandler(this.Viewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSubject;
        private System.Windows.Forms.ComboBox cmbMeasurementRecord;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSetWorkingDir;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.TextBox txtWorkingDir;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox lstSpecificMeasurements;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}