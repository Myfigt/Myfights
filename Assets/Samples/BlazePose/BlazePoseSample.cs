using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// BlazePose form MediaPipe
/// https://github.com/google/mediapipe
/// https://viz.mediapipe.dev/demo/pose_tracking
/// </summary>
public sealed class BlazePoseSample : MonoBehaviour
{

    [SerializeField, FilePopup("*.tflite")] string poseDetectionModelFile = "coco_ssd_mobilenet_quant.tflite";
    [SerializeField, FilePopup("*.tflite")] string poseLandmarkModelFile = "coco_ssd_mobilenet_quant.tflite";
    [SerializeField] RawImage cameraView = null;
    [SerializeField] RawImage debugView = null;
    [SerializeField] Canvas canvas = null;
    [SerializeField] bool useLandmarkFilter = true;
    [SerializeField] Vector3 filterVelocityScale = Vector3.one * 10;
    [SerializeField] bool runBackground;
    [SerializeField, Range(0f, 1f)] float visibilityThreshold = 0.5f;


    WebCamTexture webcamTexture;
    PoseDetect poseDetect;
    PoseLandmarkDetect poseLandmark;

    Vector3[] rtCorners = new Vector3[4]; // just cache for GetWorldCorners
    // [SerializeField] // for debug raw data
    Vector4[] worldJoints;
    PrimitiveDraw draw;
    PoseDetect.Result poseResult;
    PoseLandmarkDetect.Result landmarkResult;
    UniTask<bool> task;
    CancellationToken cancellationToken;

    bool NeedsDetectionUpdate => poseResult == null || poseResult.score < 0.5f;

    public TextMeshProUGUI match, damage,score,score1;
    public Text facingT;

    public float totalDamage = 0;

    // pose calculations by Arif
    public bool poseDone = false, checkAchieve = false, poseStarted = false;
    public float timer = 0.0f,posescore=0.0f;

    public int lockPosition = 0; // 1=front , 2=left , 3=right;
    public bool facingFront = false, facingLeft = false, facingRight = false, facingBack = false;

    void Start()
    {
        // Init model
        poseDetect = new PoseDetect(poseDetectionModelFile);
        poseLandmark = new PoseLandmarkDetect(poseLandmarkModelFile);
        Debug.Log("About to initialize WebCamUtil ");
        // Init camera 
        string cameraName = WebCamUtil.FindName(new WebCamUtil.PreferSpec()
        {
            isFrontFacing = false,
            kind = WebCamKind.WideAngle,
        });
        Debug.Log("Camera Recieved ");
        webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        cameraView.texture = webcamTexture;
        webcamTexture.Play();
        Debug.Log($"Starting camera: {cameraName}");

        draw = new PrimitiveDraw(Camera.main, gameObject.layer);
        worldJoints = new Vector4[PoseLandmarkDetect.JointCount];

        cancellationToken = this.GetCancellationTokenOnDestroy();
        score.text = "";
    }

    void OnDestroy()
    {
        webcamTexture?.Stop();
        poseDetect?.Dispose();
        poseLandmark?.Dispose();
        draw?.Dispose();
    }

