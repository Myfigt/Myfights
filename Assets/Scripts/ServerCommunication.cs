using System;
//using System.Security.Policy;
//using System.Windows.Forms;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

/// <summary>
/// Forefront class for the server communication.
/// </summary>
public class ServerCommunication : MonoBehaviour
{
    public delegate void OnMessageRecieved(string responce, WS_ActionType action);
   
    public static event OnMessageRecieved NewMessageRecieved;
  
    // Server IP address
    [SerializeField]
    private string hostIPBase;

    // Server port
    [SerializeField]
    private int port = 3000;

    // Flag to use localhost
    [SerializeField]
    private bool useLocalhost = true;

    // Address used in code
    private string host => useLocalhost ? "localhost" : hostIPBase;
    // Final server address
    private string server;

    // WebSocket Client
    private WsClient client;

    // Class with messages for "lobby"
    public LobbyMessaging Lobby { private set; get; }

    /// <summary>
    /// Unity method called on initialization
    /// </summary>
    private void Awake()
    {
        //server = "ws://" + host + ":" + port;
        //client = new WsClient("ws://54.66.142.35:8000/ws/data/");

        // Messaging
       // Lobby = new LobbyMessaging(this);
    }

    /// <summary>
    /// Unity method called every frame
    /// </summary>
    private void Update()
    {
        if (client!= null)
        {
            // Check if server send new messages
            var cqueue = client.receiveQueue;
            string msg;
            Debug.Log(cqueue.Count);
            while (cqueue.TryPeek(out msg))
            {
                // Parse newly received messages
                cqueue.TryDequeue(out msg);
                HandleMessage(msg);
            }
        }
        
    }

    /// <summary>
    /// Method responsible for handling server messages
    /// </summary>
    /// <param name="msg">Message.</param>
    private void HandleMessage(string msg)
    {
        Debug.Log("Server: " + msg);

        // Deserializing message from the server
        //var message = JsonUtility.FromJson<WSMessage>(msg);
        string payloadData = string.Empty;
        WS_ActionType _action = WS_ActionType.None;
        Hashtable responceData = (Hashtable)easy.JSON.JsonDecode(msg);
        foreach (DictionaryEntry item in responceData)
        {
            if (item.Key.ToString() == "action")
            {
                _action = (WS_ActionType)Enum.Parse(typeof(WS_ActionType), item.Value.ToString() );
            }
            else if (item.Key.ToString() == "payload")
            {
                //Hashtable PayLoadData = (Hashtable)easy.JSON.JsonDecode(item.Value.ToString());
                foreach (DictionaryEntry data in item.Value as Hashtable)
                {
                    if (data.Key.ToString() == "payload")
                    {
                        payloadData = data.Value.ToString();
                    }
                }
                    

            }
        }
        NewMessageRecieved?.Invoke(payloadData , _action);
    }

    /// <summary>
    /// Call this method to connect to the server
    /// </summary>
    public async void ConnectToServer(int userID)
    {
        server =$"{host}{userID}/";
        client = new WsClient(server);
        await client.Connect();
    }

    /// <summary>
    /// Method which sends data through websocket
    /// </summary>
    /// <param name="message">Message.</param>
    public void SendRequest(string payLoad, WS_ActionType action, string authTokken)
    {
        WSMessage wSMessage = new WSMessage() { action = action.ToString(), token = authTokken, payload = payLoad };


        string data1 = Newtonsoft.Json.JsonConvert.SerializeObject(wSMessage, Formatting.Indented);
        Debug.Log(data1);
        WsClient.Send(data1);


    }
}
public enum WS_ActionType
{ 
    None,
    make_user_online_offline,
    add_friend,
    join_room,
    accept_friend_request,
    notification,

}

[Serializable]
public class WSMessage
{
    public string action { get; set; }
    public string token { get; set; }
    public string payload { get; set; }
}




