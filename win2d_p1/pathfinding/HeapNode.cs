using System;

namespace win2d_p1
{
	class HeapNode
	{
		public float Key { get; set; }
		public PathNode Value { get; set; }
		
		public HeapNode(PathNode value) 
		{
			Key = value.F;
			Value = value;
		}
	}
}

