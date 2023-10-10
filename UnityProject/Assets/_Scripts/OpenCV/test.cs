using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Unity.VisualScripting;

public class test : MonoBehaviour
{
    public WebCamTexture wct;
    WebCamDevice[] wdevice;
    public int thresshold1, thresshold2;
    Window a;
    Window b;
    Window c;
    Window d;
    public int tamañoX = 1, tamañoY = 1;
    Size sizeZ;

    Mat[] arrayMat = new Mat[4];

    // Start is called before the first frame update
    void Start()
    {
        a = new Window("a image");
  
        
        wdevice = WebCamTexture.devices;
        foreach (WebCamDevice device in wdevice)
        {
            Debug.Log("name" + device.name);
            Debug.Log("");
        }

        wct = new WebCamTexture(wdevice[0].name, 640, 480);
        wct.Play();

        sizeZ.Width = tamañoX;
        sizeZ.Height = tamañoY;








    }

    // Update is called once per frame
    void Update()
    {
            sizeZ.Width = tamañoX;
            sizeZ.Height = tamañoY;
            Texture2D text = GameManager.Instance.WebcamToTexture2D(wct);
            SaveTextureAsPNG(text, Application.dataPath, wct);

            using var src = new Mat(Application.dataPath + "textura.png",ImreadModes.Grayscale);
            using var dst = new Mat();

            using var src2 = new Mat();
            using var dst2 = new Mat();


            Cv2.Blur(src, src2, sizeZ);
            Cv2.Canny(src, dst, thresshold1, thresshold2);
            Cv2.Canny(src2, dst2, thresshold1, thresshold2);

            arrayMat[0] = src2;
            arrayMat[1] = dst;
            arrayMat[2] = dst2;
            arrayMat[3] = dst2;
        
            Cv2.HConcat(arrayMat, src);
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

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath,WebCamTexture wct)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        _texture.SetPixels(wct.GetPixels());
        _texture.Apply();
        System.IO.File.WriteAllBytes(_fullPath + "textura.png", _texture.EncodeToPNG());
        
    }


    


}
