using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GetColor : MonoBehaviour
{
    //private WebCam webcam;
    //private WebCamTexture tex;
    //int currentCamIndex = 0;

    //public RawImage Pic;

    public Image coloring;
    public Image colhue;
    public Image colsat;
    public Image colval;
    //public RawImage PicIm = default;
    Texture2D tex;
    Color col;
    //Rect r;
    //Vector2 localP;
    int px;
    int py;
    float H;
    float S;
    float V;

    // Resultados finais
    [HideInInspector] public string colorDetected;

    // Start is called before the first frame update
    void Start()
    {
        //webcam = GetComponent<WebCam>();
        //if(!cam.isPlaying) cam.Play();
        //PicIm.texture = cam;

        //WebCamDevice device = WebCamTexture.devices[currentCamIndex];
        //tex = new WebCamTexture(device.name);
        //Photo();
        //Invoke("GetPhoto",1f);
    }

    public IEnumerator GetColorPhoto() 
    {
        yield return new WaitForEndOfFrame();

        tex = ScreenCapture.CaptureScreenshotAsTexture();

        px = tex.width / 2;
        py = tex.height / 2;

        col = tex.GetPixel(px, py);
        Debug.Log("i get called!");
        Color.RGBToHSV(col, out H, out S, out V);
        coloring.color = col;
        colhue.color = Color.HSVToRGB(H, 1, 1);
        colsat.color = Color.HSVToRGB(0, S, 1);
        colval.color = Color.HSVToRGB(0, 0, V);
        Destroy(tex);

        if (V >= 0.0f && V < 0.1f) colorDetected = "black"; //Debug.Log("black.");
        else if (V >= 0.1f && V < 0.9f && S < 0.15f) colorDetected = "grey"; //Debug.Log("grey.");
        else if (V >= 0.9f && S < 0.15f) colorDetected = "white"; //Debug.Log("white.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.917f && H < 0.056f) colorDetected = "red"; //Debug.Log("red.");
        else if (V >= 0.1f && S >= 0.15f && S < 0.5f && H >= 0.056f && H < 0.125f) colorDetected = "brown"; //Debug.Log("brown.");
        else if (V >= 0.1f && S >= 0.5f && H >= 0.056f && H < 0.125f) colorDetected = "orange"; //Debug.Log("orange.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.125f && H < 0.167f) colorDetected = "yellow"; //Debug.Log("yellow.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.167f && H < 0.444f) colorDetected = "green"; //Debug.Log("green.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.444f && H < 0.722f) colorDetected = "blue"; //Debug.Log("blue.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.722f && H < 0.806f) colorDetected = "purple"; //Debug.Log("purple.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.806f && H < 0.917f) colorDetected = "pink"; //Debug.Log("pink.");

	}

    public void StartColorDetection()
    {
        StartCoroutine(GetColorPhoto());
    }
    /*
    // Update is called once per frame
    void Update()
    {
        //PicIm = Pic.GetComponent<RawImage>();
        //cam = webcam.tex;
        //tex = new Texture2D(webcam.tex.width, webcam.tex.height);
        //tex = PicIm.texture as Texture2D;
        //r = PicIm.rectTransform.rect;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(Pic.rectTransform, Input.mousePosition, null, out localP);
        //if(localP.x > r.x && localP.y > r.y && localP.x < (r.width + r.x) && localP.y < (r.height + r.y)) {
        //px = Math.Clamp(0, (int)(((localP.x - r.x) * tex.width) / r.width), tex.width);
        //py = Math.Clamp(0, (int)(((localP.y - r.y) * tex.height) / r.height), tex.height);


        px = webcam.tex.width / 2;
        py = webcam.tex.height / 2;


        col = webcam.tex.GetPixel(px, py);
        Color.RGBToHSV(col, out H, out S, out V);
        coloring.color = col;
        colhue.color = Color.HSVToRGB(H, 1, 1);
        colsat.color = Color.HSVToRGB(0, S, 1);
        colval.color = Color.HSVToRGB(0, 0, V);

        if (V >= 0.0f && V < 0.1f) colorDetected = "black"; //Debug.Log("black.");
        else if (V >= 0.1f && V < 0.9f && S < 0.15f) colorDetected = "grey"; //Debug.Log("grey.");
        else if (V >= 0.9f && S < 0.15f) colorDetected = "white"; //Debug.Log("white.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.917f && H < 0.056f) colorDetected = "red"; //Debug.Log("red.");
        else if (V >= 0.1f && S >= 0.15f && S < 0.5f && H >= 0.056f && H < 0.125f) colorDetected = "brown"; //Debug.Log("brown.");
        else if (V >= 0.1f && S >= 0.5f && H >= 0.056f && H < 0.125f) colorDetected = "orange"; //Debug.Log("orange.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.125f && H < 0.167f) colorDetected = "yellow"; //Debug.Log("yellow.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.167f && H < 0.444f) colorDetected = "green"; //Debug.Log("green.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.444f && H < 0.722f) colorDetected = "blue"; //Debug.Log("blue.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.722f && H < 0.806f) colorDetected = "purple"; //Debug.Log("purple.");
        else if (V >= 0.1f && S >= 0.15f && H >= 0.806f && H < 0.917f) colorDetected = "pink"; //Debug.Log("pink.");
    }*/
}
