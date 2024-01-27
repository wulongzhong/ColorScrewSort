using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UILevelPlaying : UIBase
{
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
    protected UnityEngine.ParticleSystem fxNormal;
    [SerializeField]
    protected UnityEngine.ParticleSystem fxHard;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tLevel;
    [SerializeField]
    protected UnityEngine.RectTransform step;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tRollBack;
    [SerializeField]
    protected TMPro.TextMeshProUGUI tAddStick;

}
