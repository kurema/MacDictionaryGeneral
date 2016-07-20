using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacDictionaryGeneral
{
    public class PlistReader
    {
        public void Read(string path)
        {
            var dic=PlistCS.Plist.readPlist(path);
        }
    }
}
