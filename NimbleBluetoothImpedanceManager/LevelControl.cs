using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nimble.Sequences;

namespace NimbleBluetoothImpedanceManager
{
    partial class LevelControl : Form
    {
        private static Logger logger;
        INimbleCommsManager nimbleCommsManager;
        System.Timers.Timer tmrStatus;

        int StimLevel;
        bool? StimOn;
        int currentLevel;
        int setLevel;
        CompiledSequence relevantSequence;
        
        public LevelControl(INimbleCommsManager commsManager, SequenceFileManager filemanager)
        {
            logger = LogManager.GetCurrentClassLogger();

            nimbleCommsManager = commsManager;
            relevantSequence = filemanager.CompiledSequences[nimbleCommsManager.RemoteNimbleProcessor.GenGUID];
            
            bool temp;
            bool res = nimbleCommsManager.IsStimOn(out temp);
            if (!res)
                StimOn = null;
            else
                StimOn = temp;

            setLevel = nimbleCommsManager.GetRampLevel(relevantSequence);

            tmrStatus = new System.Timers.Timer(2000);
            tmrStatus.Elapsed += statusTimer_Tick;
            tmrStatus.Start();
            InitializeComponent();
        }

        void UpdateInterface()
        {
            if (this.InvokeRequired)
                this.BeginInvoke((Action)(() =>
                {
                    lblCurrentLevel.Text = currentLevel.ToString();
                    lblTarget.Text = setLevel.ToString();
                }));
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            currentLevel = nimbleCommsManager.GetRampProgress(relevantSequence);
            UpdateInterface();
            //logger.Debug("tick");
            //sSystem.Threading.Thread.Sleep(1000);
        }

        private void LevelControl_Load(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (setLevel <= 1)
                return;
            var res = nimbleCommsManager.SetRampLevel(setLevel - 1, relevantSequence);
            if (res)
                setLevel--;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var res = nimbleCommsManager.SetRampLevel(setLevel + 1, relevantSequence);
            if (res)
                setLevel++;
        }

        private void btnStimOn_Click(object sender, EventArgs e)
        {
            var res = nimbleCommsManager.SetStimActivity(true);
        }

        private void btnStimOff_Click(object sender, EventArgs e)
        {
            var res = nimbleCommsManager.SetStimActivity(false);
        }

        private void btnEnterValue_Click(object sender, EventArgs e)
        {
            using (FrmSetLevel setlvl = new FrmSetLevel(100, 0))
            {
                var res = setlvl.ShowDialog();
                if (res == DialogResult.OK)
                {
                    logger.Debug("{0}, val = {1}", res, setlvl.Result);
                }
                else
                {

                }
            }
        }

    }
}
