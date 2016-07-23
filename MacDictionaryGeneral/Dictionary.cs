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


        public KeyValuePair<string, byte[]>[][] GetKeyword(bool[] target, string keyword, Func<string, string, bool> f)
        {
            return GetKeyword(target, keyword, (a, b, c) => f(Encoding.Unicode.GetString(a), c));
        }

        public KeyValuePair<string,byte[]>[][] GetKeyword(bool[] target,string keyword,Func<byte[],byte[],string,bool> f)
        {
            var result = new List<KeyValuePair<string, byte[]>[]>();
            var be = (bool)KeywordIndex["IDXIndexBigEndian"];
            byte[] keywordBytes = Encoding.Unicode.GetBytes(keyword);
            using (var fs = new FileStream(GeneralObjectReader.GetAuxiliaryPath(KeywordIndex,CurrentDirectory), FileMode.Open))
            {
                fs.Seek(0x48, SeekOrigin.Begin);
                var idx= GeneralObjectReader.LoadFullEntry(fs, KeywordIndex);
                foreach(var item1 in idx)
                {
                    foreach (var item2 in item1)
                    {
                        foreach (var item3 in item2)
                        {
                            for(int i = 2; i < item3.Count(); i++)
                            {
                                if (target.Count() > i - 2 && target[i - 2] && f(item3[i].Value, keywordBytes, keyword))
                                {
                                    result.Add(item3);
                                    break;
                                }
                            }
                        }
                    }
                }
                return result.ToArray();
            }
        }

        public string[] GetBodyDataArray(KeyValuePair<string, byte[]>[][] keywords)
        {
            var result = new List<string>();
            foreach(var keyword in keywords)
            {
                var text=(GetBodyDataSingle(keyword));
                if (text != "" && !result.Contains(text)) result.Add(text);
            }
            return result.ToArray();
        }


        public string GetFullHtml(string[] source)
        {
            string style = GeneralObjectReader.SearchFile((string)Plist["DCSDictionaryCSS"], CurrentDirectory);
            style = "file:///" + style.Replace('\\','/');

            StringBuilder htmlB = new StringBuilder();
            htmlB.Append("<!DOCTYPE html>\n<html><head><meta http-equiv='content-style-type' content='text/css' /><link rel='stylesheet' type='text/css' href='"+style+"' /></head><body>");
            foreach(var s in source)
            {
                htmlB.Append(s);
            }
            htmlB.Append("</body></html>");
            return htmlB.ToString();
        }


        public string GetBodyDataSingle(KeyValuePair<string, byte[]>[] keyword)
        {
            return GetBodyDataSingle(Functions.GetEntry(keyword, "DCSExternalBodyID").Value);
        }

        public string GetBodyDataSingle(byte[] keyword)
        {
            var be = (bool)BodyDataIndex["IDXIndexBigEndian"];
            if (keyword.Length == 8)
            {
                var temp = new byte[4];
                Array.Copy(keyword, 4, temp, 0, 4);
                var archiveAddress = Functions.UnpackInt(temp, be)+0x40;
                Array.Copy(keyword, temp, 4);
                var contentAddress = Functions.UnpackInt(temp, be);
                return GetBodyDataSingle(archiveAddress, contentAddress);
            }
            throw new NotImplementedException();
        }

        public string GetBodyDataSingle(Int32 archiveAddress,Int32 contentAddress)
        {
            var be = (bool)BodyDataIndex["IDXIndexBigEndian"];
            using(var fs=new FileStream(GeneralObjectReader.GetIndexPath(BodyDataIndex,CurrentDirectory), FileMode.Open))
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
                    i++;
                }
            }
            return "";
        }
    }
}
