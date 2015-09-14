using System.Collections.Generic;

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
    }
}
