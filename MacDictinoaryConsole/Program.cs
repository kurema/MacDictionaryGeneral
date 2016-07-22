using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using MacDictionaryGeneral;

namespace MacDictinoaryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic1 = (Dictionary<string, object>)PlistCS.Plist.readPlist("../Info.plist");

            using (var fs = new FileStream("../KeyText.data", FileMode.Open))
            {
                var t = (Dictionary<string, object>)((List<object>)dic1["IDXDictionaryIndexes"])[0];
                fs.Seek(0x48, SeekOrigin.Begin);
                var result = MacDictionaryGeneral.GeneralObjectReader.LoadSingleEntry(fs, t);
            }
            using (var fs = new FileStream("../Body.data", FileMode.Open))
            {
                var t = (Dictionary<string, object>)((List<object>)dic1["IDXDictionaryIndexes"])[2];
                fs.Seek(0x60, SeekOrigin.Begin);
                long[] Addr;
                var result = MacDictionaryGeneral.GeneralObjectReader.LoadSingleEntry(fs, t, out Addr);
            }
            //{
            //    var dic = new MacDictionaryGeneral.MacDictionary(@"");
            //    var text = dic.GetBodyDataSingle(0x60, 0);
            //}
            using (var fs = new FileStream("../EntryID.index", FileMode.Open))
            {
                fs.Seek(0x2040, SeekOrigin.Begin);
                long[] addr;
                var array = Functions.EncodeArray(Functions.LoadBytesArray(fs, 2, false, out addr, false), System.Text.Encoding.Unicode);
            }
        }
    }
}
