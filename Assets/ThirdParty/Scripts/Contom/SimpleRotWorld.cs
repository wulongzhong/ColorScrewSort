using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotWorld : MonoBehaviour
{
    public static SimpleRotWorld Instance;

    public GameObject[] sceneGos;

    public Vector3 initRot;
    public float currScale;

    public Camera mainCamera;
    public Transform ggLightSetup;

    private static bool bHasCache = false;
    private static Vector3 cacheInitRot;

    private void Awake()
    {
        Instance = this;

        if (!bHasCache)
        {
            cacheInitRot = initRot;
            bHasCache = true;
        }
        else
        {
            initRot = cacheInitRot;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        Debug.Log("SimpleRotWorld Start");
        RefreshRot(Vector2.zero);
    }

#if UNITY_EDITOR
    bool bLastHasTouch;
    Vector3 lastMousePos;
#endif

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButton(0))
        {
            if(bLastHasTouch)
            {
                RefreshRot(Input.mousePosition - lastMousePos);
            }

            bLastHasTouch = true;
            lastMousePos = Input.mousePosition;
        }
        else
        {
            bLastHasTouch = false;
        }
#else
        if(Input.touchCount == 1)
        {
            RefreshRot(Input.GetTouch(0).deltaPosition);
        }
#endif
    }

    private void RefreshScale(float scaleDelta)
    {

    }

    private void RefreshRot(Vector2 rotDelta)
    {
        initRot.y += rotDelta.x / 10;
        initRot.x -= rotDelta.y / 10;

        if(initRot.x < 10)
        {
            initRot.x = 10;
        }
        if(initRot.x > 45)
        {
            initRot.x = 45;
        }

        mainCamera.transform.eulerAngles = initRot;
        Vector3 dir = Vector3.zero;
        var yAngle = initRot.y / 180 * Mathf.PI;
        dir.x = 0 - Mathf.Sin(yAngle);
        dir.z = 0 - Mathf.Cos(yAngle);
        dir.Normalize();
        dir.y = Mathf.Sin(initRot.x / 180  * Mathf.PI);
        dir.x *= Mathf.Cos(initRot.x / 180 * Mathf.PI);
        dir.z *= Mathf.Cos(initRot.x / 180 * Mathf.PI);
        dir.Normalize();
        mainCamera.transform.position = dir * currScale;

        var tempggLightSetupRot = ggLightSetup.eulerAngles;
        tempggLightSetupRot.y = initRot.y;
        ggLightSetup.eulerAngles = tempggLightSetupRot;

        cacheInitRot = initRot;
    }

    public void HideWorld()
    {
        foreach(var go in sceneGos)
        {
            go.SetActive(false);
        }
    }

    public void ShowWorld()
    {
        foreach (var go in sceneGos)
        {
            go.SetActive(true);
        }
    }
}