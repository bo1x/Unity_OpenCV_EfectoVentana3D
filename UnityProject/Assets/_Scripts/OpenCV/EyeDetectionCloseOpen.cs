using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class EyeDetectionCloseOpen : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;

    CascadeClassifier eyeCascade = new CascadeClassifier();

    Window ventana;
    



    // Start is called before the first frame update
    void Start()
    {
        
        ventana = new Window("capturawebcam");


        eyeCascade.Load(Application.dataPath + "/haarcascades/haarcascade_eye.xml");

        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[0].name, 640, 480);
        wct.Play();





    }

    // Update is called once per frame
    void Update()
    {


        Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
        SaveTextureAsPNG(text, Application.dataPath, wct);

        using var src = new Mat(Application.dataPath + "textura.png");
        using var dst = new Mat();



        var eyes= eyeCascade.DetectMultiScale(src, 1.3, 5);
        Debug.Log(eyes.Length);
        foreach (var eye in eyes)
        {
            Point centro = new Point(eye.Left+eye.Width/2,eye.Top+eye.Height/2);
            
            int radio = eye.Height / 2;
            //Cv2.Rectangle(src, eye.TopLeft, eye.BottomRight, new Scalar(255, 0, 0), 5);
            Cv2.Circle(src, centro, radio, new Scalar(255, 0, 0), 5);
        }




        //MatToTexture(src, Application.dataPath);




        Cv2.ImWrite(Application.dataPath + "ImagenFinal.png", src);
        ventana.ShowImage(src);

    }
    void OnDestroy()
    {
        ventana.Close();
        
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

    void MatToTexture(Mat sourceMat, string _fullPath)
    {
        //Get the height and width of the Mat 
        int imgHeight = sourceMat.Height;
        int imgWidth = sourceMat.Width;

        byte[] matData = new byte[imgHeight * imgWidth];

        //Get the byte array and store in matData
        sourceMat.GetArray(out matData);
        
        //Create the Color array that will hold the pixels 
        Color32[] c = new Color32[imgHeight * imgWidth];

        //Get the pixel data from parallel loop
        Parallel.For(0, imgHeight, i => {
            for (var j = 0; j < imgWidth; j++)
            {
                byte vec = matData[j + i * imgWidth];
                var color32 = new Color32
                {
                    r = vec,
                    g = vec,
                    b = vec,
                    a = 0
                };
                c[j + i * imgWidth] = color32;
            }
        });

        //Create Texture from the result
        Texture2D tex = new Texture2D(imgWidth, imgHeight, TextureFormat.RGBA32, true, true);
        tex.SetPixels32(c);
        tex.Apply();
        System.IO.File.WriteAllBytes(_fullPath + "MatATextura.png", tex.EncodeToPNG());
    }





}