using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NicEnabler
{
    public class Helpers
    {
        public static List<string> getControlSections(string line)
        {
            List<string> tmp = new List<string>();

            string[] split = line.Split('=');
            string[] split2 = split[1].Split(',');

            for (int i = 1; i < split2.Length;i++ )
            {
                tmp.Add("[" + split2[0].Trim() + "." + split2[i].Trim() + "]");
            }
            return tmp;
        }

        public static bool ReadInfFile(string filePath, out List<string> lines)
        {
            lines = new List<string>();
            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    string line;

                    // Read the file and display it line by line.
                    file = new StreamReader(filePath);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (!(line.StartsWith(";") || string.IsNullOrEmpty(line) || line.Trim().Length == 0))
                        {
                            lines.Add(line);
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
                finally
                {
                    file?.Close();
                }
            }
            return lines.Count > 0;
        }
    }
}
