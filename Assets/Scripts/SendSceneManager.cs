using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SendSceneManager : MonoBehaviour
{
    static public string FrontVid, LeftVid, RightVid,FolderName;
    static public int POSENUMBER=0;

    public VideoClip fVid, lVid, rVid;

    public BlazePoseSample BPS;

    public VideoPlayer videoplayer;

    public bool pF, pB, pL, pR;

    static public int PS_LeftRisingElbow = 1,PS_LeftLowKick=2,PS_LeftHighKick=3,PS_RightLowKick=4;
    // Start is called before the first frame update
    void Start()
    {
        loadvideo();
        pF = false;pB = false;pL = false;pR = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (BPS != null)
        {
            if (BPS.facingFront)
            {
                videoplayer.clip = fVid;
            }
            if (BPS.facingLeft)
                videoplayer.clip = lVid;
            if (BPS.facingRight)
                videoplayer.clip = rVid;
            if (BPS.facingBack)
                videoplayer.clip = fVid;

            if(pF!=BPS.facingFront || pB != BPS.facingBack || pL != BPS.facingLeft || pR != BPS.facingRight)
            {
                pF = BPS.facingFront;pB = BPS.facingBack;pL = BPS.facingLeft;pR = BPS.facingRight;
                videoplayer.Play();
            }
        }
    }
    public void loadvideo()
    {
        videoplayer = GameObject.Find("video").GetComponent<VideoPlayer>();
        /*videoplayer.url = Path.Combine(Application.streamingAssetsPath, "L Cross Front.mp4");*/
        fVid = Resources.Load<VideoClip>(FolderName + "/" + FrontVid) as VideoClip;
        videoplayer.clip = fVid;
        lVid = Resources.Load<VideoClip>(FolderName +"/"+ LeftVid) as VideoClip;
        rVid = Resources.Load<VideoClip>(FolderName + "/" + RightVid) as VideoClip;
        videoplayer.Play();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
