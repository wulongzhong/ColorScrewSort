using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class NutBev : MonoBehaviour
{
    public int color;
    public bool moving;
    public int currPosY;
    public bool bMasking;

    public Color mainColor;
    public Color emissionColor;

    private Material mat;

    public void Init(int color, bool bMask)
    {
        this.color = color;
        bMasking = bMask;
        if (bMasking)
        {
            var meshRenderer = gameObject.transform.Find("Model").GetComponent<MeshRenderer>();
            mat = new Material(meshRenderer.material);
            meshRenderer.material = mat;
            mainColor = mat.color;
            emissionColor = mat.GetColor("_EmissionColor");

            mat.color = Color.white * (192.0f / 256.0f);
            mat.SetColor("_EmissionColor", Color.clear);
        }
    }

    public void ClearMask()
    {
        if (!bMasking)
        {
            return;
        }
        bMasking = false;
        StartCoroutine(IeClearMask());
    }

    private IEnumerator IeClearMask()
    {
        float startTime = Time.time;
        float ieTime = 0.3f;
        while (true)
        {
            yield return null;
            float offsetTime = Time.time - startTime;
            if(offsetTime > ieTime)
            {
                break;
            }
            float timeScle = offsetTime / ieTime;
            mat.color = Color.Lerp(Color.white * (192.0f / 256.0f), mainColor, timeScle);
            mat.SetColor("_EmissionColor", Color.Lerp(Color.clear, emissionColor, timeScle));
        }
        //mat.renderQueue = 2000;
        mat.color = mainColor;
        mat.SetColor("_EmissionColor", emissionColor);
    }

    public void PlayDownEffect()
    {

    }
}
