using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace easy
{
	public class JSONSerialised
	{
		protected void D (string s)
		{
			//Debug.Log ("JSON: " + s);
		}
		protected void Parser (Hashtable ht)
		{
			Dictionary<string, PropertyInfo> pia = new Dictionary<string, PropertyInfo> ();
			Dictionary<string, JSONStructAttribute> jsa = new Dictionary<string, JSONStructAttribute> ();
			
			string c = this.GetType ().FullName;
			
			if( ht == null)
			{
				D(c+" ... passed null. abort.");
				return;
			}
			
			D ("Parser: ht size:" + ht.Count);
			
			
			D ("scanning properties... " + c + " / " + this.GetType ().GetProperties ().Length);
			
			foreach (PropertyInfo pi in this.GetType ().GetProperties ()) {
				string name = pi.Name;
				D ("####property: " + name);
				foreach (JSONStructAttribute sa in pi.GetCustomAttributes (typeof(JSONStructAttribute), true)) {
					D ("########struct attribute: " + sa.ToString());
					name = sa.Name;
					jsa.Add (name, sa);
				}
				
				pia.Add (name, pi);
			}
			
			D ("scanning hashtable...");
			foreach (string key in ht.Keys) {
				D ("####key:" + key);
				if (pia.ContainsKey (key)) {
					D ("########key found" + key);
					PropertyInfo pi = pia[key];
					MethodInfo mi = pi.GetSetMethod ();
					JSONStructAttribute ji = null;
					if (jsa.ContainsKey (key))
						ji = jsa[key];
					
					Type t = ht[key].GetType ();
					if (t == typeof(Hashtable)) {
						D ("############ struct");
						D ("############ struct count: "+ ((Hashtable)ht[key]).Count);
						//hashtable = new class, convention adds to property
						mi.Invoke (this, new object[] { ConvertCtor (ht[key], ji) });
					} else if (t == typeof(ArrayList)) {
						
						D ("############ array ");
						D ("############ array count "+ ((ArrayList)ht[key]).Count);
						mi = pi.GetGetMethod();
						object target = mi.Invoke(this,null);
						MethodInfo mAdd = pi.GetGetMethod().ReturnType.GetMethod ("Add");
						D("KEY:"+key);
						D("PI::"+pi.Name);
						D("PIGT:"+pi.GetGetMethod().ReturnType.Name);
						D("TGT::"+target.ToString());
						if(mAdd == null)
							throw new NotSupportedException("add method, not found.");
						int index = 0;
						foreach (object oo in (ArrayList)ht[key]) {
							object x = Convert ( oo, ji);
							D("applying "+key+" index#"+ index);
							D("adding "+x);
							D("to ... "+mAdd.ToString());
							mAdd.Invoke (target, new object[] { x });
							index++;
						}
					} else {
						D ("############ "+t.FullName);
						mi.Invoke (this, new object[] { Convert (ht[key], ji) });
						
					}
				}
			}
		}
		
		protected object Convert (object o, JSONStructAttribute ji)
		{
			D ("############ converting... "+o);
			
			Type t = o.GetType ();
			if (t == typeof(double))
				return o;
			if (t == typeof(string))
				return o;
			if (t == typeof(bool))
				return o;
			if(t == typeof(Hashtable))
				return ConvertCtor(o,ji);
			
			throw new NotSupportedException ("convert:" + o);
		}
		
		protected object ConvertCtor (object o, JSONStructAttribute ji)
		{
			D ("############ creating new object ");
			if(o == null)
				throw new NotSupportedException("passing null as object?");
			if(ji == null)
				throw new NotSupportedException("You forgot an attribute!");
			D ("############ creating new object "+ji.Target.FullName);
			ConstructorInfo ctor = ji.Target.GetConstructor (new Type[] { typeof(Hashtable) });
			if (ctor != null)
				return ctor.Invoke (new object[] { o });
			else
				throw new NotSupportedException ("ctor ");
		}
		
		protected string DDX(object o)
		{
			if(o == null)
				return "<null>\n";
			if (o.GetType () == typeof(bool))
				return DD ((bool)o);
			if (o.GetType () == typeof(double))
				return DD ((double)o);
			if (o.GetType () == typeof(string))
				return DD ((string)o);
			if (o.GetType () == typeof(ArrayList))
				return DD ((ArrayList)o);
			if (o.GetType () == typeof(Hashtable))
				return DD ((Hashtable)o);
			
			throw new NotSupportedException("eh? "+o);
		}
		protected string DD(Hashtable ht)
		{
			if(ht == null)
				return "<null>\n";
			string ret = "{ ";
			foreach(string key in ht.Keys)
			{
				ret+="\n\t["+key+"]="+DDX( ht[key]);
			}
			return ret+"\n}";
		}
		
		protected string DD(ArrayList al)
		{
			if (al == null)
				return "<null>\n";
			string ret = "[ ";
			int index =0;
			foreach (object o in al) {
				ret += "\n\t[" + index + "]=" + DDX (o);
				index++;
			}
			return ret + "\n]";
			
		}
		
		protected string DD(string s)
		{
			if(s==null)
				return "<null>\n";
			return "\""+s+"\"\n";
		}
		
		protected string DD(double d)
		{
			return d.ToString()+"\n";
		}
		
		protected string DD(bool b)
		{
			return b.ToString()+"\n";
		}
	}
}
