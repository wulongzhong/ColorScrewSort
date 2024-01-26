using ConfigPB;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class LevelPlayMgr : MonoBehaviour
{
    public class LevelWinEvent : EventArgsBase
    {
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

    public struct NutMoveRecord
    {
        public StickBev startStickBev;
        public StickBev endStickBev;
        public int count;
    }

    public static LevelPlayMgr Instance;

    private const int maxStickCount = 15;
    private const int maxRowStickCount = 5;
    private const float upMoveSpeed = 6;
    private const float downMoveSpeed = 3;

    public Camera mainCamera;
    public MeshRenderer rendererBackGround;
    public GameObject stickPrefab;
    public GameObject nutPrefab;
    public List<Material> listColorMat;

    private Material matBackGround;
    private ResLoader matBackGroundResLoader;

    private LevelData levelData;
    public int levelHeight;
    private List<StickBev> listStickBev;
    public Stack<NutMoveRecord> nutMoveRecords;
    public int currSelectStickIndex = -1;

    public bool bInitViewing = false;

    private bool bCurrHardLevel = false;


    private void Awake()
    {
        Instance = this;
        matBackGround = new Material(rendererBackGround.material);
        rendererBackGround.material = matBackGround;
    }

    public void Init()
    {
        RefreshBGSkin();
        listStickBev = new List<StickBev>();
        nutMoveRecords = new Stack<NutMoveRecord>();
        //LoadLevel(NormalDataHandler.Instance.CurrLevelId, false);
        LoadLevel(34, false);
        //LoadSpecialLevel(3);
    }

    public void RefreshBGSkin()
    {
        if(matBackGroundResLoader != null)
        {
            matBackGroundResLoader.Dispose();
            matBackGroundResLoader = null;
        }
        matBackGroundResLoader = new ResLoader();

        matBackGround.mainTexture = matBackGroundResLoader.LoadAsset<Texture2D>("Assets/ExRes/Texture2D/theme8.png");
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
        if (bInitViewing)
        {
            return;
        }
        if (NormalDataHandler.Instance.CurrNormalLevelIsHard)
        {
            NormalDataHandler.Instance.CurrNormalLevelId++;
            NormalDataHandler.Instance.CurrNormalLevelIsHard = false;
        }
        else
        {
            NormalDataHandler.Instance.CurrNormalLevelIsHard = true;
        }

        LoadLevel(NormalDataHandler.Instance.CurrNormalLevelId, NormalDataHandler.Instance.CurrNormalLevelIsHard);
    }

    public void RefreshLevel()
    {
        if (bInitViewing)
        {
            return;
        }
        ClearLevel();
        InitLevelView();
    }

    public void LoadLevel(int levelId, bool bHard)
    {
        ClearLevel();
        ResLoader resLoader = new ResLoader();

        string levelPath;
        if (!bHard)
        {
            bCurrHardLevel = false;
            levelPath = $"Assets/GamePlay/DataTable/level/Lv_{levelId}.txt";
        }
        else
        {
            bCurrHardLevel = true;
            levelPath = $"Assets/GamePlay/DataTable/levelhard/Lv_{levelId}.txt";
        }
        levelData = LevelData.Parser.ParseJson(resLoader.LoadAsset<TextAsset>(levelPath).text);
#if ENABLE_LOG
        Debug.Log(levelData);
#endif
        levelHeight = 0;
        foreach (var stickData in levelData.Data)
        {
            if(stickData.Stick.Count > levelHeight)
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
        bCurrHardLevel = false;
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
        bInitViewing = true;
        for (int i = 0; i < levelData.Data.Count; i++)
        {
            var goStick = Instantiate(stickPrefab, this.transform);
            StickBev stickBev = goStick.GetComponent<StickBev>();
            stickBev.Init(levelHeight);
            listStickBev.Add(stickBev);
        }

        RefreshStickPos();

        for (int i = 0; i < levelData.Data.Count; i++)
        {
            CreateStickNuts(i);
        }
    }

    private async void CreateStickNuts(int stickIndex)
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
            nutBev.Init(colors[j], bCurrHardLevel);
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
            float moveTime = (nutBev.transform.position.y - targetY) / downMoveSpeed;
            nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime);
            nutBev.transform.eulerAngles = Vector3.zero;
            nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime, RotateMode.LocalAxisAdd);
            await UniTask.Delay(400);
        }
        stickBev.bMoving = false;
        stickBev.RefreshState();
        if(colors.Count > 0 && colors[0] != 0)
        {
            bInitViewing = false;
        }
    }

    public GameObject CreateNut(int color)
    {
        var go = Instantiate(nutPrefab);
        go.transform.localScale = Vector3.one;
        go.transform.Find("Model").GetComponent<MeshRenderer>().material = listColorMat[color - 1];
        return go;
    }

    public void RefreshStickPos()
    {
        int rowCount = 1;
        if(listStickBev.Count > 10)
        {
            rowCount = 3;
        }
        else if(listStickBev.Count > 4)
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
                if(pos.y < rowStickCount[r])
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
            if(rowCount == 1)
            {
                viewPos.z -= 3.0f;
            }

            if(rowCount == 2)
            {
                viewPos.z -= 0.5f;
            }

            if(rowCount == 3)
            {
                viewPos.z += 2.5f;
            }

            viewPos.x = pos.y * 1.4f - (rowStickCount[r] - 1) * 0.5f * 1.4f;
            listStickBev[i].transform.position = viewPos;
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse0))
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
#if ENABLE_LOG
                Debug.Log(selectStickIndex);
