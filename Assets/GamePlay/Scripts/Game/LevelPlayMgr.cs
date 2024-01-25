using ConfigPB;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LevelPlayMgr : MonoBehaviour
{
    public static LevelPlayMgr Instance;

    private const int maxStickCount = 12;
    private const int maxRowStickCount = 5;

    public Camera mainCamera;
    public MeshRenderer rendererBackGround;
    public GameObject stickPrefab;
    public GameObject nutPrefab;
    public List<Material> listColorMat;

    private LevelData levelData;
    private int levelHeight;

    private List<StickBev> listStickBev;

    private Material matBackGround;
    private ResLoader matBackGroundResLoader;

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
        //LoadLevel(NormalDataHandler.Instance.CurrLevelId, false);
        LoadLevel(34, false);
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

    public void LoadLevel(int levelId, bool bHard)
    {
        ResLoader resLoader = new ResLoader();

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
        InitLevelView();
    }

    public async void InitLevelView()
    {
        for(int i = 0; i < levelData.Data.Count; i++)
        {
            var currData = levelData.Data[i];
            var goStick = Instantiate(stickPrefab, this.transform);
            StickBev stickBev = goStick.GetComponent<StickBev>();
            stickBev.Init(levelHeight);
            listStickBev.Add(stickBev);
        }

        RereshStickPos();


        for(int i = 0; i < levelData.Data.Count; i++)
        {
            var stick = levelData.Data[i].Stick;
            for (int j = 0; j < stick.Count; ++j)
            {
                var nut = CreateNut(stick[j]);
                nut.transform.parent = listStickBev[i].transform;
                nut.transform.position = listStickBev[i].transform.position + new Vector3(0, 6, 0);
                nut.transform.DOLocalMove(new Vector3(0, StickBev.distanceHop * j + 0.3f, 0), 1.0f);
                await UniTask.Delay(50);
            }
        }
    }

    public GameObject CreateNut(int color)
    {
        var go = Instantiate(nutPrefab);
        go.transform.localScale = Vector3.one;
        go.transform.Find("Model").GetComponent<MeshRenderer>().material = listColorMat[color + 1];
        return go;
    }

    public void RereshStickPos()
    {
        int rowCount = 1;
        if(listStickBev.Count > 9)
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
            rowStickCount.Add(listStickBev.Count / 3 + (listStickBev.Count % 3 == 2 ? 1 : 0));
            rowStickCount.Add(listStickBev.Count / 3 + (listStickBev.Count % 3 > 0 ? 1 : 0));
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
            viewPos.z = (pos.x) * -5;
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
}