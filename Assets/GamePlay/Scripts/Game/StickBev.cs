using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;

public class StickBev : MonoBehaviour
{
    public class StickEndEvent : EventArgsBase
    {
        public override void Clear()
        {
        }
    }

    public ParticleSystem victoryEffect;
    public ParticleSystem heigherVictoryEffect;
    public ParticleSystem doneEffect;
    [SerializeField]
    private List<MeshRenderer> listLoopModels;
    public GameObject goTop;

    public const float distanceHop = 0.3864f;

    public GameObject goX;
    public GameObject goY;
    public GameObject goFull;

    public List<NutBev> listNutBev = new List<NutBev>();

    public bool bEnd { get; set; } = false;
    public bool bMoving { get; set; } = false;

    public int topColor { get; private set; } = -1;
    public int topColorCount { get; private set; } = 0;

    public void Init(int height)
    {
        for(int i = 0; i < listLoopModels.Count; i++)
        {
            listLoopModels[i].enabled = i < height - 1;
        }
        goTop.transform.localPosition = new Vector3(0, distanceHop * (height - 1) + 0.2f, 0);

        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, (height + 1) * distanceHop * 0.5f, 0);
        boxCollider.size = new Vector3(1, height * distanceHop, 1);
    }

    public void RefreshState()
    {
        topColor = -1;
        topColorCount = 0;
        if(listNutBev.Count > 0)
        {
            topColor = listNutBev[^1].color;
            if (listNutBev[^1].bMasking)
            {
                listNutBev[^1].ClearMask();
            }
            topColorCount = 1;
            for(int i = listNutBev.Count - 2; i >= 0; --i)
            {
                if (listNutBev[i].color == topColor && !listNutBev[i].bMasking)
                {
                    ++topColorCount;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void RefreshEffect()
    {
        if (topColorCount == LevelPlayMgr.Instance.levelHeight)
        {
            if (!bEnd)
            {
                bEnd = true;
                PlayVictoryEffect();
            }
        }
        else
        {
            if (bEnd)
            {
                bEnd = false;
                StopVictoryEffect();
            }
        }
    }

    public void PlayVictoryEffect()
    {
        if (LevelPlayMgr.Instance.levelHeight <= 5)
        {
            victoryEffect.gameObject.SetActive(true);
            victoryEffect.Play();
        }
        else
        {
            heigherVictoryEffect.gameObject.SetActive(true);
            heigherVictoryEffect.Play();
        }
        doneEffect.gameObject.SetActive(true);
        float size = goTop.transform.localPosition.y + 0.5f;
        doneEffect.transform.localPosition = new Vector3(0, size / 2 + 0.3f, 0);
        var shape = doneEffect.shape;
        shape.scale = new Vector3(1, 1, size);
        doneEffect.Play();
        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<StickEndEvent>());
    }

    public void StopVictoryEffect()
    {
        victoryEffect.gameObject.SetActive(false);
        heigherVictoryEffect.gameObject.SetActive(false);
        doneEffect.gameObject.SetActive(false);
        doneEffect.Stop();
    }
}