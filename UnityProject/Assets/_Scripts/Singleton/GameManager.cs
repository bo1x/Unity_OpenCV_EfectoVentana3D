using OpenCvSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Diagnostics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private WebCamTexture _webcam;
    private bool _canEyeTracking;
    private bool _canDevelopeMode;

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
    }

    public void SetWebcam( WebCamTexture actualWebcam )
    {
        if (_webcam != null)
        {
            if (_webcam.deviceName == actualWebcam.deviceName)
                return;

            _webcam.Stop();
        }

        _webcam = actualWebcam;

        //GameManager.Instance.WebCamConstructor(60, 640, 360);
        _webcam.Play();
        Debug.Log(_webcam.deviceName);
        //mas paridas para el inicio de la webcam
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
        _webcam.requestedFPS = fps;
        _webcam.requestedHeight = height;
        _webcam.requestedWidth = width;
    }

    public WebCamTexture GetWebcam()
    {
        return _webcam;
    }


    //OpenCV Things

    public Mat WebCamMat()
    {
        return GameManager.Instance.TextureToMat(GameManager.Instance.WebcamToTexture2D(GameManager.Instance.GetWebcam()));
    }

    public Mat TextureToMat(Texture2D sourceTexture)
    {
        
        int imgHeight = sourceTexture.height;
        int imgWidth = sourceTexture.width;

        Color32[] c = sourceTexture.GetPixels32();
        Debug.Log(c[50]);

        byte[] matData = new byte[imgHeight * imgWidth];
        Vec4b[] videoSourceImageData = new Vec4b[imgHeight * imgWidth]; ;
        
        Parallel.For(0, imgHeight, i =>
        {
            for (var j = 0; j < imgWidth; j++)
            {
                var col = c[j + i * imgWidth];
                var vec3 = new Vec4b
                {
                    Item0 = col.b,
                    Item1 = col.g,
                    Item2 = col.r,
                    Item3 = col.a
                };

                videoSourceImageData[j + i * imgWidth] = vec3;
                //Debug.Log(videoSourceImageData[x + i * imgWidth] + " " + vec3 + col);
            }
        });

        Mat mat = new Mat(imgHeight, imgWidth, MatType.CV_8UC4);

        Debug.Log(mat.Width);
        mat.SetArray(videoSourceImageData);
        Debug.Log(c[600]);
        Debug.Log(videoSourceImageData[600]);



        Cv2.Flip(mat, mat, FlipMode.X);

        return mat;
    }
        

    public Texture2D MatToTexture(Mat sourceMat)
    {
        Cv2.Flip(sourceMat, sourceMat, FlipMode.X);

        int imgHeight = sourceMat.Height;
        int imgWidth = sourceMat.Width;

        Vec4b[] matData = new Vec4b[imgHeight * imgWidth];

        sourceMat.GetArray(out matData);

        Color32[] c = new Color32[imgHeight * imgWidth];

         Parallel.For(0, imgHeight, i => {
             for (var x = 0; x < imgWidth; x++)
             {
                 var color32 = new Color32
                 {
                     r = matData[x + i * imgWidth].Item2,
                     g = matData[x + i * imgWidth].Item1,
                     b = matData[x + i * imgWidth].Item0,
                     a = matData[x + i * imgWidth].Item3
                 };
                 c[x + i * imgWidth] = color32;
             }
         });

         Texture2D tex = new Texture2D(imgWidth, imgHeight, TextureFormat.RGBA32, true, true);
         tex.SetPixels32(c);
         tex.Apply();

        Debug.Log(c[600]);
        Debug.Log(matData[600]);

        return tex;
    }


    public Texture2D WebcamToTexture2D(WebCamTexture sourceCam)
    {
        Texture2D _Texture = new Texture2D(sourceCam.width, sourceCam.height);
        _Texture.SetPixels32(sourceCam.GetPixels32());
        _Texture.Apply();
        return _Texture;
    }

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

}
