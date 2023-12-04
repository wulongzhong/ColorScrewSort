using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public enum UIBaseAniType
    {
        Open,
        Close,
    }
    private Animator animator;
    private GraphicRaycaster raycaster;
    [HideInInspector]
    public ResLoader resLoader;

    [Header("���ػ�ģ������")]
    public ConfigPB.LocalizationModuleType localizationModuleType;
    [Header("Ĭ�ϰ�ť��ЧID")]
    public int btnDefaultClickSoundId = 114;

    [Space]
    [Header("�˴�����Ϊ�Զ����ɣ����ֶ��༭")]
    [Space]

    public TextMeshProUGUI[] arrStaticLocalizationText;
    public Button[] arrPlaySoundBtn;
    public int[] arrBtnSoundId;

    /// <summary>
    /// �Ƿ��ڲ��ſ���������
    /// </summary>
    public bool BPlayingOpenAnimation {  get; set; }
    /// <summary>
    /// �Ƿ��ڲ��Źرն�����
    /// </summary>
    public bool BPlayingCloseAnimation { get; set; }
    /// <summary>
    /// ��������ʱ�� ����
    /// </summary>
    public float AnimationEndTime { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        raycaster = GetComponent<GraphicRaycaster>();
        OnInit();
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnOpen(object userData)
    {
        BPlayingOpenAnimation = false;
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Open.ToString())))
            {
                raycaster.enabled = false;
                animator.Play(UIBaseAniType.Open.ToString());
                BPlayingOpenAnimation = true;
                AnimationEndTime = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
        raycaster.enabled = true;
    }

    public virtual void OnOpenAnimationEnd()
    {
        raycaster.enabled = true;
        BPlayingOpenAnimation = false;
    }

    public virtual void OnClose(bool bRecycle = true)
    {
        BPlayingCloseAnimation = false;
        raycaster.enabled = false;
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Close.ToString())))
            {
                animator.Play(UIBaseAniType.Close.ToString());
                BPlayingCloseAnimation = true;
                AnimationEndTime = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
    }

    public virtual void OnCloseAnimationEnd(bool bRecycle = true)
    {
        BPlayingCloseAnimation = false;
        if (!bRecycle)
        {
            OnRealDestory();
        }
    }

    public virtual void OnRealDestory()
    {
        Destroy(gameObject);
        resLoader.Dispose();
        resLoader = null;
    }

    protected virtual void OnUpdate()
    {
        if (BPlayingOpenAnimation && Time.time > AnimationEndTime)
        {
            OnOpenAnimationEnd();
        }
        if (BPlayingCloseAnimation && Time.time > AnimationEndTime)
        {
            OnCloseAnimationEnd();
        }
    }
}