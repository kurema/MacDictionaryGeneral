using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MacDictionaryGeneral
{
    public class GeneralObjectReader
    {
        public static KeyValuePair<string, byte[]>[][][][] LoadFullEntry(Stream sr, Dictionary<string, object> info)
        {
            var result = new List<KeyValuePair<string, byte[]>[][][]>();
            while (sr.Position < sr.Length)
            {
                result.Add(LoadSingleEntry(sr, info));
            }
            return result.ToArray();
        }


        public static byte[] GetBody(Stream sr, Dictionary<string, object> info)
        {
            bool BigEndien = (bool)info["IDXIndexBigEndian"];
            var entryBinary = Functions.LoadBytes(sr, 4, BigEndien);

            int compressionType = -1;
            if (info.ContainsKey("TrieAuxiliaryDataOptions"))
            {
                var dataOption = (Dictionary<string, object>)info["TrieAuxiliaryDataOptions"];
                if (dataOption.ContainsKey("HeapDataCompressionType"))
                {
                    compressionType = (int)dataOption["HeapDataCompressionType"];
                }
            }
            else
            {
                if (info.ContainsKey("HeapDataCompressionType"))
                {
                    compressionType = (int)info["HeapDataCompressionType"];
                }
            }


            byte[] body;
            if (compressionType <= 0)
            {
                body = entryBinary;
            }
            else if (compressionType == 2)
            {
                body = Functions.Decompress(Functions.LoadBytes(new MemoryStream(entryBinary), 4, BigEndien), BigEndien);
            }
            else if (compressionType == 3)
            {
                body = Functions.Decompress(entryBinary, BigEndien);
            }
            else
            {
                throw new Exception();
            }
            return body;
        }

        public static KeyValuePair<string, byte[]>[][][] LoadSingleEntry(Stream sr, Dictionary<string, object> info)
        {
            long[] dummy;
            return LoadSingleEntry(sr, info, out dummy);
        }

        public static KeyValuePair<string, byte[]>[][][] LoadSingleEntry(Stream sr,Dictionary<string, object> info,out long[] Address)
        {
            bool BigEndien = (bool)info["IDXIndexBigEndian"];

            var tempms = new MemoryStream(GetBody(sr, info));
            tempms.Seek(0, SeekOrigin.Begin);

            bool Auxiliary = info.ContainsKey("TrieAuxiliaryDataOptions");

            if (Auxiliary)
            {
                var arrays = Functions.LoadBytesArray(tempms, 4, BigEndien,out Address);

                List<KeyValuePair<string, byte[]>[][]> result = new List<KeyValuePair<string, byte[]>[][]>();

                foreach (var item in arrays)
                {
                    List<KeyValuePair<string, byte[]>[]> result2 = new List<KeyValuePair<string, byte[]>[]>();

                    byte[][] localArrays;
                    if (info.ContainsKey("IDXIndexDataSizeLength"))
                    {
                        var ms2 = new MemoryStream(item);
                        ms2.Seek(4, SeekOrigin.Begin);
                        localArrays = Functions.LoadBytesArray(ms2, (int)info["IDXIndexDataSizeLength"], BigEndien);
                    }
                    else
                    {
                        localArrays = new byte[1][] { item };
                    }

                    foreach (var item2 in localArrays)
                    {
                        var ms = new MemoryStream(item2);
                        result2.Add(LoadDataFieldArray(ms, (Dictionary<string, object>)info["IDXIndexDataFields"], BigEndien));
                    }
                    result.Add(result2.ToArray());
                }
                return result.ToArray();
            }
            else
            {
                List<KeyValuePair<string, byte[]>[]> result2 = new List<KeyValuePair<string, byte[]>[]>();
                var addrResult = new List<long>();

                while (tempms.Position < tempms.Length)
                {
                    addrResult.Add(tempms.Position);
                    result2.Add(LoadDataFieldArray(tempms, (Dictionary<string, object>)info["IDXIndexDataFields"], BigEndien));
                }
                Address = addrResult.ToArray();
                return new KeyValuePair<string, byte[]>[1][][] { result2.ToArray() };
            }
        }

        public static KeyValuePair<string, byte[]>[] LoadDataFieldArray(Stream ms, Dictionary<string, object> dataFieldInfo,bool BigEndien)
        {
            List<KeyValuePair<string, byte[]>> temp = new List<KeyValuePair<string, byte[]>>();

            if (dataFieldInfo.ContainsKey("IDXExternalDataFields"))
            {
                temp.AddRange(LoadDataFieldSingleArray(ms, (List<object>)dataFieldInfo["IDXExternalDataFields"], BigEndien));
            }
            if (dataFieldInfo.ContainsKey("IDXFixedDataFields"))
            {
                temp.AddRange(LoadDataFieldSingleArray(ms, (List<object>)dataFieldInfo["IDXFixedDataFields"], BigEndien));
            }
            if (dataFieldInfo.ContainsKey("IDXVariableDataFields"))
            {
                temp.AddRange(LoadDataFieldSingleArray(ms, (List<object>)dataFieldInfo["IDXVariableDataFields"], BigEndien));
            }
            return temp.ToArray();
        }

        public static KeyValuePair<string, byte[]>[] LoadDataFieldSingleArray(Stream sr, List<object> info, bool BigEndien)
        {
            List<KeyValuePair<string, byte[]>> result = new List<KeyValuePair<string, byte[]>>();
            foreach (Dictionary<string, object> item in info)
            {
                result.Add(LoadDataFieldSingle(sr, item, BigEndien));
            }
            return result.ToArray();
        }

        public static KeyValuePair<string,byte[]> LoadDataFieldSingle(Stream sr, Dictionary<string, object> info,bool BigEndien)
        {
            if (info.ContainsKey("IDXDataSize"))
            {
                int length = (int)info["IDXDataSize"];
                byte[] result = new byte[length];
                sr.Read(result, 0, length);
                return new KeyValuePair<string, byte[]>((string)info["IDXDataFieldName"], result);
            }else if (info.ContainsKey("IDXDataSizeLength"))
            {
                var data= Functions.LoadBytes(sr, (int)info["IDXDataSizeLength"], BigEndien);
                return new KeyValuePair<string, byte[]>((string)info["IDXDataFieldName"], data);
            }
            throw new Exception();
        }
    }
}
