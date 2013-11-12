using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBOR.Tags
{
    class Base64URLTag : ItemTag
    {
        public static ulong[] TAG_NUM = new ulong[] { 33 };
        
        public Base64URLTag(ulong tagNum)
        {
            this.tagNumber = tagNum;
        }

        public override object processData(object data)
        {
            String s = (data as string);
            s = s.Replace("_", "/");
            s = s.Replace("-", "+");
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');

            byte[] decoded = System.Convert.FromBase64String(s);

            String decodedString = System.Text.Encoding.UTF8.GetString(decoded);

            return new Uri(decodedString);
        }

        public override bool isDataSupported(object data)
        {
            return (data is string);
        }
    }
}
