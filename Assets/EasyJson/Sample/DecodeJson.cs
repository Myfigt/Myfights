using UnityEngine;
using System.Collections;

public class DecodeJson : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		// Json string -> Hash table
		{
			string json = "{\"name\":\"John\"}";
			Hashtable table = (Hashtable)easy.JSON.JsonDecode(json);
			Debug.Log("[DECODE]Hashtable -> Name : " + table["name"]);
		}
		// Json string -> Array list
		{
			string json = "[100, 500, 800]";
			ArrayList list = (ArrayList)easy.JSON.JsonDecode(json);
			string output = "";
			foreach(object data in list)
			{
				output += data.ToString() + "   ";
			}
			Debug.Log("[DECODE]ArrayList : " + output);
		}
		// Json string -> Objects
		{
			string json = "{\"data\":{\"birth_year\":\"1990\", \"gender\":\"man\", \"friends\":[\"John\", \"Daniel\"], \"age\":15}, \"name\":\"Josh\"}";
			Hashtable table = (Hashtable)easy.JSON.JsonDecode(json);
			Hashtable data = (Hashtable)table["data"];
			
			string output = "";
			foreach(DictionaryEntry pair in data)
			{
				output += string.Format("{0}={1}  ", pair.Key, pair.Value);
			}
			Debug.Log("[DECODE]Objects : " + output);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	}
}