#endif
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

    private bool PopStickFirst(StickBev stickBev)
    {
        if(stickBev.listNutBev.Count == 0)
        {
            return false;
        }
        var nutBev = stickBev.listNutBev[^1];
        float targetY = stickBev.goTop.transform.position.y + 1;
        float moveTime = (targetY - nutBev.transform.position.y) / upMoveSpeed;
        nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime);
        nutBev.transform.eulerAngles = Vector3.zero;
        nutBev.transform.DORotate(new Vector3(0, -720, 0), moveTime, RotateMode.LocalAxisAdd);
        return true;
    }

    private void CancelPopStickFirst(StickBev stickBev)
    {
        var nutBev = stickBev.listNutBev[^1];
        float targetY = StickBev.distanceHop * nutBev.currPosY + 0.3f;
        float moveTime = (nutBev.transform.position.y - targetY) / downMoveSpeed;
        nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime);
        nutBev.transform.eulerAngles = Vector3.zero;
        nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime, RotateMode.LocalAxisAdd);
    }

    private async void MoveNut(StickBev startStickBev, StickBev endStickBev, int moveCount = 0)
    {
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

        for (int tempI = 0; tempI < moveCount; ++tempI)
        {
            int i = tempI;
            var nutBev = endStickBev.listNutBev[endStickBev.listNutBev.Count - moveCount + i];

            float moveTime = (topPosY - nutBev.transform.position.y) / upMoveSpeed;
            if(i > 0)
            {
                nutBev.transform.DOLocalMove(new Vector3(0, topPosY, 0), moveTime);
                nutBev.transform.eulerAngles = Vector3.zero;
                nutBev.transform.DORotate(new Vector3(0, -720, 0), moveTime, RotateMode.LocalAxisAdd);
                await UniTask.Delay(Mathf.RoundToInt(moveTime * 1000) + 50);
            }

            nutBev.transform.parent = null;
            nutBev.transform.DOMove(endStickBev.goTop.transform.position + new Vector3(0, 1, 0), 0.2f);

            if (i == moveCount - 1)
            {
                startStickBev.bMoving = false;
                startStickBev.RefreshState();
            }

            _ = UniTask.Create(
             async () =>
             {
                 await UniTask.Delay(250);
                 nutBev.transform.parent = endStickBev.transform;

                 float targetY = StickBev.distanceHop * nutBev.currPosY + 0.3f;
                 moveTime = (nutBev.transform.position.y - targetY) / downMoveSpeed;
                 nutBev.transform.DOLocalMove(new Vector3(0, targetY, 0), moveTime).OnComplete(() =>
                 {
                     nutBev.PlayDownEffect();
                 });
                 nutBev.transform.eulerAngles = Vector3.zero;
                 nutBev.transform.DORotate(new Vector3(0, 720, 0), moveTime, RotateMode.LocalAxisAdd);
                 if (i == moveCount - 1)
                 {
                     await UniTask.Delay(Mathf.RoundToInt(moveTime * 1000));
                 }
             });
        }

        
        endStickBev.bMoving = false;
        endStickBev.RefreshState();

        CheckCanContinueGame();
        CheckWin();

        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<LevelMoveNutEvent>());
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

    public async void CheckWin()
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
            await UniTask.Delay(1000);
        }

        GpEventMgr.Instance.PostEvent(EventArgsPool.Get<LevelWinEvent>());
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
    }
}