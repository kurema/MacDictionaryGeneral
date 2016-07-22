using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MacDictionaryGeneral
{
    public class MacDictionary
    {
        private Dictionary<string, object> Plist;
        public string CurrentDirectory { get; private set; }

        private Dictionary<string, object> ReferenceIndex;
        private Dictionary<string, object> KeywordIndex;
        private Dictionary<string, object> BodyDataIndex;

        private int ReferenceIndexOffset = 0x60;
        private int BodyDataOffset = 0x60;
        private int KeywordIndexOffset = 0x48;

        public MacDictionary(string PlistPath)
        {
            CurrentDirectory = System.IO.Path.GetDirectoryName(PlistPath);
            Plist = (Dictionary<string, object>)PlistCS.Plist.readPlist(PlistPath);
            foreach (Dictionary<string, object> item in ((List<object>)Plist["IDXDictionaryIndexes"]))
            {
                if ((string)item["IDXIndexName"] == "DCSKeywordIndex")
                {
                    KeywordIndex = item;
                }
                else if ((string)item["IDXIndexName"] == "DCSBodyDataIndex")
                {
                    BodyDataIndex = item;
                }
                else if ((string)item["IDXIndexName"] == "DCSReferenceIndex")
                {
                    ReferenceIndex = item;
                }
            }
        }

        public string SearchFile(string fileName)
        {
            if (File.Exists(Path.Combine(CurrentDirectory, fileName)))
            {
                return Path.Combine(CurrentDirectory, fileName);
            }
            else if (File.Exists(Path.Combine(CurrentDirectory, "Resources", fileName)))
            {
                return Path.Combine(CurrentDirectory, "Resources", fileName);
            }
            return null;
        }


        public string GetBodyDataSingle(Int32 archiveAddress,Int32 contentAddress)
        {
            var be = (bool)BodyDataIndex["IDXIndexBigEndian"];
            using(var fs=new FileStream(SearchFile((string)BodyDataIndex["IDXIndexPath"]), FileMode.Open))
            {
                long[] address;
                fs.Seek(archiveAddress, SeekOrigin.Begin);
                var result = GeneralObjectReader.LoadSingleEntry(fs, BodyDataIndex,out address);
                int i = 0;
                foreach(var addr in address)
                {
                    if (addr == contentAddress)
                    {
                        return Encoding.UTF8.GetString(result[0][i][0].Value);
                    }
                }
            }
            return "";
        }
    }
}
