﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MacDictionaryGeneral
{
    public static class Functions
    {
        public static int UnpackInt(byte[] bytes,bool BigEndien)
        {
            if (BitConverter.IsLittleEndian == BigEndien)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint UnpackUInt(byte[] bytes, bool BigEndien)
        {
            if (BitConverter.IsLittleEndian == BigEndien)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static short UnpackShort(byte[] bytes, bool BigEndien)
        {
            if (BitConverter.IsLittleEndian == BigEndien)
                Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort UnpackUShort(byte[] bytes, bool BigEndien)
        {
            if (BitConverter.IsLittleEndian == BigEndien)
                Array.Reverse(bytes);

            return BitConverter.ToUInt16(bytes, 0);
        }
        public static byte[][] LoadBytesArray(Stream sr, int SizeLength, bool BigEndien,bool IncludeSelfCount=false)
        {
            long[] dummy;
            return LoadBytesArray(sr, SizeLength, BigEndien,out dummy,IncludeSelfCount);
        }

        public static byte[][] LoadBytesArray(Stream sr, int SizeLength, bool BigEndien,out long[] Address, bool IncludeSelfCount = false)
        {
            List<byte[]> result = new List<byte[]>();
            var addrResult = new List<long>();
            while (sr.Position < sr.Length) {
                addrResult.Add(sr.Position);
                result.Add(LoadBytes(sr, SizeLength, BigEndien, IncludeSelfCount));
            }
            Address = addrResult.ToArray();
            return result.ToArray();
        }

        public static string[] EncodeArray(byte[][] data,System.Text.Encoding enc)
        {
            var result = new string[data.GetLength(0)];
            for(int i = 0; i < data.Length; i++)
            {
                result[i] = enc.GetString(data[i]);
            }
            return result;
        }


        public static byte[] LoadBytes(Stream sr,int SizeLength,bool BigEndien, bool IncludeSelfCount = false)
        {
            Int64 Size = 0;
            Int64 Base = 1;
            for(int i = 0; i < SizeLength; i++)
            {
                int temp = sr.ReadByte();
                if (BigEndien)
                {
                    Size = Size * 0x100 + temp;
                }
                else
                {
                    Size += Base * temp;
                    Base *= 0x100;
                }
            }
            if (Size > int.MaxValue) { throw new Exception(); }
            if (IncludeSelfCount) { Size -= SizeLength; }
            var result = new byte[Size];
            sr.Read(result, 0, (int)Size);
            return result;
        }

        public static byte[] Decompress(Stream sr, bool BigEndien)
        {
            byte[] bytes = new byte[4];

            sr.Read(bytes, 0, 4);
            int compdLen = Functions.UnpackInt(bytes, BigEndien);
            var data = new byte[compdLen - 4];
            sr.Read(data, 0, compdLen - 4);

            return Decompress(data, BigEndien);
        }

        public static byte[] Decompress(byte[] Origin, bool BigEndien)
        {
            var stream = new MemoryStream(Origin);

            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            int rawLen = Functions.UnpackInt(bytes, BigEndien);

            stream.Seek(2, SeekOrigin.Current);

            using (var ds = new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress))
            {
                var result = new byte[rawLen];
                ds.Read(result, 0, rawLen);
                return result;
            }
        }

        public static KeyValuePair<string, byte[]> GetEntry(KeyValuePair<string, byte[]>[] list,string key)
        {
            foreach(var item in list)
            {
                if (item.Key == key) { return item; }
            }
            throw new KeyNotFoundException();
        }
    }
}
