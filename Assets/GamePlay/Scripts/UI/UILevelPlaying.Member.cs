using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelPlaying : UIBase
{
    [SerializeField]
    protected UnityEngine.RectTransform panelVictoryStep;
    [SerializeField]
    protected UnityEngine.RectTransform tfPopupStartLevel;
    [SerializeField]
    protected UnityEngine.UI.Button btnRemoveAds;
    [SerializeField]
    protected UnityEngine.UI.Button buttonListLevel;
    [SerializeField]
    protected UnityEngine.UI.Button btnPause;
    [SerializeField]
    protected UnityEngine.UI.Button btnSkip;
    [SerializeField]
    protected UnityEngine.UI.Button btnRollBack;
    [SerializeField]
    protected UnityEngine.UI.Button btnAddStick;
    [SerializeField]
    protected UnityEngine.UI.Button btnRestart;
    [SerializeField]
    protected UnityEngine.UI.Button btnReward;
    [SerializeField]
    protected UnityEngine.UI.Button PopupAddItem;
    [SerializeField]
    protected UnityEngine.UI.Button PopupNoMove;
    [SerializeField]
    protected UITopDiamond topDiamondUI;
    [SerializeField]
    protected UnityEngine.RectTransform tfNoMorePossibleMove;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxNormal;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxHard;
    [SerializeField]
    protected UnityEngine.Animator winAnimator;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tLevel;
    [SerializeField]
    protected UnityEngine.RectTransform step;
    [SerializeField]
    protected UnityEngine.UI.Image tfRollAdd;
    [SerializeField]
    protected UnityEngine.UI.Image tfStickAdd;
    [SerializeField]
    protected UnityEngine.RectTransform tfAddStickTip;
    [SerializeField]
    protected TMPro.TextMeshProUGUI textReward;
    [SerializeField]
    protected UnityEngine.RectTransform tfRewardGem;
    [SerializeField]
    protected UnityEngine.RectTransform tfRewardUndo;
    [SerializeField]
    protected UnityEngine.UI.Image tfRewardStick;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tRollBack;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tAddStick;
    [SerializeField]
    protected UnityEngine.UI.Button btnCloseMoreItem;
    [SerializeField]
    protected UnityEngine.UI.Button btnWatchAdsItem;
    [SerializeField]
    protected UnityEngine.UI.Image imageItemRollBack;
    [SerializeField]
    protected UnityEngine.UI.Image imageItemAddStick;
    [SerializeField]
    protected UnityEngine.UI.Button btnBuyItem;
    [SerializeField]
    protected UnityEngine.UI.Image imageItemRollBack2;
    [SerializeField]
    protected UnityEngine.UI.Image imageItemAddStick2;

}
