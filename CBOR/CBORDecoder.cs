using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections.Generic;


namespace CBOR
{
	public class CBORDecoder
	{
		Stream buffer; 
		public CBORDecoder (Stream s)
		{
			buffer = s;
		}

		public CBORDecoder (byte[] data)
		{
			buffer = new MemoryStream(data);
		}

		public object ReadItem ()
		{
			ItemHeader header = ReadHeader ();

			switch (header.majorType) {
			case MajorType.UNSIGNED_INT:
				if (header.value == 0) {
					return header.additionalInfo;
				} else{

					return (int)header.value;
				}
			case MajorType.NEGATIVE_INT:
				if (header.value == 0) {
					return ((long)(header.additionalInfo + 1) * -1);
				} else{

					return ((long)(header.value + 1) * -1);
				}
			case MajorType.BYTE_STRING:
				ulong byteLength = header.value == 0 ? header.additionalInfo : header.value;

				byte[] bytes = new byte[byteLength];
				for (ulong x = 0; x < byteLength; x++)
				{
					bytes[x] = (byte)buffer.ReadByte();
				}

				return bytes;
			case MajorType.TEXT_STRING:
				ulong stringLength = header.value == 0 ? header.additionalInfo : header.value;

				byte[] data = new byte[stringLength];
				for (ulong x = 0; x < stringLength; x++)
				{
					data[x] = (byte)buffer.ReadByte();
				}

				return Encoding.UTF8.GetString (data);

			case MajorType.ARRAY:
				ArrayList array = new ArrayList();
				if (header.indefinite == false)
				{
					ulong elementCount = header.additionalInfo;
					if (header.value != 0) { elementCount = header.value; }

					for (ulong x = 0; x < elementCount; x++)
					{
						array.Add(ReadItem ());
					}
				} else {
					while (PeekBreak() == false)
					{
						array.Add (ReadItem ());
					}
					buffer.ReadByte();
				}

				return array;
			case MajorType.MAP:
				Dictionary<string,object> dict = new Dictionary<string, object>();

				ulong pairCount = header.value == 0 ? header.additionalInfo : header.value;
				for (ulong x = 0; x < pairCount; x++)
				{
					dict.Add((string)ReadItem (),ReadItem ());
				}

				return dict;
			case MajorType.FLOATING_POINT_OR_SIMPLE:
				if (header.additionalInfo < 24)
				{
					switch(header.additionalInfo)
					{
						case 20:
							return false;
						case 21:
							return true;
						case 22:
							return null;
						case 23:
							return null;
					}
				}

				if (header.additionalInfo == 24)
				{
					// no simple value in range 32-255 has been defined
					throw new Exception();
				}

				if (header.additionalInfo == 25)
				{
					Half halfValue = Half.ToHalf(BitConverter.GetBytes(header.value),0);

					return (float)halfValue;
				} else if (header.additionalInfo == 26)
				{
					// single (32 bit) precision float value
					return BitConverter.ToSingle(BitConverter.GetBytes(header.value),0);
				} else if (header.additionalInfo == 27)
				{
					// double (64 bit) precision float value
					return BitConverter.ToDouble(BitConverter.GetBytes(header.value),0);
				}
				// unknown simple value type
				throw new Exception();
			}

			return null;
		}

		public List<ItemTag> ReadTags ()
		{
			List<ItemTag> tags = new List<ItemTag>();

			byte b = (byte)buffer.ReadByte ();

			while (b >> 5 == 6) {
				ItemTag tag = new ItemTag();

				ulong extraInfo = (ulong)b & 0x1f;
				ulong tagNum = 0;
				if (extraInfo >= 24 && extraInfo <= 27)
				{
					tagNum = readUnsigned(1 << (b-24));

				}else {
					tagNum = extraInfo;
				}

				tag.tagNumber = tagNum;
				tags.Add(tag);
				b = (byte)buffer.ReadByte ();
			}
			buffer.Seek(-1,SeekOrigin.Current);
			return tags;
		}

		public ItemHeader ReadHeader ()
		{
			ItemHeader header = new ItemHeader ();

			header.tags = ReadTags ();

			ulong size = 0;
			byte b = (byte)buffer.ReadByte ();

			if (b == 0xFF) {
				header.breakMarker = true;
				return header;
			}

			header.majorType = (MajorType)(b >> 5);

			b &= 0x1f;
			header.additionalInfo = (ulong)b;
				if (b >= 24 && b <= 27) {
					b = (byte)(1 << (b - 24));
					header.value = readUnsigned (b);
				} else if (b > 27 && b < 31) {
					throw new Exception ();
				} else if (b == 31) {
					header.indefinite = true;
				}
			return header;
		}

		public MajorType PeekType ()
		{
			long pos = buffer.Position;
			MajorType type = ReadHeader().majorType;
			buffer.Seek(pos,SeekOrigin.Begin);
			return type;
		}

      	public bool PeekBreak()
		{
			long pos = buffer.Position;
			bool isBreak = ReadHeader().breakMarker;
			buffer.Seek(pos,SeekOrigin.Begin);
			return isBreak;
		}

		public ulong PeekSize()
		{
			long pos = buffer.Position;
			ItemHeader header = ReadHeader ();
			ulong size = header.value != 0 ? header.value : header.additionalInfo;
			buffer.Seek(pos,SeekOrigin.Begin);
			return size;
		}

		public bool PeekIndefinite()
		{
			long pos = buffer.Position;
			bool isIndefinite = ReadHeader().indefinite;
			buffer.Seek(pos,SeekOrigin.Begin);
			return isIndefinite;
		}

		private ulong readUnsigned(int size){
			byte[] buff = new byte[8];

			buffer.Read (buff,0,size);

			Array.Reverse(buff,0,size);

			return BitConverter.ToUInt64(buff,0);

		}
	}
}

