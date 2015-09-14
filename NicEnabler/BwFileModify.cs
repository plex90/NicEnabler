using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace NicEnabler
{
    class BwFileModify
    {
        readonly Form1 form1;
        readonly BackgroundWorker worker;
        readonly string filePath;
        bool ok = true;
        public BwFileModify(Form1 _form1, string _filePath)
        {
            filePath = _filePath;
            form1 = _form1;

            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public void StartWorker()
        {
            worker?.RunWorkerAsync();
        }

        public void StopWorker()
        {
            if (worker!=null && worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }

        List<string> listSectionNames = new List<string>();
        readonly List<List<Section>> listSections = new List<List<Section>>();
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Read File
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

                bool inControlFlag = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (!inControlFlag && lines[i].Trim().Equals("[ControlFlags]"))
                    {
                        inControlFlag = true;
                    }
                    //Nächste leerzeile
                    else if (lines[i].Trim().Length == 0 && inControlFlag)
                    {
                        break;
                    }
                    else if (inControlFlag)
                    {
                        lines[i] = ";" + lines[i];
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

                // ### Alle validen Elemente in 'allSections' schreiben
                List<Section> allSections = listSections.SelectMany(listSec => listSec).ToList();
                // ### Mergen --- Doppelte vermeiden
                foreach (List<Section> t in listSections)
                {
                    foreach (Section sec in allSections)
                    {
                        if (!t.Contains(sec))
                        {
                            t.Add(sec);
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
