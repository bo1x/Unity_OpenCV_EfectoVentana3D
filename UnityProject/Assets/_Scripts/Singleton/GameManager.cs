using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;
using UnityEngine.Scripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private WebCamTexture _webcam;
    private bool _flipWebCam = true;
    private int requestedFps = 30;
    private Coroutine loading;
    private float minLoadTime = 0.3f;
    private bool isLoading = false;

    [SerializeField] private GameObject pauseprefab;
    private GameObject _pauseObject;
    private bool pause = false;
    private int DropDownWebcam;

    [SerializeField] private GameObject _loading;
    [SerializeField]private Image _FadeImage;
    private float _fadeTime = 0.2f;
    private bool showWebcam = false;
    private Vector2Int requestSize = new Vector2Int(640, 360);

    private bool _canEyeTracking;
    private bool _moveAxisZ = true;
    private bool _canDevelopeMode;
    private Vector3 _sensibility = Vector3.one * 30;

    private int _targetFrameRate = 30;

    Color32[] c;
    Vec3b[] videoSourceImageData;
    Mat mat;
    Mat _webcamMat;
    Vec3b[] matData;
    Texture2D tex;

    //Face Detector
    CascadeClassifier faceCascade = new CascadeClassifier();
    CascadeClassifier eyeCascade = new CascadeClassifier();
    bool imageHaveFace = false;
    Mat faceMat;
    float offsetFace = 14;
    OpenCvSharp.Rect lastFace;
    Window a;

    private float X, Y, Z;
    private int frameGuiños;
    private bool guinar = false;
    


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFrameRate;
        _FadeImage.gameObject.SetActive(false);
        _loading.SetActive(false);

        debugWindow();
        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        /* Recordar sustituir a la hora de buildear XD y tambien de añadir la carpeta en lo del programa
        faceCascade.Load(System.IO.Directory.GetCurrentDirectory() + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(System.IO.Directory.GetCurrentDirectory() + "/haarcascades/haarcascade_eye.xml");*/
    }

    private void Update()
    {
        WebCamCalling();

        debugWindow();
    }


    private void debugWindow()
    {
        if (CanDevelopeMode())
        {
            if (a == null)
                a = new Window("capturawebcam");

            a.ShowImage(OpenCVFace());
        }
        else if (CanDevelopeMode() == false && a != null)
        {
            a.Close();
            a = null;
        }
    }

    #region SceneManager

    public void NextScene(int value)
    {
        switch (value)
        {
            case 0:
                LoadLevelUI("ConfigurationScene");
                break;
            case 1:
                LoadLevelUI("Game");
                break;
            default:
                Debug.LogWarning("Esta escena no se reconoce");
                break;
        }
    }

    public void LoadLevelUI(string levelToLoad)
    {
        loading = StartCoroutine(LoadLevelASync(levelToLoad));
    }

    IEnumerator LoadLevelASync(string levelToLoad)
    {
        isLoading = true;
        _loading.transform.parent.gameObject.SetActive(true);

        _FadeImage.gameObject.SetActive(true);
        _FadeImage.canvasRenderer.SetAlpha(0);


        while (!Fade(1))
            yield return null;

        _loading.SetActive(true);

        while (!Fade(0))
            yield return null;
        
        if(levelToLoad == "Game")
            pauseprefab.SetActive(true);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);


        if(_webcam != null)
            _webcam.Stop();

        System.GC.Collect();

        float progressValue = 0f;

        while (!loadOperation.isDone)
        {
            progressValue += Time.deltaTime;
            yield return null;
        }

        if (_webcam != null)
            _webcam.Play();

        while (progressValue < minLoadTime)
        {
            progressValue += Time.deltaTime;
            yield return null;
        }

        pauseprefab.SetActive(false);

        while (!Fade(1))
            yield return null;

        _loading.SetActive(false);

        while (!Fade(0))
            yield return null;

        _loading.transform.parent.gameObject.SetActive(false);
        isLoading = false;

        yield return null;
    }

    private bool Fade(float target)
    {
        _FadeImage.CrossFadeAlpha(target, _fadeTime, true);

        if(Mathf.Abs(_FadeImage.canvasRenderer.GetAlpha() - target) <= 0.05f){
            _FadeImage.canvasRenderer.SetAlpha(target);
            return true;
        }

        return false;
    }

    public bool GetPause()
    {
        return pause;
    }

    public void Pause(bool value)
    {
        if (pause && value == false)
        {
            pause = false;
            pauseprefab.SetActive(false);
            return;
        }
        else if (value == false)
            return;

        pause = true;
        pauseprefab.SetActive(true);

    }

    public bool GetLoad()
    {
        return isLoading;
    }

    #endregion

    #region Webcam

    public void SetWebcam( WebCamTexture actualWebcam )
    {
        if (_webcam != null)
        {
            if (_webcam.deviceName == actualWebcam.deviceName)
                return;

            _webcam.Stop();
        }

        _webcam = actualWebcam;

        GameManager.Instance.WebCamConstructor(requestedFps, requestSize.x, requestSize.y);
        _webcam.Play();
    }

    public void NoAvaibleWebcam()
    {
        Debug.LogError("No hay webcams disponibles");
        //Mandarte a la escena o ponerte el prefab para configurar la webcam

    }

    public void StopWebcam()
    {
        if (_webcam == null)
            return;

        _webcam.Stop();
        _webcam = null;
    }


    public void WebCamConstructor(int fps, int width, int height) {
        requestedFps = fps;
        requestSize = new Vector2Int(width, height);
        
        if (_webcam == null)
        {
            return;
        }
        _webcam.Stop();
        _webcam.requestedFPS = requestedFps;
        _webcam.requestedHeight = requestSize.x;
        _webcam.requestedWidth = requestSize.y;
        _webcam.Play();
    }

    public WebCamTexture GetWebcam()
    {
        return _webcam;
    }

    public int RequestedFps()
    {
        return requestedFps;
    }

    public Vector2Int RequestedSize()
    {
        return requestSize;
    }

    public bool CanShowWebcam()
    {
        return showWebcam;
    }

    public void CanShowWebcam(bool value)
    {
        showWebcam = value;
        return;
    }

    public Vector3 GetSensibility()
    {
        return _sensibility;
    }

    public void setdropdown(int value)
    {
        DropDownWebcam = value;
        return;
    }
    public int whatdropdown()
    {
        return DropDownWebcam;
    }

    public void SetSensibility(float value)
    {
        _sensibility = Vector3.one * value;
        return;
    }



    public Vector3 getOpenCVAxis()
    {
        return new Vector3(X, Y, Z);
    }
    #endregion

    #region OpenCV
    //OpenCV Things

    public Mat WebCamMat()
    {
        return _webcamMat;
    }

    public Mat OpenCVFace()
    {
        if (_webcam == null)
            return null;

        if (!_webcam.didUpdateThisFrame)
            return null;

        faceMat = WebCamMat();
        var faces = faceCascade.DetectMultiScale(faceMat, 1.3, 5);
        
        imageHaveFace = faces.Length == 0 ? false : true;

        if(!imageHaveFace)
            return faceMat;

        if (faces.Length > 1)
            Debug.Log(faces.Length);

        foreach (OpenCvSharp.Rect face in faces)
        {
            if (faces.Length == 1)
                lastFace = face;

            if (faces.Length == 1 || face.Top < lastFace.Top + offsetFace && face.Top > lastFace.Top - offsetFace && face.Left < lastFace.Left + offsetFace && face.Left > lastFace.Left - offsetFace)
            {

                X = face.Left + face.Width / 2;
                X = X - faceMat.Width / 2;
                //Debug.Log(X);
                X = X * (360f / faceMat.Height);
               // Debug.Log(X);
                Y = face.Top + face.Height / 2;
                Y = Y - faceMat.Height / 2;
                Y = Y * (360f / faceMat.Height);
                Z = face.Height;
                Z = Z * (360f / faceMat.Height);
                Y = -Y;


                Point centro = new Point(face.Left + face.Width / 2, face.Top + face.Height / 2);
                Scalar color = new Scalar(0, 0, 255);
                Cv2.Circle(faceMat, centro, 1, color, 5);
                Cv2.Rectangle(faceMat, face.TopLeft, face.BottomRight, new Scalar(0, 255, 0), 2);
            }
        }
        var eyes = eyeCascade.DetectMultiScale(faceMat, 1.3, 5);
        foreach (OpenCvSharp.Rect eye in eyes)
        {
            guinar = false;
            if (eyes.Length == 1)
            {
                frameGuiños++;

            }
            if (eyes.Length >= 2)
            {
                frameGuiños = 0;
            }
            if (frameGuiños >= 8)
            {
                guinar = true;
            }
            Debug.Log(eyes.Length);
            Point centroojo = new Point(eye.Left + eye.Width / 2, eye.Top + eye.Height / 2);
            Scalar colorojo = new Scalar(0, 0, 255);
            Cv2.Circle(faceMat, centroojo, 1, colorojo, 5);
        }

        return faceMat;
    }

    private Mat WebCamCalling()
    {

        if (_webcam == null)
            return _webcamMat;

        if (!_webcam.didUpdateThisFrame)
            return _webcamMat;



        _webcamMat = new Mat(_webcam.height, _webcam.width, MatType.CV_8UC4);
        _webcamMat = TextureToMat(WebcamToTexture2D(_webcam));

        if (_flipWebCam)
            Cv2.Flip(mat, mat, FlipMode.Y);

        return _webcamMat;
    }
    public bool IsgGuinado()
    {
        return guinar;
    }
    #region Conversiones entre imagenes
    public Mat TextureToMat(Texture2D sourceTexture)
    {
        
        int imgHeight = sourceTexture.height;
        int imgWidth = sourceTexture.width;

        c = sourceTexture.GetPixels32();

        GCHandle gcBitsHandle = GCHandle.Alloc(c, GCHandleType.Pinned);

        videoSourceImageData = new Vec3b[imgHeight * imgWidth]; ;
        
        Parallel.For(0, imgHeight, i =>
        {
            for (var j = 0; j < imgWidth; j++)
            {
                Vec3b vec3 = new Vec3b
                {
                    Item0 = c[j + i * imgWidth].b,
                    Item1 = c[j + i * imgWidth].g,
                    Item2 = c[j + i * imgWidth].r
                };

                videoSourceImageData[j + i * imgWidth] = vec3;
            }
        });

        mat = new Mat(imgHeight, imgWidth, MatType.CV_8UC3);

        mat.SetArray(videoSourceImageData);
        GCHandle gcMatsHandle = GCHandle.Alloc(mat, GCHandleType.Pinned);
        Cv2.Flip(mat, mat, FlipMode.X);

        gcMatsHandle.Free();
        gcBitsHandle.Free();
        return mat;
    }
        

    public Texture2D MatToTexture(Mat sourceMat)
    {
        Cv2.Flip(sourceMat, sourceMat, FlipMode.X);

        matData = new Vec3b[sourceMat.Height * sourceMat.Width];

        sourceMat.GetArray(out matData);

         c = new Color32[sourceMat.Height * sourceMat.Width];

         Parallel.For(0, sourceMat.Height, i => {
             for (var x = 0; x < sourceMat.Width; x++)
             {
                 Color32 color32 = new Color32
                 {
                     r = matData[x + i * sourceMat.Width].Item2,
                     g = matData[x + i * sourceMat.Width].Item1,
                     b = matData[x + i * sourceMat.Width].Item0,
                     a = 255
                 };
                 c[x + i * sourceMat.Width] = color32;
             }
         });

         tex = new Texture2D(sourceMat.Width, sourceMat.Height, TextureFormat.RGB24, false);
         tex.SetPixels32(c);
         tex.Apply();
        GCHandle gcBitsHandle = GCHandle.Alloc(c, GCHandleType.Pinned);
        GCHandle gcMatsHandle = GCHandle.Alloc(mat, GCHandleType.Pinned);
        GCHandle gcVec3bHandle = GCHandle.Alloc(matData, GCHandleType.Pinned);
        GCHandle gcTexture = GCHandle.Alloc(tex, GCHandleType.Pinned);

        gcTexture.Free();
        gcVec3bHandle.Free();
        gcMatsHandle.Free();
        gcBitsHandle.Free();
        return tex;
    }


    public Texture2D WebcamToTexture2D(WebCamTexture sourceCam)
    {
        Texture2D _Texture = new Texture2D(sourceCam.width, sourceCam.height, TextureFormat.RGB24, false);
        _Texture.SetPixels32(sourceCam.GetPixels32());
        _Texture.Apply();
        return _Texture;
    }
    #endregion
    #endregion

    #region "Switchers"

    public bool CanEyeTracking()
    {
        return _canEyeTracking;
    }

    public void SetEyeTracking(bool value)
    {
        _canEyeTracking = value;
    }

    public bool CanDevelopeMode()
    {
        return _canDevelopeMode;
    }

    public void SetDevelopeMode(bool value)
    {
        _canDevelopeMode = value;
    }

    public bool isWebcamFliped()
    {
        return _flipWebCam;
    }

    public void SetWebcamFlip(bool value)
    {
        _flipWebCam = value;
    }

    public bool HaveAxisZ()
    {
        return _moveAxisZ;
    }

    public void HaveAxisZ(bool value)
    {
        _moveAxisZ = value;
    }
    #endregion


    void OnDestroy()
    {
        if(a != null)
            a.Close();

        if (GameManager.Instance.GetWebcam() != null)
        {
            GameManager.Instance.GetWebcam().Stop();
        }
    }
}
