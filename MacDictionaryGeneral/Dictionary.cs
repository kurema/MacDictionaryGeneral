using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacDictionaryGeneral
{
    public class Dictionary
    {
        private Dictionary<string, object> Plist;
        public string CurrentDirectory { get; private set; }

        public Dictionary(string PlistPath)
        {
            CurrentDirectory = System.IO.Path.GetDirectoryName(PlistPath);
            Plist = (Dictionary<string, object>)PlistCS.Plist.readPlist(PlistPath);
        }

    }
}
