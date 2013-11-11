using System;
using System.Collections;
using System.Collections.Generic;


namespace CBOR
{
	public class ItemHeader
	{
		public List<ItemTag> tags = new List<ItemTag>();
		public MajorType majorType { get; set; }
		public ulong additionalInfo {get;set;}
		public ulong value {get;set;}
		public bool indefinite {get;set;}
		public bool breakMarker {get;set;}
	}

	public class ItemTag
	{
		public ulong tagNumber;
	}
}

