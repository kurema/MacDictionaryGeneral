using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacDictinoaryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic1=(PlistCS.DictionaryOrdered<string, object>)PlistCS.Plist.readPlist("../Info.plist");
            var t = dic1["IDXDictionaryIndexes"];
            }
    }
}
