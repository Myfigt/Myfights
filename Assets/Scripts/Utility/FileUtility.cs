using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class FileUtility : MonoBehaviour
{
    private static FileUtility instance = null;
    public static FileUtility Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FileUtility>();
            }
            return instance;
        }
    }
    public void DownloadFile(string url,Action<bool,DownloadHandler> OnComplete)
    {
        StartCoroutine(DownloadFileIEnumerator(url,OnComplete));
    }
    private IEnumerator DownloadFileIEnumerator(string url, Action<bool, DownloadHandler> OnComplete)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                OnComplete?.Invoke(false, null);
            }
            else
            {
                OnComplete?.Invoke(true, www.downloadHandler);
            }
        }
    }

    public void WriteBytes(string filePath,byte[] bytes)
    {
        System.IO.File.WriteAllBytes(filePath, bytes);
    }
}
