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
            this.cmbSubject = new System.Windows.Forms.ComboBox();
            this.cmbMeasurementRecord = new System.Windows.Forms.ComboBox();
            this.chkSpecificMeasurement = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // cmbSubject
            // 
            this.cmbSubject.FormattingEnabled = true;
            this.cmbSubject.Location = new System.Drawing.Point(12, 43);
            this.cmbSubject.Name = "cmbSubject";
            this.cmbSubject.Size = new System.Drawing.Size(177, 21);
            this.cmbSubject.TabIndex = 0;
            // 
            // cmbMeasurementRecord
            // 
            this.cmbMeasurementRecord.FormattingEnabled = true;
            this.cmbMeasurementRecord.Location = new System.Drawing.Point(12, 70);
            this.cmbMeasurementRecord.Name = "cmbMeasurementRecord";
            this.cmbMeasurementRecord.Size = new System.Drawing.Size(177, 21);
            this.cmbMeasurementRecord.TabIndex = 1;
            // 
            // chkSpecificMeasurement
            // 
            this.chkSpecificMeasurement.FormattingEnabled = true;
            this.chkSpecificMeasurement.Location = new System.Drawing.Point(12, 97);
            this.chkSpecificMeasurement.Name = "chkSpecificMeasurement";
            this.chkSpecificMeasurement.Size = new System.Drawing.Size(177, 94);
            this.chkSpecificMeasurement.TabIndex = 2;
            // 
            // Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 446);
            this.Controls.Add(this.chkSpecificMeasurement);
            this.Controls.Add(this.cmbMeasurementRecord);
            this.Controls.Add(this.cmbSubject);
            this.Name = "Viewer";
            this.Text = "Viewer";
            this.Load += new System.EventHandler(this.Viewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSubject;
        private System.Windows.Forms.ComboBox cmbMeasurementRecord;
        private System.Windows.Forms.CheckedListBox chkSpecificMeasurement;
    }
}