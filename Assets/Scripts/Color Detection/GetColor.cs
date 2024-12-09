using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GetColor : MonoBehaviour
{
    public RawImage Pic;
    public Image coloring;
    public Image colhue;
    public Image colsat;
    public Image colval;
    RawImage PicIm;
    Texture2D tex;
    Color col;
    Rect r;
    Vector2 localP;
    int px;
    int py;
    float H;
    float S;
    float V;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PicIm = Pic.GetComponent<RawImage>();
        tex = Pic.texture as Texture2D;
        r = Pic.rectTransform.rect;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(Pic.rectTransform, Input.mousePosition, null, out localP);
        if(localP.x > r.x && localP.y > r.y && localP.x < (r.width + r.x) && localP.y < (r.height + r.y)) {
            px = Math.Clamp(0, (int)(((localP.x - r.x) * tex.width) / r.width), tex.width);
            py = Math.Clamp(0, (int)(((localP.y - r.y) * tex.height) / r.height), tex.height);

            col = tex.GetPixel(px,py);
            Color.RGBToHSV(col, out H, out S, out V);
            coloring.color = col;
            colhue.color = Color.HSVToRGB(H, 1, 1);
            colsat.color = Color.HSVToRGB(0, S, 1);
            colval.color = Color.HSVToRGB(0, 0, V);
        }
    }
}
