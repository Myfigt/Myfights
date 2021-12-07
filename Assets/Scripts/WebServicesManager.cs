using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebServicesManager : MonoBehaviour {
	#region Constants
	public const string baseURL = "http://44.198.255.36/demo/api/";//"http://136.243.148.174:8090/";
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
		GameObject tournamentWebServices = new GameObject ();
		DontDestroyOnLoad (tournamentWebServices);
		instance = tournamentWebServices.AddComponent <WebServicesManager>();
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
		data.AddField("email",_email);
		data.AddField("password",_password);
		data.AddField("platform",platform);
		data.AddField("deviceId",deviceID);

		string JSONStr = easy.JSON.JsonEncode(data);
		Debug.Log ("RegisterGuest JSON QUERY PARAMETERS = " + JSONStr);
		byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
		headers.Add("Accept", contentType);

		WWW www = new WWW (url, pData, headers);
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
    /*
   #region Register Guest API
   public delegate void OnRegisterGuestComplete(string responce);
   public delegate void OnRegisterGuestFailed(string error);

   public static event OnRegisterGuestComplete registerGuestComplete;
   public static event OnRegisterGuestFailed registerGuestFailed;

   public void RegisterGuest(string _email, string _userName, string _password)
   {
       StartCoroutine(_RegisterGuest(_email, _userName, _password));
   }

   IEnumerator _RegisterGuest(string _email, string _userName, string _password)
   {
       string url = baseURL + "jpws_v2/user/register";

       Hashtable data = new Hashtable();
       data.Add("name", _userName);
       data.Add("email", _email);
       data.Add("password", _password);
       data.Add("platform", platform);
       data.Add("deviceId", deviceID);

       string JSONStr = easy.JSON.JsonEncode(data);
       Debug.Log("ResgisterUser JSON QUERY PARAMETERS = " + JSONStr);
       byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

       Dictionary<string, string> headers = new Dictionary<string, string>();
       headers.Add("Content-Type", contentType);
       //headers.Add("Authorization", authorization);
       headers.Add("Accept", contentType);

       WWW www = new WWW(url, pData, headers);
       yield return www;

       if (!string.IsNullOrEmpty(www.error))
       {
           Debug.LogError("ResgisterGuest Error" + www.error);
           if (registerGuestFailed != null)
           {
               registerGuestFailed(www.error);
           }
       }
       else
       {
           Debug.Log("ResgisterGuest RESPONCE = " + www.text);
           if (registerGuestComplete != null)
           {
               registerGuestComplete(www.text);
           }
       }

       yield return null;
   }
   #endregion


   #region Social User API
   public delegate void OnSocialRegisterUserComplete (string responce);
   public delegate void OnSocialRegisterUserFailed (string error);

   public static event OnSocialRegisterUserComplete socialRegisterUserComplete;
   public static event OnSocialRegisterUserFailed socialRegisterUserFailed;

   public void SocialResgisterUser(string _email,string _userName,string _password,string contact,string id,string source){
       StartCoroutine (_SocialResgisterUser( _email, _userName, _password, contact,  id, source));
   }

   IEnumerator _SocialResgisterUser(string _email,string _userName,string _password,string contact, string id,string source){
       string url = baseURL+"jpws_v2/user/login/social";

       Hashtable data = new Hashtable ();
       data.Add ("name",_userName);
       data.Add ("email",_email);
       data.Add ("password",_password);
       data.Add ("platform",platform);
       data.Add ("deviceId",deviceID);
       data.Add ("socialId",id);
       data.Add ("socialName",_userName);
       data.Add ("socialContact",contact);
       data.Add ("socialSource",source);

       //override flag for multi device login
       data.Add ("override",true);

       string JSONStr = easy.JSON.JsonEncode(data);
       Debug.LogError ("socialRegisterUserComplete JSON QUERY PARAMETERS = " + JSONStr);
       byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

       Dictionary<string,string> headers = new Dictionary<string, string>();
       headers.Add("Content-Type", contentType);
       //headers.Add("Authorization", authorization);
       headers.Add("Accept", contentType);

       WWW www = new WWW (url, pData, headers);
       yield return www;

       if (!string.IsNullOrEmpty(www.error)){
           Debug.LogError("socialRegisterUserComplete Error" + www.error);
           if(socialRegisterUserFailed != null){
               socialRegisterUserFailed (www.error);
           }
       }
       else{
           Debug.Log ("socialRegisterUserComplete RESPONCE = " + www.text);
           if(socialRegisterUserComplete != null){
               socialRegisterUserComplete (www.text);
           }
       }

       yield return null;
   }
   #endregion
*/
    #region Login User API
    public delegate void OnLoginUserComplete (string responce);
	public delegate void OnLoginUserFailed (string error);
	public static event OnLoginUserComplete loginUserComplete;
	public static event OnLoginUserFailed loginUserFailed;

	public void LoginUser(string _email,string _password,bool _override){
		StartCoroutine (_LoginUser(_email,_password,_override));
	}

	IEnumerator _LoginUser(string _email,string _password,bool _override){
		string url = baseURL+"login";
        
		Hashtable data = new Hashtable ();
		data.Add ("email",_email);
		data.Add ("password",_password);
		data.Add ("platform",platform);
		data.Add ("deviceId",deviceID);
		//override flag for multi device login
		_override = true;

		if(_override){
			data.Add ("override",_override);
		}

		string JSONStr = easy.JSON.JsonEncode(data);
		Debug.Log ("Login JSON QUERY PARAMETERS = " + JSONStr);
		byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
		headers.Add("Accept", contentType);

		WWW www = new WWW (url, pData, headers);
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
    /*
    #region Login User API
    public delegate void OnLoginGuestComplete(string responce);
    public delegate void OnLoginGuestFailed(string error);
    public static event OnLoginGuestComplete loginGuestComplete;
    public static event OnLoginGuestFailed loginGuestFailed;

    public void LoginGuest(string _email, string _password, bool _override)
    {
        StartCoroutine(_LoginGuest(_email, _password, _override));
    }

    IEnumerator _LoginGuest(string _email, string _password, bool _override)
    {
        string url = baseURL + "jpws_v2/user/login";

        Hashtable data = new Hashtable();
        data.Add("email", _email);
        data.Add("password", _password);
        data.Add("platform", platform);
        data.Add("deviceId", deviceID);

        //override flag for multi device login
        _override = true;

        if (_override)
        {
            data.Add("override", _override);
        }

        string JSONStr = easy.JSON.JsonEncode(data);
        Debug.Log("Login JSON QUERY PARAMETERS = " + JSONStr);
        byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", contentType);
        //headers.Add("Authorization", authorization);
        headers.Add("Accept", contentType);

        WWW www = new WWW(url, pData, headers);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Login Error" + www.error);
            if (loginGuestFailed != null)
            {
                loginGuestFailed(www.error);
            }
        }
        else
        {
            Debug.Log("Login RESPONCE = " + www.text);
            if (loginGuestComplete != null)
            {
                loginGuestComplete(www.text);
            }
        }

        yield return null;
    }
    #endregion

	#region Push Scores API/ Update User
	public delegate void OnPushScoresComplete (string responce);
	public delegate void OnPushScoresFailed (string error);
	public static event OnPushScoresComplete pushScoresComplete;
	public static event OnPushScoresFailed pushScoresFailed;

	public void PushScores(string _segmentName,int _preminumEarned,int _secondaryEarned,int _currentScore,Hashtable _prizesWon){
		PlayerProfile.Instance ().UpdateStats (_preminumEarned,_secondaryEarned,_currentScore);
		StartCoroutine (_PushScores(_segmentName,_preminumEarned,_secondaryEarned,_currentScore,_prizesWon));
	}

	IEnumerator _PushScores(string _segmentName,int _preminumEarned,int _secondaryEarned,int _currentScore,Hashtable _prizesWon){
		string url = baseURL+"jpws_v2/user/update";
		Hashtable data = new Hashtable ();
		//Mandatory
		data.Add ("email",PlayerProfile.Instance ().userEmail);	
		data.Add ("platform",platform);
		data.Add ("deviceId",deviceID);
		data.Add ("password", PlayerProfile.Instance ().password);

		//Optional
		if (PlayerProfile.Instance ().userCurrentTournamentId != 0) {
			Hashtable tScores = new Hashtable ();
			tScores.Add ("tournamentId", PlayerProfile.Instance ().userCurrentTournamentId);
			tScores.Add ("tournamentScore", PlayerProfile.Instance ().userCurrentTournamentScore);
			data.Add ("tournamentScores", tScores);
		}

		Hashtable segmentScores = new Hashtable ();
		segmentScores.Add (_segmentName, _currentScore);

		data.Add ("premiumCurrency",PlayerProfile.Instance ().userPremiumCurrency);
		data.Add ("secondaryCurrency",PlayerProfile.Instance ().userSecondryCurrency);
		data.Add ("lifetimeScore",PlayerProfile.Instance ().userLifetimeScore);
		data.Add ("segmentScrores",segmentScores);
		data.Add ("prizesWon",_prizesWon);

		string JSONStr = easy.JSON.JsonEncode(data);
        Debug.Log("URL:" + url);
		Debug.Log ("PushScores JSON QUERY PARAMETERS = " + JSONStr);
		byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
		headers.Add("Accept", contentType);

		WWW www = new WWW (url, pData, headers);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("PushScores Error" + www.error);
			if(pushScoresFailed != null){
				pushScoresFailed (www.error);
			}
		}
		else{
			Debug.Log ("PushScores RESPONCE = " + www.text);
			if(pushScoresComplete != null){
				pushScoresComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion

	#region Push UpdateCurrencies API/ Update User
	public delegate void OnUpdateCurrenciesComplete (string responce);
	public delegate void OnUpdateCurrenciesFailed (string error);
	public static event OnUpdateCurrenciesComplete updateCurrenciesComplete;
	public static event OnUpdateCurrenciesFailed updateCurrenciesFailed;

	public void UpdateCurrencies(int _preminumEarned,int _secondaryEarned)
	{
//		if (_secondaryEarned < 0)
//			return;
		PlayerProfile.Instance ().UpdateStats (_preminumEarned,_secondaryEarned,0);
		StartCoroutine (_UpdateCurrencies(_preminumEarned,_secondaryEarned));
	}

	IEnumerator _UpdateCurrencies(int _preminumEarned,int _secondaryEarned){
		string url = baseURL+"jpws_v2/user/update";
		Hashtable data = new Hashtable ();
		//Mandatory
		data.Add ("email",PlayerProfile.Instance ().userEmail);	
		data.Add ("platform",platform);
		data.Add ("deviceId",deviceID);
		data.Add ("password", PlayerProfile.Instance ().password);

		//Optional
		data.Add ("premiumCurrency",PlayerProfile.Instance ().userPremiumCurrency);
		data.Add ("secondaryCurrency",PlayerProfile.Instance ().userSecondryCurrency);

		string JSONStr = easy.JSON.JsonEncode(data);
		Debug.Log ("UpdateCurrencies JSON QUERY PARAMETERS = " + JSONStr);
		byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
		headers.Add("Accept", contentType);

		WWW www = new WWW (url, pData, headers);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("UpdateCurrencies Error" + www.error);
			if(updateCurrenciesFailed != null){
				updateCurrenciesFailed (www.error);
			}
		}
		else{
			Debug.Log ("UpdateCurrencies RESPONCE = " + www.text);
			if(updateCurrenciesComplete != null){
				updateCurrenciesComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion

    #region Submit Claim Data
    public delegate void OnSubmitClaimDataComplete(string responce);
    public delegate void OnSubmitClaimFailed(string error);
    public static event OnSubmitClaimDataComplete SubmitClaimComplete;
    public static event OnSubmitClaimFailed SubmitClaimFailed;

    public void SubmitClaimData(string name, string phoneNo, string _eMail)
    {
        StartCoroutine(_SubmitClaimData(name, phoneNo, _eMail));
    }

    IEnumerator _SubmitClaimData(string Name, string PhoneNo, string EMail)
    {

        Hashtable data = new Hashtable();
        data.Add("email", PlayerProfile.Instance().userEmail);
        data.Add("platform", platform);
        data.Add("deviceId", deviceID);
        data.Add("password", PlayerProfile.Instance().password);
        data.Add("contactname", Name);
        data.Add("contactnumber", PhoneNo);
        data.Add("contactemail", EMail);
        if (TournamentUIManager.Instance().PreviousTournamentdate != null)
            data.Add("tournamentId", TournamentUIManager.Instance().PreviousTournamentdate.id);
        
        string url = baseURL+"jpws_v2/user/updatecontact?email="+PlayerProfile.Instance().userEmail+"+&deviceId="+deviceID+"&platform="+platform+"&password="+PlayerProfile.Instance().password+"&contactname="+Name+"&contactnumber="+PhoneNo+"&contactemail="+EMail;
        //string JSONStr = easy.JSON.JsonEncode(data);
        //Debug.Log("PushScores JSON QUERY PARAMETERS = " + JSONStr);
        //byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

        //Dictionary<string, string> headers = new Dictionary<string, string>();
        //headers.Add("Content-Type", contentType);
        ////headers.Add("Authorization", authorization);
        //headers.Add("Accept", contentType);

        Debug.Log("Get Economy Data = " + url);
        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Get Economy Data Error" + www.error);
            if (www.error != null)
            {
                SubmitClaimFailed(www.error);
            }
        }
        else
        {
            Debug.Log("Get Economy Data RESPONCE = " + www.text);
            if (SubmitClaimComplete != null)
            {
                SubmitClaimComplete(www.text);
            }
        }

        yield return null;
    }
    #endregion

	#region Register User For Tournament  API/ Update User
	public delegate void RegisterUserForTournamentComplete (string responce);
	public delegate void RegisterUserForTournamentFailed (string error);
	public static event RegisterUserForTournamentComplete registerUserForTournamentComplete;
	public static event RegisterUserForTournamentFailed registerUserForTournamentFailed;

	public void RegisterUserForTournament(int tID){
		//PlayerProfile UpdateStats Function call here
	
		StartCoroutine (_RegisterUserForTournament(tID));
	}

	IEnumerator _RegisterUserForTournament(int tournamentID){
		string url = baseURL+"jpws_v2/user/update";

		Hashtable data = new Hashtable ();
		//Mandatory
		data.Add ("email",PlayerProfile.Instance ().userEmail);	
		data.Add ("platform",platform);
		data.Add ("deviceId",deviceID);
		data.Add ("password", PlayerProfile.Instance ().password);
		data.Add ("currentTournamentId", tournamentID);

		string JSONStr = easy.JSON.JsonEncode(data);
        Debug.Log("URL:" + url);
		Debug.Log ("RegisterUserForTournament JSON QUERY PARAMETERS = " + JSONStr);
		byte[] pData = Encoding.ASCII.GetBytes(JSONStr);

		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", contentType);
		//headers.Add("Authorization", authorization);
		headers.Add("Accept", contentType);

		WWW www = new WWW (url, pData, headers);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("RegisterUserForTournament Error" + www.error);
			if(registerUserForTournamentFailed != null){
				registerUserForTournamentFailed (www.error);
			}
		}
		else{
			Debug.Log ("RegisterUserForTournament RESPONCE = " + www.text);
			if(registerUserForTournamentComplete != null){
				registerUserForTournamentComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion

	#region Get Active Tournaments API
	public delegate void OnGetTournamentsComplete (string responce);
	public delegate void OnGetTournamentsFailed (string error);
	public static event OnGetTournamentsComplete getTournamentsComplete;
	public static event OnGetTournamentsFailed getTournamentsFailed;

	public void GetTournaments(){
		StartCoroutine (GetAllTournaments());
	}

	IEnumerator GetAllTournaments(){
		string url = baseURL + "jpws_v2/tournament/info?email=" + PlayerProfile.Instance ().userEmail + "&deviceId=" + deviceID + "&platform=" + platform;
		Debug.Log ("GetAllTournaments URL = " + url);
		WWW www = new WWW (url);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("GetAllTournaments Error" + www.error);
			if(getTournamentsFailed != null){
				getTournamentsFailed (www.error);
			}
		}
		else{
			Debug.Log ("GetAllTournaments RESPONCE = " + www.text);
			if(getTournamentsComplete != null){
				getTournamentsComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion

    #region Get Specific Tournament API
    public delegate void OnGetSpecificTournamentsComplete(string responce);
    public delegate void OnGetSpecificTournamentsFailed(string error);
    public static event OnGetSpecificTournamentsComplete getSpecificTournamentsComplete;
    public static event OnGetSpecificTournamentsFailed getSpecificTournamentsFailed;

    public void GetSpecificTournaments(int id)
    {
        StartCoroutine(GetSpecificTournament(id));
    }

    IEnumerator GetSpecificTournament(int id)
    {
        string url = baseURL + "jpws_v2/tournament/"+id+"?email=" + PlayerProfile.Instance().userEmail + "&deviceId=" + deviceID + "&platform=" + platform;
        Debug.Log("GetSpecificTournament URL = " + url);
        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("GetSpecificTournament Error" + www.error);
            if (getSpecificTournamentsFailed != null)
            {
                getSpecificTournamentsFailed(www.error);
            }
        }
        else
        {
            Debug.Log("GetSpecificTournament RESPONCE = " + www.text);
            if (getSpecificTournamentsComplete != null)
            {
                getSpecificTournamentsComplete(www.text);
            }
        }

        yield return null;
    }
    #endregion


    #region Get Life Time Score API
	public delegate void OnGetLifefTimeScoresComplete (string responce);
	public delegate void OnGetLifefTimeScoresFailed (string error);
	public static event OnGetLifefTimeScoresComplete getLifeTimeScoresComplete;
	public static event OnGetLifefTimeScoresFailed getLifeTimeScoresFailed;

	public void GetLifeTimeScores(int _fetchSize){
		StartCoroutine (_GetLifeTimeScores(_fetchSize));
	}

	IEnumerator _GetLifeTimeScores(int _fetchSize){
		string url = baseURL + "jpws_v2/score/lifetime?email=" + PlayerProfile.Instance ().userEmail + "&deviceId=" +  deviceID + "&platform=" +  platform +"&fetchSize="+_fetchSize;
		WWW www = new WWW (url);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("GetLifeTimeScores Error" + www.error);
			if(getLifeTimeScoresFailed != null){
				getLifeTimeScoresFailed (www.error);
			}
		}
		else{
			Debug.Log ("GetLifeTimeScores RESPONCE = " + www.text);
			if(getLifeTimeScoresComplete != null){
				getLifeTimeScoresComplete (www.text);
			}
		}

		yield return null;
	}
	#endregion

	#region Get Tournament Scores API
	public delegate void OnGetTournamentScoresComplete (string responce);
	public delegate void OnGetTournamentScoresFailed (string error);
	public static event OnGetTournamentScoresComplete getTournamentScoresComplete;
	public static event OnGetTournamentScoresFailed getTournamentScoresFailed;

	public void GetTournamentScores(int _fetchSize,int _id)
    {
		StartCoroutine (_GetTournamentScores(_fetchSize,_id));
	}

	IEnumerator _GetTournamentScores(int _fetchSize,int _id){
		string url = baseURL + "jpws_v2/score/tournament?email=" + PlayerProfile.Instance ().userEmail + "&deviceId=" +  deviceID + "&platform=" +  platform +"&tournamentId=" +  _id +"&fetchSize="+_fetchSize;
		Debug.Log ("Get Tournament Scores = " + url);
		WWW www = new WWW (url);
		yield return www;

		if (!string.IsNullOrEmpty(www.error)){
			Debug.LogError("getTournamentScoresFailed Error" + www.error);
			if(getTournamentScoresFailed != null){
				getTournamentScoresFailed (www.error);
			}
		}
		else{
			Debug.Log ("getTournamentScoresComplete RESPONCE = " + www.text);
			if(getTournamentScoresComplete != null){
				getTournamentScoresComplete (www.text);
			}
		}

		yield return null;
	}
    #endregion

    #region Get Economy Date API
    public delegate void OnGetEconomyDataComplete(string responce);
    public delegate void OnGetEconomyDatFailed(string error);
    public static event OnGetEconomyDataComplete getEconomyDataComplete;
    public static event OnGetEconomyDatFailed getEconomyDataFailed;

    public void GetEconomyData()
    {
        StartCoroutine(_GetEconomyData());
    }

    IEnumerator _GetEconomyData()
    {
        string url = "https://jeetopakistan.rockvillegroup.com/jpws_v2/economy/json";
        Debug.Log("SCENE NAME: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Debug.Log("Get Economy Data = " + url);
        WWW www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Get Economy Data Error" + www.error);
            if (getEconomyDataFailed != null)
            {
                getEconomyDataFailed(www.error);
            }
        }
        else
        {
            Debug.Log("Get Economy Data RESPONCE = " + www.text);
            if (getEconomyDataComplete != null)
            {
                getEconomyDataComplete(www.text);
            }
        }

        yield return null;
    }
    #endregion
    */
}
