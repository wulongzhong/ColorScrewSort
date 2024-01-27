using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITopDiamond : MonoBehaviour
{
    public TMPro.TextMeshProUGUI tDiamond;
    public Transform tfDiamondEffect;
    public Transform tfDiamondIcon;

    private void OnEnable()
    {
        RefreshDiamondCount();
    }

    public void RefreshDiamondCount()
    {
        tDiamond.text = NormalDataHandler.Instance.GoldCount.ToString();
    }

    public async void PlayDiamondFly()
    {
        int oldDiamondCount = NormalDataHandler.Instance.LastGoldCount;
        tDiamond.text = oldDiamondCount.ToString();
        int targetDiamondCount = NormalDataHandler.Instance.GoldCount;
        tfDiamondEffect.gameObject.SetActive(true);

        for(int i = 0; i < tfDiamondEffect.childCount; ++i)
        {
            var transf = tfDiamondEffect.GetChild(i);
            transf.gameObject.SetActive(true);
            transf.localPosition = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), 0);
        }

        for (int i = tfDiamondEffect.childCount - 1; i >= 0; --i)
        {
            int tempI = i;
            var transf = tfDiamondEffect.GetChild(tempI);
            transf.DOMove(tfDiamondIcon.position, 0.5f).OnComplete(() =>
            {
                tDiamond.text = Mathf.RoundToInt(Mathf.Lerp(oldDiamondCount, targetDiamondCount, 1 - tempI * 1.0f / tfDiamondEffect.childCount)).ToString();
                transf.gameObject.SetActive(false);
            });
            await UniTask.Delay(50);
        }
    }
}