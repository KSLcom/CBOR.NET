using System;
using System.IO;
using CBOR;
using CBOR.Tags;
using System.Web;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using System.Xml;
namespace CBOR_Test
{
    [TestFixture]
    public class DecoderTests
    {
        public static void Main(String[] args)
        {
            
        }
        [Test]
        public void Base64URLDecode()
        {
            byte[] data = new byte[] { 0xD8, 0x21, 0x78, 0x1E, 0x61, 0x48, 0x52, 0x30, 0x63, 0x44, 0x6F, 0x76, 0x4C, 0x33, 0x64, 0x33, 0x64, 0x79, 0x35, 0x6C, 0x65, 0x47, 0x46, 0x74, 0x63, 0x47, 0x78, 0x6C, 0x4C, 0x6D, 0x4E, 0x76, 0x62, 0x51 };
            CBORDecoder decoder = new CBORDecoder(data);
            var f = decoder.ReadItem();
            Assert.IsInstanceOf<Uri>(f);
        }

        [Test]
        public void CBORItemEmbed()
        {
            byte[] data = new byte[] { 0xd8, 0x18, 0x45, 0x64, 0x49, 0x45, 0x54, 0x46 };
            CBORDecoder decoder = new CBORDecoder(data);
            var f = decoder.ReadItem();
            Assert.IsInstanceOf<String>(f);
        }
        [Test]
        public void URIDecode()
        {
            byte[] data = new byte[] { 0xd8, 0x20, 0x76, 0x68, 0x74, 0x74, 0x70, 0x3a, 0x2f, 0x2f, 0x77, 0x77, 0x77, 0x2e, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f, 0x6d };
            CBORDecoder decoder = new CBORDecoder(data);
            var f = decoder.ReadItem();
            Assert.IsInstanceOf<Uri>(f);
        }

        [Test]
        public void IndefiniteArray()
        {
            //[_ -100, -100 ]
			byte[] data = new byte[] {0x9F,0x38, 0x63,0x38, 0x63,0xFF};
			CBORDecoder decoder = new CBORDecoder(data);

			var f = decoder.ReadItem();

			Assert.IsInstanceOf(typeof(ArrayList),f);

			ArrayList list = (ArrayList)f;

			Assert.AreEqual(2,list.Count);

			Assert.AreEqual(-100,list[0]);

			Assert.AreEqual(list[0],list[1]);
        }

		[Test]
		public void SmallArray ()
		{
			// [1,2]
			byte[] data = new byte[] {0x82, 0x01, 0x02};
			CBORDecoder decoder = new CBORDecoder(data);

			var f = decoder.ReadItem();

			Assert.IsInstanceOf<ArrayList>(f);
		}

		[Test]
		public void SmallPositiveInteger ()
		{
            // 08
			byte[] data = new byte[]{0x08};
			CBORDecoder decoder = new CBORDecoder (data);

			var f = decoder.ReadItem ();

			Assert.AreEqual(8,f);
		}

		[Test]
		public void SmallMap()
		{
			// {"a":1,"b":"c"}
			byte[] data = new byte[] {0xa2, 0x61, 0x61, 0x01, 0x61, 0x62,0x61,0x63};
			CBORDecoder decoder = new CBORDecoder(data);

			var f = decoder.ReadItem();

			Assert.IsInstanceOf<Dictionary<String,object>>(f);

			Dictionary<String,object> map = (Dictionary<String,object>)f;

			Assert.AreEqual(2,map.Keys.Count);

			f = null;
		}

		[Test]
		public void NestedArray ()
		{
			// [[1],[2,[3,4]]]
			byte[] data = new byte[] {0x82, 0x81, 0x01, 0x82, 0x02, 0x82, 0x03, 0x04};
			CBORDecoder decoder = new CBORDecoder(data);

			var f = decoder.ReadItem();
			Assert.IsInstanceOf<ArrayList>(f);

			ArrayList list1 = (ArrayList)f;
			Assert.IsInstanceOf<ArrayList>(list1[0]);
			Assert.IsInstanceOf<ArrayList>(list1[1]);

			ArrayList list2 = (ArrayList)(list1[1]);

			Assert.IsInstanceOf<ArrayList>(list2[1]);

		}

		[Test]
		public void Comprehensive() {
			//{"a": 1, "b": [2, true,false,{"a":`01,02,03,04`,"b":[1.5f]}]}  
			byte[] data = new byte[] {0xd9,0xd9,0xf7,0xa2, 0x61, 0x61, 0x01, 0x61, 0x62, 0x84, 0x02,0xF5,0xF4,0xa2,0x61, 0x61,0x44,0x01,0x02,0x03,0x04,0x61, 0x62,0x81,0xf9,0x3e,0x00};

            CBORDecoder decoder = new CBORDecoder(data);

            var f = decoder.ReadItem();

			// 1.5f
			//byte[] data = new byte[] {0xf9,0x3e,0x00};


		}
        [Test]
		public void BigInteger() {
			//-18446744073709551617 (BigInt (tags))
			byte[] data = new byte[]{0xc2,0x49,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
            CBORDecoder decoder = new CBORDecoder(data);

            var f = decoder.ReadItem();

            Console.WriteLine(f);

            data = new byte[] { 0xc3, 0x49, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            decoder = new CBORDecoder(data);

            f = decoder.ReadItem();
            Console.WriteLine(f);

            Assert.IsInstanceOf<BigInteger>(f);
		}

        [Test]
        public void DateTime()
        {
            byte[] data = new byte[] { 0xc1, 0x1a, 0x51, 0x4b, 0x67, 0xb0 };
            CBORDecoder decoder = new CBORDecoder(data);
            var f = decoder.ReadItem();

            Console.WriteLine(f);

            data = new byte[] { 0xc1, 0xfb, 0x41, 0xd4, 0x52, 0xd9, 0xec, 0x20, 0x00, 0x00 };
            decoder = new CBORDecoder(data);
            f = decoder.ReadItem();

            Console.WriteLine(f);
        }

        [Test]
        public void TagTest()
        {

            ItemTag tag = TagRegistry.getTagInstance(2);

            Assert.IsInstanceOf<BigIntegerTag>(tag);

            tag = TagRegistry.getTagInstance(3);

            Assert.IsInstanceOf<BigIntegerTag>(tag);

            tag = TagRegistry.getTagInstance(999999999);
            Assert.IsInstanceOf<UnknownTag>(tag);
        }
    }

}