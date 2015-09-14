using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NicEnabler
{
    public partial class Form1 : Form
    {
        readonly List<string> infList = new List<string>();
        string folderPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var objDialog = new FolderBrowserDialog
            {
                Description = "Please specify folder",
                SelectedPath = Environment.GetEnvironmentVariable("USERPROFILE")
            };
            DialogResult objResult = objDialog.ShowDialog(this);
            if (objResult == DialogResult.OK)
            {
                folderPath = objDialog.SelectedPath;
            }
            GetInfs(folderPath);
            if (infList != null && infList.Count > 0)
            {
                foreach (string infFile in infList)
                {
                    AddToBwStack(infFile);
                }
            }
        }
        
        private void GetInfs(string folderPath)
        {
            //Find and Add inf files to stack
            try
            {
                foreach (string d in Directory.GetDirectories(folderPath))
                {
                    foreach (string f in Directory.GetFiles(d, "*.inf"))
                    {
                        infList.Add(f);
                    }
                    GetInfs(d);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void AddToBwStack(string filePath)
        {
            BwFileModify mofiy = new BwFileModify(this,filePath);
            mofiy.StartWorker();
            startedWorkers++;
        }

        int startedWorkers = 0;
        int finishedWorkers = 0;
        bool ok = true;
        public void FinishedCallback(bool _ok)
        {
            finishedWorkers++;
            if (!_ok)
            {
                ok = _ok;
            }
            if (startedWorkers==finishedWorkers&&ok)
            {
                //Versuche Setup zu starten
                string setupFile = Path.Combine(folderPath, "Autorun.exe");
                try
                {
                    System.Diagnostics.Process.Start(setupFile);
                }
                catch
                {
                    MessageBox.Show("all done, please go to step 3", "congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
                System.Diagnostics.Process.Start("mailto:plex94180@gmail.com");
            }
            catch
            {
                MessageBox.Show("No email client installed. Use webmail etc: plex94180@gmail.com", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
