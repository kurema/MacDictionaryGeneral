using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleTrie
{
    public class LOUDS
    {
        public ByteOperationDictionary<int> Select8 { get {
                return _Select8 = _Select8 ?? new ByteOperationDictionary<int>(1,(a)=> {
                    return Select(a);
                });
            } }
        private ByteOperationDictionary<int> _Select8;

        public static int Select(byte[] b)
        {
            throw new NotImplementedException();
        }
    }


    public class ByteOperationDictionary<t>
    {
        private t[] Dictionary;

        public ByteOperationDictionary(int byteLength, Func<byte[], t> f)
        {
            Dictionary = new t[(int)Math.Pow(0x100, byteLength)];
            for (int i = 0; i < Math.Pow(0x100, byteLength); i++)
            {
                Dictionary[i] = f(IntToBytes(i, byteLength));
            }
        }

        public t Operate(params byte[] arg)
        {
            return Dictionary[BytesToInt(arg)];
        }

        public static byte[] IntToBytes(int arg,int length)
        {
            var result = new byte[length];
            for(int i = 0; i < length; i++)
            {
                result[i] = (byte)(arg % 0x100);
                arg = arg >> 8;
            }
            return result;
        }

        public static Int64 BytesToInt(byte[] arg)
        {
            Int64 baseNum = 1;
            Int64 result = 0;
            for(int i = 0; i < arg.Count(); i++)
            {
                result += arg[i] * baseNum;
                baseNum = baseNum << 8;
            }
            return result;
        }

        public static byte[] AddByte(byte[] b,byte a)
        {
            int i = 0;
            while (true)
            {
                int t = b[i] + a;
                if (t >= 0x100)
                {
                    a = (byte)(t >> 2);
                    b[i] = (byte)(t % 0x100);
                }
                else
                {
                    b[i] = (byte)t;
                    return b;
                }
                if (i > b.Count())
                {
                    return null;
                }
            }
        }
    }
}
