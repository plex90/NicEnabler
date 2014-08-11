using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NicEnabler
{
    public class Section
    { 
        public string displayname {get;set;}
        public string section { get; set; }
        public string deviceid { get; set; }
        public Section(string line)
        {
            //MessageBox.Show(line);
            try
            {
                string[] split = line.Split('=');
                displayname = split[0].Trim();
                string[] split2 = split[1].Split(',');
                section = "=" + split2[0].Trim()+",";
                deviceid = split2[1].Trim();
            }
            catch (Exception ex)
            { 
            }
        }

        public bool Valid()
        {
            return (displayname != null && section != null && deviceid != null);
        }
    }
}
