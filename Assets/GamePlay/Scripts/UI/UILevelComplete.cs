using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelComplete : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }

    private bool animing = true;

    public override void OnInit()
    {
        base.OnInit();

        this.btnNextLevel.onClick.AddListener(() =>
        {
            if (!animing)
            {
                UIMgr.Instance.CloseUI<UILevelComplete>(true);
                LevelPlayMgr.Instance.LoadNextLevel();
                UIMgr.Instance.OpenUI<UIMain>();
            }
        });

        this.btnClaimChest.onClick.AddListener(() => {
            OnClickClaimChest();
        });
        PlayAni();
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        this.tfLevelChest.gameObject.SetActive(false);
        this.tfThemeProcess.gameObject.SetActive(false);
    }

    private async void PlayAni()
    {
        animing = true;
        NormalDataHandler.Instance.GoldCount += 10;
        topDiamondUI.PlayDiamondFly();
        await UniTask.Delay(500);
        //play silder;
        this.tfLevelChest.gameObject.SetActive(true);
        this.tfThemeProcess.gameObject.SetActive(NormalDataHandler.Instance.CurrNormalLevelId >= 5);
        this.tfLevelChest.transform.localScale = Vector3.zero;
        this.tfThemeProcess.transform.localScale = Vector3.zero;
        this.tfLevelChest.transform.DOScale(1, 0.25f);
        if(NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tfThemeProcess.transform.DOScale(1, 0.25f);

        this.fillChest.fillAmount = NormalDataHandler.Instance.CurrChestFragCount * 1.0f / 4;
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.fillTheme.fillAmount = NormalDataHandler.Instance.CurrThemeFragCount * 1.0f / 5;

        this.tChest.text = $"{NormalDataHandler.Instance.CurrChestFragCount}/4";
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tTheme.text = $"{NormalDataHandler.Instance.CurrThemeFragCount}/5";

        await UniTask.Delay(400);

        NormalDataHandler.Instance.CurrChestFragCount++;
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            NormalDataHandler.Instance.CurrThemeFragCount++;

        this.fillChest.DOFillAmount(NormalDataHandler.Instance.CurrChestFragCount * 1.0f / 4, 0.6f);
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.fillTheme.DOFillAmount(NormalDataHandler.Instance.CurrThemeFragCount * 1.0f / 5, 0.6f);

        await UniTask.Delay(600);

        this.tChest.text = $"{NormalDataHandler.Instance.CurrChestFragCount}/4";
        if (NormalDataHandler.Instance.CurrNormalLevelId >= 5)
            this.tTheme.text = $"{NormalDataHandler.Instance.CurrThemeFragCount}/5";

        await UniTask.Delay(1000);

        if (NormalDataHandler.Instance.CurrChestFragCount >= 4)
        {
            PopupWin.gameObject.SetActive(false);
            PopupRewardChest.gameObject.SetActive(true);
            NormalDataHandler.Instance.CurrChestFragCount -= 4;

            bool addPropUndo = UnityEngine.Random.value > 0.5f;

            tfGem.gameObject.SetActive(false);
            tfUndo.gameObject.SetActive(false);
            tfAddRow.gameObject.SetActive(false);

            var gemPos = tfGem.anchoredPosition;
            tfGem.anchoredPosition = Vector3.zero;

            var undoPos = tfUndo.anchoredPosition;
            tfUndo.anchoredPosition = Vector3.zero;

            var addRowPos = tfAddRow.anchoredPosition;
            tfAddRow.anchoredPosition = Vector3.zero;

            await UniTask.Delay(834);

            Par_F.Play();

            await UniTask.Delay(1000);
            idleOpenChest.DOPlay();

            tfGem.gameObject.SetActive(true);
            tfUndo.gameObject.SetActive(addPropUndo);
            tfAddRow.gameObject.SetActive(!addPropUndo);

            tfGem.DOAnchorPos(gemPos, 0.4f);
            if (addPropUndo)
            {
                tfUndo.DOAnchorPos(undoPos, 0.4f);
            }
            else
            {
                tfAddRow.DOAnchorPos(addRowPos, 0.4f);
            }
            await UniTask.Delay(400);
            fxGem.Play();
            if (addPropUndo)
            {
                fxUndo.Play();
            }
            else
            {
                fxAddRow.Play();
            }

            while (PopupRewardChest.gameObject.activeSelf)
            {
                await UniTask.Delay(200);
            }
            await UniTask.Delay(200);
        }

        if (NormalDataHandler.Instance.CurrThemeFragCount >= 5)
        {
            PopupWin.gameObject.SetActive(false);
            PopupRewardTheme.gameObject.SetActive(true);
            NormalDataHandler.Instance.CurrThemeFragCount -= 5;

            while (PopupRewardTheme.gameObject.activeSelf)
            {
                await UniTask.Delay(200);
            }
            await UniTask.Delay(200);
        }

        animing = false;
    }

    private async void OnClickClaimChest()
    {
        tfGem.DOAnchorPosX(0, 0.2f);
        if (tfUndo.gameObject.activeSelf)
        {
            tfUndo.DOAnchorPosX(0, 0.2f);
        }

        if (tfAddRow.gameObject.activeSelf)
        {
            tfAddRow.DOAnchorPosX(0, 0.2f);
        }
        

        await UniTask.Delay(250);

        topDiamondUI.PlayDiamondFly();
        await UniTask.Delay(500);
        PopupRewardChest.gameObject.SetActive(false);
        PopupWin.GetComponent<Animator>().enabled = false;
        PopupWin.gameObject.SetActive(true);
    }

    private void OnClickUseTheme()
    {
        PopupRewardTheme.gameObject.SetActive(false);
    }
    private void OnClickLaterTheme()
    {
        PopupRewardTheme.gameObject.SetActive(false);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnClose(bool bRecycle = true)
    {
        base.OnClose(bRecycle);
        if (EventHandlerIndex != 0)
        {
            this.ClearUpEventHandle();
        }
    }
}
