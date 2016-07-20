using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MacDictinoaryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic1 = (Dictionary<string, object>)PlistCS.Plist.readPlist("../Info.plist");
            var t = (Dictionary<string, object>)((List<object>)dic1["IDXDictionaryIndexes"])[2];

            //using (var fs = new FileStream("../KeyText.data", FileMode.Open))
            //{
            //    fs.Seek(0x48, SeekOrigin.Begin);
            //    var result=  MacDictionaryGeneral.GeneralObjectReader.LoadSingleEntry(fs, t, true);
            //}
            using (var fs = new FileStream("../Body.data", FileMode.Open))
            {
                fs.Seek(0x60, SeekOrigin.Begin);
                var result = MacDictionaryGeneral.GeneralObjectReader.LoadSingleEntry(fs, t, false);
            }
        }
    }
}
