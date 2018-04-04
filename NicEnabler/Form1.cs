using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NicEnabler
{
    public partial class Form1 : Form
    {
        List<string> infList = new List<string>();
        string folderPath;
        public Form1()
        {
            InitializeComponent();
            Text += "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var objDialog = new FolderBrowserDialog
            {
                Description = "Please specify folder",
                SelectedPath = GetDownloadFolder()
            };
            DialogResult objResult = objDialog.ShowDialog(this);
            if (objResult == DialogResult.OK)
            {
                folderPath = objDialog.SelectedPath;
                GetInfs(folderPath);
                if (infList != null && infList.Count > 0)
                {
                    progressBar1.Maximum = infList.Count;
                    foreach (string infFile in infList)
                    {
                        AddToBwStack(infFile);
                    }
                }
            }
        }
        
        private void GetInfs(string folderPath)
        {
            infList.Clear();
            string[] files = Directory.GetFiles(folderPath, "*.inf", SearchOption.AllDirectories);
            infList = files.ToList();
        }

        private void AddToBwStack(string filePath)
        {
            BwFileModify mofiy = new BwFileModify(this, filePath);
            mofiy.StartWorker();
            startedWorkers++;
        }

        int startedWorkers = 0;
        int finishedWorkers = 0;
        bool ok = true;
        public void FinishedCallback(bool _ok)
        {
            finishedWorkers++;
            progressBar1.Value = finishedWorkers;
            if (!_ok)
            {
                ok = _ok;
            }
            if (startedWorkers == finishedWorkers && ok)
            {
                //Versuche Setup zu starten
                string setupFile = Path.Combine(folderPath, "Autorun.exe");
                if (File.Exists(setupFile))
                {
                    System.Diagnostics.Process.Start(setupFile);
                }
                else
                {
                    MessageBox.Show("all done, please go to step 3", "congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                progressBar1.Value = 0;
            }
            else if (startedWorkers == finishedWorkers && !ok)
            {
                MessageBox.Show("an error has occurred", "oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:plex1290@gmail.com");
            }
            catch
            {
                MessageBox.Show("No email client installed. Use webmail etc: plex1290@gmail.com", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.betterplace.org");
            }
            catch
            {
                // ignored
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://downloadcenter.intel.com/download/22283/Intel-Ethernet-Connections-CD");
            }
            catch
            {
                // ignored
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.howtogeek.com/167723/how-to-disable-driver-signature-verification-on-64-bit-windows-8.1-so-that-you-can-install-unsigned-drivers/");
            }
            catch
            {
                // ignored
            }
        }

        private string GetDownloadFolder()
        {
            string homePath = Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadPath = Path.Combine(homePath, "Downloads");
            if (Directory.Exists(downloadPath))
            {
                return downloadPath;
            }
            return homePath;
        }
    }
}
