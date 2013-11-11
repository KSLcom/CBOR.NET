using System;
using System.IO;
using CBOR;
using System.Web;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

namespace CBOR_Test
{
	public class DummyClass {
		public static void Main(String[] args)
		{

		}

	}
    [TestFixture]
    public class DecoderTest
    {
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

    }

}