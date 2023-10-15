using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

public class CamFce : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;

    CascadeClassifier faceCascade = new CascadeClassifier();
    CascadeClassifier eyeCascade = new CascadeClassifier();

    int AltoCam = 0;
    int AnchoCam = 0;

    public int X, Y, Z;

    Window a;
    // Start is called before the first frame update
    void Start()
    {
        a = new Window("capturawebcam");

        faceCascade.Load(Application.dataPath + "/haarcascades/haarcascade_frontalface_default.xml");
        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[0].name, 640, 480);
        wct.Play();


        AltoCam = wct.height;
        AnchoCam = wct.width;


    }

    // Update is called once per frame
    void Update()
    {
        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        SaveTextureAsPNG(text, Application.dataPath, wct);

        using var src = new Mat(Application.dataPath + "textura.png", ImreadModes.Grayscale);
        Cv2.Flip(src, src, FlipMode.Y);
        using var dst = new Mat();

        
        var faces = faceCascade.DetectMultiScale(src, 1.3, 5);
        foreach (var face in faces)
        {

            X = face.Left + face.Width / 2; 
            X = X - wct.width / 2;
            Y = face.Top + face.Height / 2;
            Y = Y - wct.height / 2;
            Z = face.Height;
            Y = -Y;
            
            
            Point centro = new Point(face.Left + face.Width / 2, face.Top + face.Height / 2);
            Scalar color = new Scalar(0, 0, 255);
            Cv2.Circle(src, centro, 1, color, 5);
        }
        
        

        //gameObject.transform.position = new Vector3(0+X / 30, 0+Y / 30, 0 + Z / 30);
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(0 + X / 30, 0 + Y / 30, 0 + Z / 30), 0.3f);



        a.ShowImage(src);
    }
    void OnDestroy()
    {
        a.Close();
        if (wct != null)
        {
            wct.Stop();
        }
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath, WebCamTexture wct)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        _texture.SetPixels(wct.GetPixels());
        _texture.Apply();
        System.IO.File.WriteAllBytes(_fullPath + "textura.png", _texture.EncodeToPNG());

    }





}
