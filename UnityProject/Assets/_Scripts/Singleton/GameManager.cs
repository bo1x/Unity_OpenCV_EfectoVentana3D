using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Diagnostics;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private WebCamTexture _webcam;
    private bool _flipWebCam = true;
    private int requestedFps = 30;
    private bool showWebcam = false;
    private Vector2Int requestSize = new Vector2Int(640, 360);

    private bool _canEyeTracking;
    private bool _canDevelopeMode;

    private int _targetFrameRate = 30;

    Color32[] c;
    Vec3b[] videoSourceImageData;
    Mat mat;
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

    public int X, Y, Z;

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


        debugWindow();
        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");
    }

    private void Update()
    {

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
        _webcam.requestedFPS = requestedFps;
        _webcam.requestedHeight = requestSize.x;
        _webcam.requestedWidth = requestSize.y;
        _webcam.Stop();
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
    #endregion

    #region OpenCV
    //OpenCV Things

    public Mat WebCamMat()
    {
        int imgHeight = _webcam.height;
        int imgWidth = _webcam.width;

        Mat mat = new Mat(imgHeight, imgWidth, MatType.CV_8UC4);
        mat = TextureToMat(WebcamToTexture2D(_webcam));
        
        if(_flipWebCam)
            Cv2.Flip(mat, mat, FlipMode.Y);

        return mat;
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


            Debug.Log(face.Top);

            if (faces.Length == 1 || face.Top < lastFace.Top + offsetFace && face.Top > lastFace.Top - offsetFace && face.Left < lastFace.Left + offsetFace && face.Left > lastFace.Left - offsetFace)
            {

                X = face.Left + face.Width / 2;
                X = X - GetWebcam().width / 2;
                Y = face.Top + face.Height / 2;
                Y = Y - GetWebcam().height / 2;
                Z = face.Height;
                Y = -Y;


                Point centro = new Point(face.Left + face.Width / 2, face.Top + face.Height / 2);
                Scalar color = new Scalar(0, 0, 255);
                Cv2.Circle(faceMat, centro, 1, color, 5);
                Cv2.Rectangle(faceMat, face.TopLeft, face.BottomRight, new Scalar(0, 255, 0), 2);
            }
        }

        return faceMat;
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
    #endregion


    void OnDestroy()
    {
        a.Close();
        if (GameManager.Instance.GetWebcam() != null)
        {
            GameManager.Instance.GetWebcam().Stop();
        }
    }
}
