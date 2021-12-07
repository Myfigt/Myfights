using UnityEngine;
using System.Collections;

public class EncodeJson : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		// Hash table -> Json string.
		{
			Hashtable table = new Hashtable();
			table.Add("name", "John");
			string json = easy.JSON.JsonEncode(table);
			Debug.Log("[ENCODE]Hashtable : " + json);
		}
		
		// ArrayList -> Json string.
		{
			ArrayList list = new ArrayList();
			list.Add(100);
			list.Add(500);
			list.Add(800);
			string json = easy.JSON.JsonEncode(list);
			Debug.Log("[ENCODE]ArrayList : " + json);
		}
		
		// Hashtable & object -> Json string.
		{
			Hashtable table = new Hashtable();
			table.Add("name", "Josh");
			{
				Hashtable data = new Hashtable();
				data.Add("age", 15);
				data.Add("gender", "man");
				data.Add("birth_year", "1990");
				{
					ArrayList list = new ArrayList();
					list.Add("John");
					list.Add("Daniel");
					data.Add("friends", list);
				}
				table.Add("data", data);
			}
			string json = easy.JSON.JsonEncode(table);
			Debug.Log("[ENCODE]Object : " + json);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}
}
