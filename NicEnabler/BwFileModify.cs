using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NicEnabler
{
    class BwFileModify
    {
        Form1 form1;
        BackgroundWorker worker;
        string filePath;
        bool ok = true;
        public BwFileModify(Form1 _form1, string _filePath)
        {
            this.filePath = _filePath;
            this.form1 = _form1;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        public void StartWorker()
        {
            if (this.worker!=null)
            {
                this.worker.RunWorkerAsync();
            }
        }

        public void StopWorker()
        {
            if (this.worker!=null&&worker.IsBusy)
            {
                this.worker.CancelAsync();
            }
        }

        List<string> listSectionNames = new List<string>();
        List<List<Section>> listSections = new List<List<Section>>();
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Datei auslesen
                List<string> lines = new List<string>();
                using (System.IO.StreamReader file = new System.IO.StreamReader(filePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                //
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim().Equals("[Manufacturer]"))
                    {
                        listSectionNames = Helpers.getControlSections(lines[i + 1]);
                        break;
                    }
                }

                foreach (string fullsectionname in listSectionNames)
                {
                    bool inSection = false;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Trim().ToLower().Equals(fullsectionname.ToLower()) && !inSection)
                        {
                            inSection = true;
                            listSections.Add(new List<Section>());
                            i++;
                        }

                        if (inSection)
                        {
                            Section x = new Section(lines[i]);
                            if (x.Valid())
                            {
                                lines[i] = string.Empty;
                                listSections[listSections.Count - 1].Add(x);
                            }
                            else if (lines[i].StartsWith("["))
                            {
                                break;
                            }
                        }
                    }
                }

                List<Section> allSections = new List<Section>();
                foreach (List<Section> listSec in listSections)
                {
                    foreach (Section sec in listSec)
                    {
                        allSections.Add(sec);
                    }
                }

                //Mergen
                for (int i = 0; i < listSections.Count; i++)
                {
                    foreach (Section sec in allSections)
                    {
                        if (!listSections[i].Contains(sec))
                        {
                            listSections[i].Add(sec);
                        }
                    }
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.IO.Path.Combine(filePath)))
                {
                    int currentSectionnr = -1;

                    foreach (string line in lines)
                    {
                        if (listSectionNames.Contains(line))
                        {
                            currentSectionnr++;
                            file.WriteLine(listSectionNames[currentSectionnr]);
                            foreach (Section x in listSections[currentSectionnr])
                            {
                                file.WriteLine(x.displayname + x.section + x.deviceid);
                            }
                        }
                        else
                        {
                            if (line.Length > 0)
                            {
                                file.WriteLine(line);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ok = false;
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            form1.FinishedCallback(ok);
        }
    }
}
