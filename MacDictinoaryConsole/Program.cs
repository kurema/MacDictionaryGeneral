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
            var dic1=(Dictionary<string, object>)PlistCS.Plist.readPlist("../Info.plist");
            var t = (Dictionary<string, object>)((List<object>) dic1["IDXDictionaryIndexes"])[0];
            }
    }
}
