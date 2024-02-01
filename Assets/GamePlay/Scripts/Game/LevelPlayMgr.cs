using ConfigPB;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameAnalyticsSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

public class LevelPlayMgr : MonoBehaviour
{
    public class LevelWinEvent : EventArgsBase
    {
        public bool bIsNormalLevel;
        public int normalLevelId;
        public bool bNormalIsHard;
        public bool bFinishLevel;
        public bool bSkip;
        public static LevelWinEvent Create(bool bIsNormalLevel, int normalLevelId, bool bNormalIsHard, bool bFinishLevel, bool bSkip)
        {
            LevelWinEvent evt = (LevelWinEvent)EventArgsPool.Get<LevelWinEvent>();
            evt.bIsNormalLevel = bIsNormalLevel;
            evt.normalLevelId = normalLevelId;
            evt.bNormalIsHard = bNormalIsHard;
            evt.bFinishLevel = bFinishLevel;
            evt.bSkip = bSkip;
            return evt;
        }

        public override void Clear()
        {
        }
    }

    public class LevelNotContinueEvent : EventArgsBase
    {
        public override void Clear()
        {
        }
    }

    public class LevelMoveNutEvent : EventArgsBase
    {
        public override void Clear()
        {
        }
    }

    public class LevelPopNutEvent : EventArgsBase
    {
        public override void Clear()
        {
        }
    }

    public struct NutMoveRecord
    {
        public StickBev startStickBev;
        public StickBev endStickBev;
        public int count;
    }

    public static LevelPlayMgr Instance;

    public bool bWaitPlay = true;

    private const int maxStickCount = 15;
    private const int maxRowStickCount = 5;
    [Header("��1����������ٶ�")]
    public float upMoveSpeed = 6;
    [Header("��2����������ٶ�")]
    public float upMoveSpeed2 = 8;
    [Header("��3����������ٶ�")]
    public float upMoveSpeed3 = 10;

    [Header("��1������½��ٶ�")]
    public float downMoveSpeed = 3;
    [Header("��2������½��ٶ�")]
    public float downMoveSpeed2 = 3;
    [Header("��3������½��ٶ�")]
    public float downMoveSpeed3 = 3;


    [Header("��1�����ƽ�Ʒ���ʱ��(����)")]
    public int XZMoveTime = 300;
    [Header("��2�����ƽ�Ʒ���ʱ��(����)")]
    public int XZMoveTime2 = 250;
    [Header("��3�����ƽ�Ʒ���ʱ��(����)")]
    public int XZMoveTime3 = 200;

    [Header("��2���𲽵ȴ�ʱ��")]
    public int UpWaitTime2 = 300;
    [Header("��3���𲽵ȴ�ʱ��")]
    public int UpWaitTime3 = 250;
    [Header("��4���𲽵ȴ�ʱ��")]
    public int UpWaitTime4 = 200;

    [Header("��������")]
    public Ease easeDownMove = Ease.InSine;

    [Header("ƽ�Ʒ�������")]
    public Ease easeXZMove = Ease.OutSine;

    [Header("��������")]
    public Ease easeUpMove = Ease.OutSine;

    public Camera mainCamera;
    public MeshRenderer rendererBackGround;
    public GameObject stickPrefab;
    public GameObject nutPrefab;
    public List<Material> listColorMat;

    private Material matBackGround;
    private ResLoader matBackGroundResLoader;

    public LevelData levelData;
    public int levelHeight;
    private List<StickBev> listStickBev;
    public Stack<NutMoveRecord> nutMoveRecords;
    public int currSelectStickIndex = -1;

    public bool bInitViewing = false;
    public bool bPause = false;

    private void Awake()
    {
        Instance = this;
        matBackGround = new Material(rendererBackGround.material);
        rendererBackGround.material = matBackGround;

#if UNITY_ANDROID
        Vibration.Init();
#endif
    }

    public void Init()
    {
        RefreshBGSkin();
        listStickBev = new List<StickBev>();
        nutMoveRecords = new Stack<NutMoveRecord>();
        //NormalDataHandler.Instance.CurrNormalLevelId = 1;
        LoadLevel(NormalDataHandler.Instance.CurrNormalLevelId, NormalDataHandler.Instance.CurrNormalLevelIsHard);
        //LoadSpecialLevel(3);
    }

