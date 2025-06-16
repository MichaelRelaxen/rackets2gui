using rackets2listener;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace racketslistener2
{
    public partial class FormMain : Form
    {
        private Ratchetron api;
        private int pid;
        private string pathToScript;

        public FormMain(Ratchetron api)
        {
            this.api = api;
            pid = api.getCurrentPID();

            InitializeComponent();

            var hotkeys = Func.GetConfigData("hotkeysEnabled");
            checkBoxHotkeys.Checked = hotkeys == "true";
            var frameskip = Func.GetConfigData("frameskipEnabled");
            checkBoxFrameskip.Checked = frameskip == "true";

            this.KeyPreview = true;
        }

        private void buttonOpenScript_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Ratchet 2 TAS script file (*.rtas2s)|*.rtas2s|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.pathToScript = openFileDialog.FileName;
                    labelCurrentScript.Text = $"Current Script: {Path.GetFileName(pathToScript)}";
                }
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            api.PauseUnpauseRackets(true);
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            api.PauseUnpauseRackets(false);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            api.Disconnect();
        }

        private void buttonAdvance_Click(object sender, EventArgs e)
        {
            api.Framestep();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            api.CancelPlayback();
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            api.PauseUnpauseRackets(false);
            api.RestartPlayback();
        }

        private void buttonUploadRun_Click(object sender, EventArgs e)
        {
            api.PauseUnpauseRackets(false);
            var result = api.UploadInputsFile(this.pathToScript);
            if (result != null) 
            { 
                MessageBox.Show(result, "Error when uploading!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxHotkeys_CheckedChanged(object sender, EventArgs e)
        {
            Func.SetConfigData("hotkeysEnabled", checkBoxHotkeys.Checked ? "true" : "false");
        }

        private void checkBoxFrameskip_CheckedChanged(object sender, EventArgs e)
        {
            Func.SetConfigData("frameskipEnabled", checkBoxFrameskip.Checked ? "true" : "false");
            api.SetRenderingMode(checkBoxFrameskip.Checked, checkBoxRendering.Checked);
        }

        private void checkBoxRendering_CheckedChanged(object sender, EventArgs e)
        {
            api.SetRenderingMode(checkBoxFrameskip.Checked, checkBoxRendering.Checked);
        }

        private void checkBoxHideHud_CheckedChanged(object sender, EventArgs e)
        {
            api.SetHudStatus(checkBoxHideHud.Checked);
        }

        private void buttonSetAside_Click(object sender, EventArgs e)
        {
            api.SetAsideMethod();
        }

        private void buttonLoadSetAside_Click(object sender, EventArgs e)
        {
            api.LoadSetAsideMethod();
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (!checkBoxHotkeys.Checked) return;

            if (e.KeyCode == Keys.U)
                buttonUploadRun_Click(sender, e);
            else if (e.KeyCode == Keys.L) 
                buttonRestart_Click(sender, e);
            else if (e.KeyCode == Keys.C)
                buttonCancel_Click(sender, e);
            else if (e.KeyCode == Keys.P)
                buttonPause_Click(sender, e);
            else if (e.KeyCode == Keys.R)
                buttonResume_Click(sender, e);
            else if (e.KeyCode == Keys.S)
                buttonAdvance_Click(sender, e); 
        }

        private void buttonSetPosition_Click(object sender, EventArgs e)
        {
            api.SetPositionToLoadMethod();
        }

        private void buttonCopyPos_Click(object sender, EventArgs e)
        {
            api.CopyPositionToClipBoardMethod();
        }

        private void buttonSetPlanet_Click(object sender, EventArgs e)
        {
            api.SetCurrentLevelMethod((uint)comboLevelSelect.SelectedIndex);
        }

        private void forceAutosaveButton_Click(object sender, EventArgs e)
        {
            api.SetSaveModeMethod(3); // 3 is autosave.
        }

        private void pastePositionButton_Click(object sender, EventArgs e)
        {
            api.PastePositionFromClipboard();
        }
    }
}
