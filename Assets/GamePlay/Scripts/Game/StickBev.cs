using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickBev : MonoBehaviour
{
    public ParticleSystem victoryEffect;
    public ParticleSystem heigherVictoryEffect;
    public ParticleSystem doneEffect;
    public List<MeshRenderer> listLoopModels;
    public GameObject goTop;

    public float distanceHop = 0.3864f;

    public GameObject goX;
    public GameObject goY;
    public GameObject goFull;


    public void Init(int height)
    {
        for(int i = 0; i < listLoopModels.Count; i++)
        {
            listLoopModels[i].enabled = i < height - 1;
        }
        goTop.transform.localPosition = new Vector3(0, distanceHop * (height - 1), 0);
    }

    public void PlayVictoryEffect()
    {

    }
}