    public void RefreshBGSkin()
    {
        if (matBackGroundResLoader != null)
        {
            matBackGroundResLoader.Dispose();
            matBackGroundResLoader = null;
        }
        matBackGroundResLoader = new ResLoader();

        matBackGround.mainTexture = matBackGroundResLoader.LoadAsset<Texture>(DTTheme.Instance.GetThemeByID(NormalDataHandler.Instance.CurrSelectBackGroundId).BGResPath);
    }

    public void RefreshNutSkin(int skinId)
    {
        foreach(var stickBev in listStickBev)
        {
            foreach(var nutBev in stickBev.listNutBev)
            {
                nutBev.RefreshSkin(skinId);
            }
        }
    }

    public void PreviewBGSkin(int themeId)
    {
        if (matBackGroundResLoader != null)
        {
            matBackGroundResLoader.Dispose();
            matBackGroundResLoader = null;
        }
        matBackGroundResLoader = new ResLoader();

        matBackGround.mainTexture = matBackGroundResLoader.LoadAsset<Texture2D>(DTTheme.Instance.GetThemeByID(themeId).BGResPath);
    }

    public void ClearLevel()
    {
        DOTween.Clear();
        foreach (var stick in listStickBev)
        {
            Destroy(stick.gameObject);
        }
        listStickBev.Clear();
        nutMoveRecords.Clear();
        currSelectStickIndex = -1;
    }

    public void SkipLevel()
    {
        if (bInitViewing || bWaitPlay)
        {
            return;
        }
        OnWin(true);
    }

    public void RefreshLevel()
    {
        if (bInitViewing)
        {
            return;
        }
        ClearLevel();
        InitLevelView();
        bWaitPlay = false;
    }

    public void LoadNextNormalLevel()
    {
        LoadLevel(NormalDataHandler.Instance.CurrNormalLevelId, NormalDataHandler.Instance.CurrNormalLevelIsHard);
    }

    public void LoadNextSpecialLevel()
    {
        LoadSpecialLevel(NormalDataHandler.Instance.CurrSpecialLevelId);
    }

    public void LoadLevel(int levelId, bool bHard)
    {
        ClearLevel();
        ResLoader resLoader = new ResLoader();
        NormalDataHandler.Instance.CurrIsNormalLevel = true;
        string levelPath;
        if (!bHard)
        {
            levelPath = $"Assets/GamePlay/DataTable/level/Lv_{levelId}.txt";
        }
        else
        {
            levelPath = $"Assets/GamePlay/DataTable/levelhard/Lv_{levelId}.txt";
        }
        levelData = LevelData.Parser.ParseJson(resLoader.LoadAsset<TextAsset>(levelPath).text);
#if ENABLE_LOG
        Debug.Log(levelData);
#endif
        levelHeight = 0;
        foreach (var stickData in levelData.Data)
        {
            if (stickData.Stick.Count > levelHeight)
            {
                levelHeight = stickData.Stick.Count;
            }
        }

        resLoader.Dispose();
        resLoader = null;

        InitLevelView();
    }

    public void LoadSpecialLevel(int levelId)
    {
        ClearLevel();
        NormalDataHandler.Instance.CurrIsNormalLevel = false;
        ResLoader resLoader = new ResLoader();

        string levelPath = $"Assets/GamePlay/DataTable/speciallevel/Lv_{levelId}.txt";
        levelData = LevelData.Parser.ParseJson(resLoader.LoadAsset<TextAsset>(levelPath).text);
#if ENABLE_LOG
        Debug.Log(levelData);
#endif
        levelHeight = 0;
        foreach (var stickData in levelData.Data)
        {
            if (stickData.Stick.Count > levelHeight)
            {
                levelHeight = stickData.Stick.Count;
            }
        }

        resLoader.Dispose();
        resLoader = null;

        InitLevelView();
    }

    public void InitLevelView()
    {
        bWaitPlay = true;
        bInitViewing = true;
        for (int i = 0; i < levelData.Data.Count; i++)
        {
            var goStick = Instantiate(stickPrefab, this.transform);
            StickBev stickBev = goStick.GetComponent<StickBev>();
            stickBev.Init(levelHeight);
            listStickBev.Add(stickBev);
        }

        RefreshStickPos();
        SoundMgr.Instance.PlaySound("106");
        for (int i = 0; i < levelData.Data.Count; i++)
        {
            CreateStickNuts(i, levelData.TypeLevel == LevelType.Mask);
        }
    }

