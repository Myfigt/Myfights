using System;
using System.Collections.Generic;

namespace easy
{
	public class JSONStructAttribute : Attribute
	{
		private string sName;
		private Type cTarget;
		public JSONStructAttribute (string structName) 
		{
			sName = structName;
		}
		public JSONStructAttribute(string structName, Type target) : this(structName)
		{
			cTarget = target;
		}
		
		public string Name { get { return sName; } }
		
		public Type Target { get {return cTarget;}}
		
		public override string ToString ()
		{
			return string.Format ("[JSONStructAttribute: Name={0}, Target={1}]", Name, Target);
		}	
	}
}