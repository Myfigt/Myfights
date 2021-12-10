using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebServicesManager : MonoBehaviour {
	#region Constants
	public const string baseURL = "http://44.198.255.36/demo/api/";
	public const string authorization = "Basic cm9vdDpyb290MTIzIw==";
	public const string contentType = "application/json";
	public string deviceID;
	public string platform;
	#endregion

	#region Initializers
	private static WebServicesManager instance = null;
	public static WebServicesManager Instance{
		get{
			if (instance == null) {
				StartJPWebServicesManager ();
				return instance;
			} else {
				return instance;
			}
		}
	}

	private static void StartJPWebServicesManager(){
		GameObject WebServices = new GameObject ();
		DontDestroyOnLoad (WebServices);
		instance = WebServices.AddComponent <WebServicesManager>();
		instance.deviceID = SystemInfo.deviceUniqueIdentifier;
		if(Application.platform == RuntimePlatform.IPhonePlayer){
			instance.platform = "iOS";
		}
		else if(Application.platform == RuntimePlatform.Android){
			instance.platform = "Android";
		}
		else{
			instance.platform = "Unity";
		}
	}
	#endregion

	#region Register User API
	public delegate void OnRegisterUserComplete (string responce);
	public delegate void OnRegisterUserFailed (string error);

	public static event OnRegisterUserComplete registerUserComplete;
	public static event OnRegisterUserFailed registerUserFailed;

	public void ResgisterUser(string _email,string _userName,string _password)
    {
		StartCoroutine (_ResgisterUser(_email,_userName,_password));
	}

	IEnumerator _ResgisterUser(string _email,string _userName,string _password)
    {
		string url = baseURL+ "signup";

		WWWForm data = new WWWForm();
		data.AddField ("first_name", _userName);
        data.AddField("last_name", "zia");
        data.AddField("email",_email);
		data.AddField("password",_password);
		data.AddField("platform",platform);
		data.AddField("deviceId",deviceID);

		//string JSONStr = easy.JSON.JsonEncode(data);
		//Debug.Log ("RegisterGuest JSON QUERY PARAMETERS = " + JSONStr);
		//byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		//Dictionary<string,string> headers = new Dictionary<string, string>();
		//headers.Add("Content-Type", contentType);
		////headers.Add("Authorization", authorization);
		//headers.Add("Accept", contentType);

		WWW www = new WWW (url, data);
		yield return www;

		if (!string.IsNullOrEmpty(www.error))
        {
			Debug.LogError("ResgisterUser Error" + www.error);
			if(registerUserFailed != null)
            {
				registerUserFailed (www.error);
			}
		}
		else
        {
			Debug.Log ("ResgisterUser RESPONCE = " + www.text);
			if(registerUserComplete != null)
            {
				registerUserComplete (www.text);
			}
		}

		yield return null;
	}
    #endregion
  
    #region Login User API
    public delegate void OnLoginUserComplete (string responce);
	public delegate void OnLoginUserFailed (string error);
	public static event OnLoginUserComplete loginUserComplete;
	public static event OnLoginUserFailed loginUserFailed;

	public void LoginUser(string _email,string _password,bool _override = false){
		StartCoroutine (_LoginUser(_email,_password,_override));
	}

	IEnumerator _LoginUser(string _email,string _password,bool _override){
		string url = baseURL+"login";

        WWWForm data = new WWWForm();
        data.AddField("email", _email);
        data.AddField("password", _password);
        data.AddField("platform", platform);
        data.AddField("deviceId", deviceID);
        //override flag for multi device login
        _override = true;

		if(_override){
			data.AddField ("override",_override.ToString());
		}

        //string JSONStr = easy.JSON.JsonEncode(data);
        //Debug.Log ("Login JSON QUERY PARAMETERS = " + JSONStr);
        //byte[] pData = Encoding.ASCII.GetBytes(JSONStr);
       // Hashtable _headers = new Hashtable();
  		//Dictionary<string,string> headers = new Dictionary<string, string>();
	//	_headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
	//	_headers.Add("Accept", contentType);

		WWW www = new WWW (url, data);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("Login Error" + www.error);
			if(loginUserFailed != null){
				loginUserFailed (www.error);
			}
		}
		else{
			Debug.Log ("Login RESPONCE = " + www.text);
			if(loginUserComplete != null){
				loginUserComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion
}
