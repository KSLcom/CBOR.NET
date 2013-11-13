using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CBOR
{
    public static class CBOREncoderExtensions
    {
        public static byte[] ToCBOR(this ulong val)
        {
            //Major Type 0 (MajorType.UNSIGNED_INT)
            MemoryStream ms = new MemoryStream();

            byte[] header = new ItemHeader(MajorType.UNSIGNED_INT, val).ToByteArray();

            ms.Write(header, 0, header.Length);
            return ms.ToArray();
        }
        
        public static byte[] ToCBOR(this long val)
        {
            if (val >= 0)
            {
                return ((ulong)val).ToCBOR();
            }
            else
            {
                val = Math.Abs(val);
                val--;
                return ((ulong)val).ToCBOR();
            }
            //Major Type 1 (MajorType.NEGATIVE_INT)
            return null;
        }

        public static byte[] ToCBOR(this byte[] val)
        {
            //Major Type 2 (MajorType.BYTE_STRING)
            return null;
        }

        public static byte[] ToCBOR(this string val)
        {
            MemoryStream ms = new MemoryStream();

            byte[] header = new ItemHeader(MajorType.TEXT_STRING, (ulong)val.Length).ToByteArray();

            ms.Write(header, 0, header.Length);

            ms.Write(Encoding.UTF8.GetBytes(val),0, Encoding.UTF8.GetByteCount(val));
            //Major Type 3 (MajorType.TEXT_STRING)
            return ms.ToArray();
        }

        public static byte[] ToCBOR(this object[] val)
        {
            //Major Type 4 (MajorType.ARRAY)
            return null;
        }

        public static byte[] ToCBOR(this Dictionary<String,object> val)
        {
            //MajorType 5 (MajorType.MAP)
            return null;
        }

        public static byte[] ToCBOR(this List<Object> val)
        {
            //Major Type 4 (MajorType.ARRAY)
            return val.ToArray().ToCBOR();
        }

        public static byte[] ToCBOR(this ArrayList val)
        {
            //Major Type 4 (MajorType.ARRAY)
            return val.ToArray().ToCBOR();
        }

        

    }
    public static class CBOREncoder
    {
        
    }
}
