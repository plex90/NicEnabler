using System;
using System.Diagnostics;

namespace NicEnabler
{
    class Bcdedit
    {
        private static string loadoption = "DISABLE_INTEGRITY_CHECKS";

        public static bool CheckDisabled()
        {
            Process p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "bcdedit"
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            // Result
            return output.Contains(loadoption);
        }
    }
}
