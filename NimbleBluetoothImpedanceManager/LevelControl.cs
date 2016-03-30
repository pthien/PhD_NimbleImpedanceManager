using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NimbleBluetoothImpedanceManager
{
    partial class LevelControl : Form
    {
        private static Logger logger;
        INimbleCommsManager nimbleCommsManager;
        System.Timers.Timer tmrStatus;

        int StimLevel;
        bool StimOn;
        int currentLevel;
        public LevelControl(INimbleCommsManager commsManager)
        {
            logger = LogManager.GetCurrentClassLogger();

            nimbleCommsManager = commsManager;
            tmrStatus = new System.Timers.Timer(2000);
            tmrStatus.Elapsed += statusTimer_Tick;
            tmrStatus.Start();
            InitializeComponent();
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            currentLevel = nimbleCommsManager.GetRampLevel();
            logger.Debug("tick");
            System.Threading.Thread.Sleep(1000);
        }

        private void LevelControl_Load(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (currentLevel <= 0)
                return;
            var res = nimbleCommsManager.SetRampLevel(currentLevel - 1);
            if (res)
                currentLevel--;
        }


        private void btnUp_Click(object sender, EventArgs e)
        {
            var res = nimbleCommsManager.SetRampLevel(currentLevel + 1);
            if (res)
                currentLevel++;
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
