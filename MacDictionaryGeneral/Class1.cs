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
        public static Dictionary<string,string> LoadSingleEntry(Stream sr,Dictionary<string, object> info)
        {
            bool BigEndien = (bool)info["IDXIndexBigEndian"];
            var entryBinary = Functions.LoadBytes(sr, 4, BigEndien);

            var sr2 = new MemoryStream(entryBinary);
            if (info.ContainsKey("TrieAuxiliaryDataOptions"))
            {
                var dataOption = (Dictionary<string, object>)info["TrieAuxiliaryDataOptions"];
            }    
        }

        public interface IEntry
        {
            byte[] Content { get; }
            long Address { get; }

            string ToString(System.Text.Encoding enc);
        }

        public class Entry:IEntry
        {
            public byte[] Content { get; private set; }
            public long Address { get; private set; }

            public Entry(Stream sr, bool BigEndien)
            {
                Address = sr.Position;

                byte[] bytes = new byte[4];

                sr.Read(bytes, 0, 4);
                int rawLen = Functions.UnpackInt(bytes, BigEndien);

                var data = new byte[rawLen];
                sr.Read(data, 0, rawLen);
                this.Content = data;
            }

            public static Entry[] GetEntries(Stream sr, bool BigEndien)
            {
                List<Entry> result = new List<Entry>();
                while (true)
                {
                    result.Add(new Entry(sr, BigEndien));
                    if (sr.Position >= sr.Length) break;
                }
                return result.ToArray();
            }

            public string ToString(Encoding enc)
            {
                return enc.GetString(Content);
            }
        }

        public class EntryCompressed:IEntry
        {
            public byte[] Content { get { return _Content == null ? _Content = Functions.Decompress(Compressed, RawLength) : _Content; } }

            public long Address { get; private set; }

            private byte[] _Content = null;
            private byte[] Compressed;
            private int RawLength;

            public EntryCompressed(Stream sr,bool BigEndien)
            {
                Address = sr.Position;

            }

            public static EntryCompressed[] GetEntries(Stream sr, bool BigEndien)
            {
                List<EntryCompressed> result = new List<EntryCompressed>();
                while (true)
                {
                    result.Add(new EntryCompressed(sr, BigEndien));
                    if (sr.Position >= sr.Length) break;
                }
                return result.ToArray();
            }

            public string ToString(Encoding enc)
            {
                return enc.GetString(Content);
            }
        }
    }

}