    private async void CreateStickNuts(int stickIndex, bool bMask)
    {
        var colors = levelData.Data[stickIndex].Stick;
        var stickBev = listStickBev[stickIndex];
        stickBev.bMoving = true;
        for (int j = 0; j < colors.Count; ++j)
        {
            if (colors[j] == 0)
            {
                continue;
            }
            var nutBev = CreateNut(colors[j]).GetComponent<NutBev>();
            nutBev.Init(colors[j], bMask);
            nutBev.currPosY = j;
            stickBev.listNutBev.Add(nutBev);
            nutBev.transform.localScale = Vector3.zero;
            nutBev.transform.DOScale(1, 0.15f);
            nutBev.transform.parent = stickBev.transform;
            nutBev.transform.position = stickBev.goTop.transform.position + new Vector3(0, 1, 0);
            nutBev.transform.eulerAngles = Vector3.zero;
            nutBev.transform.DORotate(new Vector3(0, 360, 0), 0.3f, RotateMode.LocalAxisAdd);
            await UniTask.Delay(300);
            float targetY = StickBev.distanceHop * nutBev.currPosY + 0.3f;
            float moveTime = (nutBev.transform.position.y - targetY) / 6.5f;
            nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime).SetEase(easeDownMove);
            nutBev.transform.eulerAngles = Vector3.zero;
            nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime, RotateMode.LocalAxisAdd).SetEase(Ease.InSine);
            await UniTask.Delay(130);
        }
        stickBev.bMoving = false;
        stickBev.RefreshState();
        if (colors.Count > 0 && colors[0] != 0)
        {
            bInitViewing = false;
            SoundMgr.Instance.StopLoopSound("106");
        }
    }

    public GameObject CreateNut(int color)
    {
        var go = Instantiate(nutPrefab);
        go.transform.localScale = Vector3.one;
        go.GetComponent<NutBev>().mat = listColorMat[color - 1];
        return go;
    }

    public void RefreshStickPos()
    {
        int rowCount = 1;
        if (listStickBev.Count > 10)
        {
            rowCount = 3;
        }
        else if (listStickBev.Count > 4)
        {
            rowCount = 2;
        }

        if (listStickBev.Count == 2)
        {
            mainCamera.orthographicSize = 5;
        }
        else if (listStickBev.Count > 8)
        {
            mainCamera.orthographicSize = 7;
        }
        else
        {
            mainCamera.orthographicSize = 6;
        }

        List<int> rowStickCount = new List<int>();
        if (rowCount == 1)
        {
            rowStickCount.Add(listStickBev.Count);
        }
        else if (rowCount == 2)
        {
            rowStickCount.Add(listStickBev.Count / 2 + (listStickBev.Count % 2 == 0 ? 0 : 1));
            rowStickCount.Add(listStickBev.Count / 2);
        }
        else if (rowCount == 3)
        {
            rowStickCount.Add(listStickBev.Count / 3 + (listStickBev.Count % 3 > 0 ? 1 : 0));
            rowStickCount.Add(listStickBev.Count / 3 + (listStickBev.Count % 3 == 2 ? 1 : 0));
            rowStickCount.Add(listStickBev.Count / 3);
        }

        for (int i = 0; i < listStickBev.Count; i++)
        {
            Vector2Int pos = new Vector2Int(i, i);
            int r = 0;
            for (; r < rowCount; ++r)
            {
                if (pos.y < rowStickCount[r])
                {
                    pos.x = r;
                    break;
                }
                else
                {
                    pos.y -= rowStickCount[r];
                }
            }
            Vector3 viewPos = Vector3.zero;
            viewPos.z = (pos.x) * (-5 - ((levelHeight > 5) ? (levelHeight - 5) * 0.5f : (0)));
            if (rowCount == 1)
            {
                viewPos.z -= 3.0f;
            }

            if (rowCount == 2)
            {
                viewPos.z -= 0.5f;
            }

            if (rowCount == 3)
            {
                viewPos.z += 2.5f;
            }

            viewPos.x = pos.y * 1.4f - (rowStickCount[r] - 1) * 0.5f * 1.4f;
            listStickBev[i].transform.position = viewPos;
        }
    }

    private void Update()
    {
        if (bInitViewing || bWaitPlay || bPause)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            var pos = Input.mousePosition;
            var ray = mainCamera.ScreenPointToRay(pos);
            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                var stickBev = hitInfo.collider.GetComponent<StickBev>();
                if(stickBev.bMoving || stickBev.bEnd)
                {
                    return;
                }

                int selectStickIndex = listStickBev.IndexOf(stickBev);
//#if ENABLE_LOG
//                Debug.Log(selectStickIndex);
//#endif
                if (selectStickIndex == currSelectStickIndex)
                {
                    CancelPopStickFirst(stickBev);
                    currSelectStickIndex = -1;
                }
                else if (currSelectStickIndex == -1)
                {
                    if (PopStickFirst(stickBev))
                    {
                        currSelectStickIndex = selectStickIndex;
                    }
                }
                else
                {
                    var lastStickBev = listStickBev[currSelectStickIndex];

                    bool bCanMove = false;

                    if (stickBev.listNutBev.Count < levelHeight)
                    {
                        if(stickBev.listNutBev.Count == 0 || stickBev.topColor == lastStickBev.topColor)
                        {
                            bCanMove = true;
                        }
                    }

                    if (bCanMove)
                    {
                        MoveNut(lastStickBev, stickBev, 0);
                    }
                    else
                    {
                        CancelPopStickFirst(lastStickBev);
                    }
                    currSelectStickIndex = -1;
                }
            }
            else
            {
                if(currSelectStickIndex != -1)
                {
                    CancelPopStickFirst(listStickBev[currSelectStickIndex]);
                    currSelectStickIndex = -1;
                }
            }
        }
    }

    private void RefreshCanPullTip(StickBev popStickBev)
    {
        foreach(var stick in listStickBev)
        {
            if(stick == popStickBev)
            {
                continue;
            }
            if(stick.listNutBev.Count == levelHeight)
            {
                stick.goFull.SetActive(true);
            }
            else if(stick.listNutBev.Count == 0 || stick.topColor == popStickBev.topColor)
            {
                stick.goY.SetActive(true);
            }
            else
            {
                stick.goX.SetActive(true);
            }
        }
    }

    private void ClearCanPullTip()
    {
        foreach(var stick in listStickBev)
        {
            stick.goX.SetActive(false);
            stick.goY.SetActive(false);
            stick.goFull.SetActive(false);
        }
    }

    private bool PopStickFirst(StickBev stickBev)
    {
        if(stickBev.listNutBev.Count == 0)
        {
            return false;
        }

#if UNITY_ANDROID
        if(PlayerLocalCacheMgr.instance.IsEnableShake)
            Vibration.VibrateAndroid(30);
#endif

        var nutBev = stickBev.listNutBev[^1];
        float targetY = stickBev.goTop.transform.position.y + 1;
        float moveTime = (targetY - nutBev.transform.position.y) / upMoveSpeed;
        if(nutBev.tweener != null && nutBev.tweener.IsActive())
        {
            nutBev.tweener.Kill();
        }
        nutBev.tweener = nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime).SetEase(easeUpMove);
        nutBev.transform.eulerAngles = Vector3.zero;
        SoundMgr.Instance.PlaySound("106");
        nutBev.transform.DORotate(new Vector3(0, -720, 0), moveTime, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            SoundMgr.Instance.StopLoopSound("106");
        });

        if(NormalDataHandler.Instance.CurrNormalLevelId <= 4)
        {
            RefreshCanPullTip(stickBev);
        }

        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<LevelPopNutEvent>());
        return true;
    }

    private async void CancelPopStickFirst(StickBev stickBev)
    {
#if UNITY_ANDROID
        if (PlayerLocalCacheMgr.instance.IsEnableShake)
            Vibration.VibrateAndroid(20);
#endif
        SoundMgr.Instance.PlaySound("106");
        var nutBev = stickBev.listNutBev[^1];
        float targetY = StickBev.distanceHop * nutBev.currPosY + 0.3f;
        float moveTime = (nutBev.transform.position.y - targetY) / downMoveSpeed;
        if (nutBev.tweener != null && nutBev.tweener.IsActive())
        {
            nutBev.tweener.Kill();
        }
        nutBev.tweener = nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime).SetEase(easeDownMove).OnComplete(() =>
        {
            nutBev.PlayDownEffect();
            SoundMgr.Instance.PlaySound("104");
            SoundMgr.Instance.StopLoopSound("106");
            _ = UniTask.Create(
            async () =>
            {
                await UniTask.Delay(150);
#if UNITY_ANDROID
                if (PlayerLocalCacheMgr.instance.IsEnableShake)
                    Vibration.VibrateAndroid(30);
#endif
            }
            );
        });
        nutBev.transform.eulerAngles = Vector3.zero;
        nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime, RotateMode.LocalAxisAdd).SetEase(Ease.InSine);
        if (NormalDataHandler.Instance.CurrNormalLevelId <= 4)
        {
            ClearCanPullTip();
        }
    }

    private async void MoveNut(StickBev startStickBev, StickBev endStickBev, int moveCount = 0)
    {
        if (NormalDataHandler.Instance.CurrNormalLevelId <= 4)
        {
            ClearCanPullTip();
        }

        startStickBev.bMoving = true;
        endStickBev.bMoving = true;

        var topPosY = startStickBev.goTop.transform.position.y + 1;

        var moveColor = startStickBev.topColor;
        if(moveCount == 0)
        {
            moveCount = Mathf.Min(startStickBev.topColorCount, levelHeight - endStickBev.listNutBev.Count);

            NutMoveRecord nutMoveRecord = new NutMoveRecord()
            {
                startStickBev = startStickBev,
                endStickBev = endStickBev,
                count = moveCount
            };
            nutMoveRecords.Push(nutMoveRecord);
        }


        for (int i = 0; i < moveCount; ++i)
        {
            var nutBev = startStickBev.listNutBev[^1];
            nutBev.currPosY = endStickBev.listNutBev.Count;
            startStickBev.listNutBev.Remove(nutBev);
            endStickBev.listNutBev.Add(nutBev);
        }

        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<LevelMoveNutEvent>());

        for (int tempI = 0; tempI < moveCount; ++tempI)
        {
            int i = tempI;
            var nutBev = endStickBev.listNutBev[endStickBev.listNutBev.Count - moveCount + i];

            if(i == 0)
            {
                while(nutBev.tweener != null && nutBev.tweener.IsActive())
                {
                    await UniTask.Delay(30);
                }
            }

            float upSpeed = upMoveSpeed;
            if (i == 1)
            {
                upSpeed = upMoveSpeed2;
            }
            else if (i > 1)
            {
                upSpeed = upMoveSpeed3;
            }

            float moveTime = (topPosY - nutBev.transform.position.y) / upSpeed;
            if(i > 0)
            {
                nutBev.transform.DOLocalMove(new Vector3(0, topPosY, 0), moveTime);
                nutBev.transform.eulerAngles = Vector3.zero;
                nutBev.transform.DORotate(new Vector3(0, -720, 0), moveTime, RotateMode.LocalAxisAdd);
                await UniTask.Delay(Mathf.RoundToInt(moveTime * 1000) + 50);
            }

            nutBev.transform.parent = null;

            int waitTime = UpWaitTime2;
            int flyTime = XZMoveTime;
            if (i == 1)
            {
                flyTime = XZMoveTime2;
                waitTime = UpWaitTime3;
            }
            else if(i > 1)
            {
                flyTime = XZMoveTime3;
                waitTime = UpWaitTime4;
            }

            nutBev.transform.DOMove(endStickBev.goTop.transform.position + new Vector3(0, 1, 0), flyTime * 0.001f).SetEase(easeXZMove);
            await UniTask.Delay(waitTime);


            if (i == moveCount - 1)
            {
                startStickBev.bMoving = false;
                startStickBev.RefreshState();
                startStickBev.RefreshEffect();
            }

            _ = UniTask.Create(
             async () =>
             {
                 //await UniTask.Delay(200);
                 nutBev.transform.parent = endStickBev.transform;
                 SoundMgr.Instance.PlaySound("106");
                 float targetY = StickBev.distanceHop * nutBev.currPosY + 0.3f;

                 float downSpeed = downMoveSpeed;
                 if(i == 1)
                 {
                     downSpeed = downMoveSpeed2;
                 }
                 else if(i > 1)
                 {
                     downSpeed = downMoveSpeed3;
                 }

                 moveTime = (nutBev.transform.position.y - targetY) / downSpeed;
                 nutBev.transform.eulerAngles = Vector3.zero;
                 nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime + 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.InSine);

                 //await UniTask.Delay(100);
                 nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime).SetEase(easeDownMove).OnComplete(() =>
                 {
                     nutBev.PlayDownEffect();
                     SoundMgr.Instance.PlaySound("104");
                     _ = UniTask.Create(
                     async () =>
                     {
                         await UniTask.Delay(150);
#if UNITY_ANDROID
                         if (PlayerLocalCacheMgr.instance.IsEnableShake)
                             Vibration.VibrateAndroid(30);
#endif
                     }
                     );
                 });
                 if (i == moveCount - 1)
                 {
                     await UniTask.Delay(Mathf.RoundToInt(moveTime * 1000));
                     endStickBev.RefreshEffect();
                     CheckCanContinueGame();
                     CheckWin();
                     if (!bInitViewing)
                     {
                         SoundMgr.Instance.StopLoopSound("106");
                     }
                 }
             });
        }

        
        endStickBev.bMoving = false;
        endStickBev.RefreshState();
    }

    public bool CheckCanCancelMove()
    {
        return nutMoveRecords.Count > 0;
    }

    public bool CheckCanContinueGame()
    {
        foreach(var stickBev in listStickBev)
        {
            if(stickBev.topColor == -1)
            {
                return true;
            }
        }

        for(int i = 0; i < listStickBev.Count; ++i)
        {
            var firstStickBev = listStickBev[i];
            for(int j = i + 1; j < listStickBev.Count; ++j)
            {
                var secondStickBev = listStickBev[j];
                if (secondStickBev.bEnd)
                {
                    continue;
                }
                if(firstStickBev.topColor != secondStickBev.topColor)
                {
                    continue;
                }
                if(levelHeight - secondStickBev.listNutBev.Count >= firstStickBev.topColorCount)
                {
                    return true;
                }

                if (levelHeight - firstStickBev.listNutBev.Count >= secondStickBev.topColorCount)
                {
                    return true;
                }
            }
        }
        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<LevelNotContinueEvent>());
        return false;
    }

    public  void CheckWin()
    {
        bool bWin = true;
        foreach (var stickBev in listStickBev)
        {
            if ((!stickBev.bEnd) && stickBev.topColor != -1)
            {
                bWin = false;
                break;
            }
        }
        if (bWin)
        {
            OnWin(false);
        }
    }

    private void OnWin(bool bSkip)
    {
        SoundMgr.Instance.StopLoopSound("106");
        bInitViewing = true;
        bool bFinish = false;
        if(!NormalDataHandler.Instance.CurrIsNormalLevel)
        {
            bFinish = true;
        }
        else
        {
            if(NormalDataHandler.Instance.CurrNormalLevelId <= 4)
            {
                bFinish = true;
            }
            else if (NormalDataHandler.Instance.CurrNormalLevelIsHard)
            {
                bFinish = true;
            }
        }
        GpEventMgr.Instance.PostEvent(LevelWinEvent.Create(
            NormalDataHandler.Instance.CurrIsNormalLevel,
            NormalDataHandler.Instance.CurrNormalLevelId,
            NormalDataHandler.Instance.CurrNormalLevelIsHard,
            bFinish,
            bSkip
        ));

        if (NormalDataHandler.Instance.CurrIsNormalLevel)
        {
            if(NormalDataHandler.Instance.CurrNormalLevelId > 4)
            {
                if (NormalDataHandler.Instance.CurrNormalLevelIsHard)
                {
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{NormalDataHandler.Instance.CurrNormalLevelId}");
                    NormalDataHandler.Instance.CurrNormalLevelId++;
                    NormalDataHandler.Instance.CurrNormalLevelIsHard = false;
                }
                else
                {
                    NormalDataHandler.Instance.CurrNormalLevelIsHard = true;
                }
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"Level_{NormalDataHandler.Instance.CurrNormalLevelId}");
                NormalDataHandler.Instance.CurrNormalLevelId++;
            }
        }
        else
        {
            NormalDataHandler.Instance.CurrSpecialLevelId++;
        }

        SoundMgr.Instance.PlaySound("108");
    }

    public bool CacelMove()
    {
        if (bInitViewing)
        {
            return false;
        }
        if (!CheckCanCancelMove())
        {
            return false;
        }
        var record = nutMoveRecords.Peek();
        if(record.startStickBev.bMoving || record.endStickBev.bMoving)
        {
            return false;
        }
        record = nutMoveRecords.Pop();
        MoveNut(record.endStickBev, record.startStickBev, record.count);
        NormalDataHandler.Instance.PropUndoCount -= 1;
        return true;
    }

    public void AddStick()
    {
        if (bInitViewing)
        {
            return;
        }
        if (listStickBev.Count >= maxStickCount)
        {
            return;
        }

        var goStick = Instantiate(stickPrefab, this.transform);
        StickBev stickBev = goStick.GetComponent<StickBev>();
        stickBev.Init(levelHeight);
        listStickBev.Add(stickBev);
        RefreshStickPos();
        NormalDataHandler.Instance.PropAddRowCount -= 1;
    }
}