    void Update()
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync();
            }
        }
        else
        {
            Invoke();
        }

        if (poseResult != null && poseResult.score > 0f)
        {
            DrawFrame(poseResult);
        }
        facingT.text = "Facing: NIL";
        if (landmarkResult != null && landmarkResult.score > 0.7f)
        {
            DrawCropMatrix(poseLandmark.CropMatrix);
            DrawJoints(landmarkResult.joints);
            match.text = "On Screen: " + (int)(landmarkResult.score*100)+"%";
            damage.text = "Total Damage: " + totalDamage;
            if (landmarkResult.joints.Length > 15)
            {
                if (lockPosition == 1)
                {
                    facingT.text = "Striking FRONT";
                }
                if (lockPosition == 2)
                {
                    facingT.text = "Striking LEFT";
                }
                if (lockPosition == 3)
                {
                    facingT.text = "Striking RIGHT";
                }
                if (lockPosition!=0 && poseStarted == false)
                {
                    lockPosition = 0;
                }
                if (lockPosition == 0)
                {
                    if (landmarkResult.joints[12].x > landmarkResult.joints[11].x)
                    {
                        bool isFrontFacing = true;
                        //score.text = "Z R wrist: " + landmarkResult.joints[15].w;
                        if (landmarkResult.joints[13].x > landmarkResult.joints[11].x)
                        {
                            facingBack = false; facingFront = false; facingLeft = true; facingRight = false;
                            isFrontFacing = false;
                            facingT.text = "Facing: Left";
                        }
                        if (landmarkResult.joints[14].x < landmarkResult.joints[12].x)
                        {
                            facingBack = false; facingFront = false; facingLeft = false; facingRight = true;
                            isFrontFacing = false;
                            facingT.text = "Facing: Right";
                        }
                        if (isFrontFacing)
                        {
                            facingBack = false; facingFront = true; facingLeft = false; facingRight = false;
                            facingT.text = "Facing: Front Facing";
                            //float angle = calculateAngle(landmarkResult.joints[11], landmarkResult.joints[13], landmarkResult.joints[15]);
                            //checkPoseMove(landmarkResult.joints[15].y > landmarkResult.joints[13].y && landmarkResult.joints[15].y< landmarkResult.joints[10].y && angle<20 
                            //    , landmarkResult.joints[15].y> landmarkResult.joints[13].y && landmarkResult.joints[15].y > landmarkResult.joints[10].y && angle > 15
                            //    , 3.0f);
                            if (poseDone == true)
                            {
                                score.text = SendSceneManager.FolderName + " Done:" + posescore;
                                posescore = 0.0f; poseDone = false;
                            }
                        }
                    }
                    else
                    {
                        facingT.text = "Facing: Back Facing";
                    }
                }
                PoseMoveManager();
            }

        }
    }
    float calculateAngle(Vector4 a,Vector4 b,Vector4 c)
    {
        float angle = 0;

        float rad = Mathf.Atan2(c.y - b.y, c.x - b.x) - Mathf.Atan2(a.y - b.y, a.x - b.x);
        angle = Mathf.Abs(rad * Mathf.Rad2Deg);
        if (angle > 180.0f)
           angle = 360.0f-angle;
        return angle;
    }

    void PoseMoveManager()
    {
        if (SendSceneManager.POSENUMBER == SendSceneManager.PS_LeftRisingElbow)
        {
            if (facingLeft)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y  && landmarkResult.joints[14].y < landmarkResult.joints[12].y
                    , landmarkResult.joints[14].y > landmarkResult.joints[12].y
                    , 3);
                return;
            }
            if (facingRight)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[14].y > landmarkResult.joints[12].y
                    , 3);
                return;
            }
            if (facingFront)
            {
                checkPoseMove(landmarkResult.joints[16].x < landmarkResult.joints[12].x && landmarkResult.joints[14].y< landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[14].y> landmarkResult.joints[12].y
                    , 3);
                return;
            }
        }
        if (SendSceneManager.POSENUMBER == SendSceneManager.PS_LeftLowKick)
        {
            if (facingLeft)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[25].y
                    , 3);
                return;
            }
            if (facingRight)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[25].y
                    , 3);
                return;
            }
            if (facingFront)
            {
                checkPoseMove(landmarkResult.joints[16].x < landmarkResult.joints[12].x && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[25].y
                    , 3);
                return;
            }
        }
        if (SendSceneManager.POSENUMBER == SendSceneManager.PS_LeftHighKick)
        {
            if (facingLeft)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[23].y
                    , 3);
                return;
            }
            if (facingRight)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[23].y
                    , 3);
                return;
            }
            if (facingFront)
            {
                checkPoseMove(landmarkResult.joints[16].x < landmarkResult.joints[12].x && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[28].y > landmarkResult.joints[23].y
                    , 3);
                return;
            }
        }
        if (SendSceneManager.POSENUMBER == SendSceneManager.PS_RightLowKick)
        {
            if (facingLeft)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y
                    , landmarkResult.joints[27].y > landmarkResult.joints[26].y
                    , 3);
                return;
            }
            if (facingRight)
            {
                checkPoseMove(landmarkResult.joints[16].y > landmarkResult.joints[12].y && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[27].y > landmarkResult.joints[26].y
                    , 3);
                return;
            }
            if (facingFront)
            {
                checkPoseMove(landmarkResult.joints[16].x < landmarkResult.joints[12].x && landmarkResult.joints[14].y < landmarkResult.joints[12].y && landmarkResult.joints[20].y > landmarkResult.joints[12].y
                    , landmarkResult.joints[27].y > landmarkResult.joints[26].y
                    , 3);
                return;
            }
        }
    }

    void checkPoseMove(bool set,bool check , float timeSet)
    {
        if(set && timer <= 0.0f)
        {
            timer = timeSet;
            poseStarted = true;
            checkAchieve = false;
            if (facingFront)
                lockPosition = 1;
            else if (facingLeft)
                lockPosition = 2;
            else if (facingRight)
                lockPosition = 3;
        }
        if (timer > 0.0f)
        {
            timer = timer - Time.deltaTime;
        }
        else
        {
            checkAchieve = false; poseStarted = false; poseDone = false; posescore = 0;
            lockPosition = 0;
        }
        if(check && poseStarted && timer > 0.0f)
        {
            checkAchieve = true;
        }
        if(set && poseStarted && checkAchieve && timer > 0.0f)
        {
            checkAchieve = false;poseStarted = false;poseDone = true;posescore = timer;
            totalDamage += timer;
            timer = 0.0f;
            lockPosition = 0;
        }
        
    }
    void DrawFrame(PoseDetect.Result pose)
    {
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        draw.color = Color.green;
        draw.Rect(MathTF.Lerp(min, max, pose.rect, true), 0.02f, min.z);

        foreach (var kp in pose.keypoints)
        {
            draw.Point(MathTF.Lerp(min, max, (Vector3)kp, true), 0.05f);
        }
        draw.Apply();
    }

    void DrawCropMatrix(in Matrix4x4 matrix)
    {
        draw.color = Color.red;

        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var mtx = WebCamUtil.GetMatrix(-webcamTexture.videoRotationAngle, false, webcamTexture.videoVerticallyMirrored)
            * matrix.inverse;
        Vector3 a = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(0, 0, 0)));
        Vector3 b = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(1, 0, 0)));
        Vector3 c = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(1, 1, 0)));
        Vector3 d = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(0, 1, 0)));

        draw.Quad(a, b, c, d, 0.02f);
        draw.Apply();
    }

    void DrawJoints(Vector4[] joints)
    {
        draw.color = Color.blue;

        // Vector3 min = rtCorners[0];
        // Vector3 max = rtCorners[2];
        // Debug.Log($"rtCorners min: {min}, max: {max}");

        // Apply webcam rotation to draw landmarks correctly
        Matrix4x4 mtx = WebCamUtil.GetMatrix(-webcamTexture.videoRotationAngle, false, webcamTexture.videoVerticallyMirrored);

        // float zScale = (max.x - min.x) / 2;
        float zScale = 1;
        float zOffset = canvas.planeDistance;
        float aspect = (float)Screen.width / (float)Screen.height;
        Vector3 scale, offset;
        if (aspect > 1)
        {
            scale = new Vector3(1f / aspect, 1f, zScale);
            offset = new Vector3((1 - 1f / aspect) / 2, 0, zOffset);
        }
        else
        {
            scale = new Vector3(1f, aspect, zScale);
            offset = new Vector3(0, (1 - aspect) / 2, zOffset);
        }

        // Update world joints
        var camera = canvas.worldCamera;
        for (int i = 0; i < joints.Length; i++)
        {
            Vector3 p = mtx.MultiplyPoint3x4((Vector3)joints[i]);
            p = Vector3.Scale(p, scale) + offset;
            p = Camera.main.ViewportToWorldPoint(p);

            // w is visibility
            worldJoints[i] = new Vector4(p.x, p.y, p.z, joints[i].w);
        }

        // Draw
        for (int i = 0; i < worldJoints.Length; i++)
        {
            Vector4 p = worldJoints[i];
            if (p.w > visibilityThreshold)
            {
                draw.Cube(p, 0.2f);
            }
        }
        var connections = PoseLandmarkDetect.Connections;
        for (int i = 0; i < connections.Length; i += 2)
        {
            var a = worldJoints[connections[i]];
            var b = worldJoints[connections[i + 1]];
            if (a.w > visibilityThreshold || b.w > visibilityThreshold)
            {
                draw.Line3D(a, b, 0.05f);
            }
        }
        draw.Apply();
    }

    void Invoke()
    {
        if (NeedsDetectionUpdate)
        {
            poseDetect.Invoke(webcamTexture);
            cameraView.material = poseDetect.transformMat;
            cameraView.rectTransform.GetWorldCorners(rtCorners);
            poseResult = poseDetect.GetResults(0.7f, 0.3f);
        }
        if (poseResult.score < 0)
        {
            poseResult = null;
            landmarkResult = null;
            return;
        }
        poseLandmark.Invoke(webcamTexture, poseResult);
        debugView.texture = poseLandmark.inputTex;

        if (useLandmarkFilter)
        {
            poseLandmark.FilterVelocityScale = filterVelocityScale.x;
        }
        landmarkResult = poseLandmark.GetResult(useLandmarkFilter);

        if (landmarkResult.score < 0.3f)
        {
            poseResult.score = landmarkResult.score;
        }
        else
        {
            poseResult = PoseLandmarkDetect.LandmarkToDetection(landmarkResult);
        }
    }

    async UniTask<bool> InvokeAsync()
    {
        if (NeedsDetectionUpdate)
        {
            // Note: `await` changes PlayerLoopTiming from Update to FixedUpdate.
            poseResult = await poseDetect.InvokeAsync(webcamTexture, cancellationToken, PlayerLoopTiming.FixedUpdate);
        }
        if (poseResult.score < 0)
        {
            poseResult = null;
            landmarkResult = null;
            return false;
        }

        if (useLandmarkFilter)
        {
            poseLandmark.FilterVelocityScale = filterVelocityScale.x;
        }
        landmarkResult = await poseLandmark.InvokeAsync(webcamTexture, poseResult, useLandmarkFilter, cancellationToken, PlayerLoopTiming.Update);

        // Back to the update timing from now on 
        if (cameraView != null)
        {
            cameraView.material = poseDetect.transformMat;
            cameraView.rectTransform.GetWorldCorners(rtCorners);
        }
        if (debugView != null)
        {
            debugView.texture = poseLandmark.inputTex;
        }

        // Generate poseResult from landmarkResult
        if (landmarkResult.score < 0.3f)
        {
            poseResult.score = landmarkResult.score;
        }
        else
        {
            poseResult = PoseLandmarkDetect.LandmarkToDetection(landmarkResult);
        }

        return true;
    }
}
