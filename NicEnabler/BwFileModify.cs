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
                //  Read File
                List<string> lines;
                if (!Helpers.ReadInfFile(filePath, out lines))
                {
                    return;
                }
                // Find Manufacturer Section to Find OS Speciefic Sections
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim().Equals("[Manufacturer]"))
                    {
                        listSectionNames = Helpers.getControlSections(lines[i + 1]);
                        break;
                    }
                }
                //
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
                List<string> allSectionLines = new List<string>();
                foreach (Section section in allSections)
                {
                    allSectionLines.Add(section.GetLine());
                }
                allSectionLines = allSectionLines.Distinct().ToList();


                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                {
                    int currentSectionnr = -1;

                    foreach (string line in lines)
                    {
                        if (listSectionNames.Contains(line))
                        {
                            currentSectionnr++;
                            file.WriteLine(listSectionNames[currentSectionnr]);
                            foreach (string x in allSectionLines)
                            {
                                file.WriteLine(x);
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

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            form1.FinishedCallback(ok);
        }
    }
}
