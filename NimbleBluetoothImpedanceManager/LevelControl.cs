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

        bool? StimOn;
        int currentLevel;
        int setLevel;
        CompiledSequence relevantSequence;

        bool ButtonsEnabled = true;

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
                    DoInterfaceUpdate();
                }));
            else
                DoInterfaceUpdate();
        }

        private void DoInterfaceUpdate()
        {
            Color clrReady = Color.LimeGreen;
            Color clrWorking = Color.Gold;
            Color clrNotReady = Color.Firebrick;

            lblCurrentLevel.Text = string.Format("{0} / {1}", currentLevel, setLevel);
            lblTarget.Text = string.Format("{0} / {1}", setLevel.ToString(), relevantSequence.GetMaxStimLevel());

            if (StimOn.HasValue)
            {
                statusStimOn.Text = StimOn.Value ? "Stimulation is on" : "Stimulation is off";
                statusStimOn.BackColor = StimOn.Value ? clrReady : clrNotReady;
            }
            else
            {
                statusStimOn.Text = "Something went wrong";
                statusStimOn.BackColor = clrWorking;
            }

            btnDown.Enabled = ButtonsEnabled;
            btnUp.Enabled = ButtonsEnabled;
            btnStimOff.Enabled = ButtonsEnabled;
            btnStimOn.Enabled = ButtonsEnabled;
            btnEnterValue.Enabled = ButtonsEnabled;

            lblMaxCurrent.Text = string.Format("Max current: {0}", relevantSequence.GetMaxCurrentInRampLevel(currentLevel));
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            bool _stimOn;
            int _rampprogress;
            int _setLevel;
            bool res = nimbleCommsManager.GetStimSummary(out _stimOn, out _rampprogress, out _setLevel, relevantSequence);

            if (res)
            {
                StimOn = _stimOn;
                currentLevel = _rampprogress;
                setLevel = _setLevel;
                UpdateInterface();
            }
            else
            {
                UpdateInterface();
            }
        }

        private void LevelControl_Load(object sender, EventArgs e)
        {
            statusTimer_Tick(null, null);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            tmrStatus.Stop();
            base.OnClosing(e);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            UpdateInterface();
            if (setLevel <= 1)
                return;
            var res = nimbleCommsManager.SetRampLevel(setLevel - 1, relevantSequence);
            if (res)
            {
                setLevel--;
            }
            else
            {
                logger.Error("Level decrease potentially not completed");
            }
            ButtonsEnabled = true;
            UpdateInterface();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            UpdateInterface();
            if (setLevel == relevantSequence.GetMaxStimLevel())
                return;
            var res = nimbleCommsManager.SetRampLevel(setLevel + 1, relevantSequence);
            if (res)
            {
                setLevel++;
            }
            else
            {
                logger.Error("Level increase potentially not completed");
            }
            ButtonsEnabled = true;
            UpdateInterface();
        }

        private void btnStimOn_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            UpdateInterface();
            var res = nimbleCommsManager.SetStimActivity(true);
            if (res)
                StimOn = true;
            else
                StimOn = null;
            ButtonsEnabled = true;
            UpdateInterface();
        }

        private void btnStimOff_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            UpdateInterface();
            var res = nimbleCommsManager.SetStimActivity(false);
            if (res)
                StimOn = false;
            else
                StimOn = null;
            ButtonsEnabled = true;
            UpdateInterface();
        }

        private void btnEnterValue_Click(object sender, EventArgs e)
        {
            ButtonsEnabled = false;
            UpdateInterface(); using (FrmSetLevel setlvl = new FrmSetLevel(relevantSequence.GetMaxStimLevel(), 1))
            {
                var res = setlvl.ShowDialog();
                if (res == DialogResult.OK)
                {
                    logger.Debug("set stim level to custom value returend {0}, val = {1}", res, setlvl.Result);
                    bool r = nimbleCommsManager.SetRampLevel(setlvl.Result, relevantSequence);
                    if (r)
                    {
                        logger.Info("Stimulation level set to {0}", setlvl.Result);
                        setLevel = setlvl.Result;
                    }
                    else
                        logger.Info("Attempt to set stimulation level to {0} potentially failed", setlvl.Result);
                }
                else
                {
                    logger.Debug("set stim level to custom value cancelled");
                }
            }
            ButtonsEnabled = true;
            UpdateInterface();
        }

    }
}
