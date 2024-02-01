using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesTutorialBorderRendererFade : MonoBehaviour
{
    public Material material;
    private float startTime;

    private float interval = 2;
    private float scale = 0.3f;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        var offsetTime = (Time.time - startTime) % interval;
        if(offsetTime > interval / 2)
        {
            offsetTime = interval - offsetTime;
        }
        offsetTime /= interval;
        material.color = new Color(1,1,1,offsetTime * scale);
    }
}