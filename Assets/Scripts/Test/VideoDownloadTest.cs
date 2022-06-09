using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoDownloadTest : MonoBehaviour
{
    public string url;
    public VideoPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DownloadAndPlay()
    {
        StartCoroutine(DownloadAndPlay_IEnumerator());
    }
    public IEnumerator DownloadAndPlay_IEnumerator()
    {
        string fileName = Path.GetFileName(url);
        string localPath = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log($"Local Path : {localPath}");
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                System.IO.File.WriteAllBytes(localPath, www.downloadHandler.data);
                player.url = localPath;
                player.Play();
            }
        }
    }
}
