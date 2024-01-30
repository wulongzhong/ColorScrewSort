using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearCache : MonoBehaviour
{
    [MenuItem("Tools/清理数据", false, 1)]
    static private void FindScriptRef()
    {
        PlayerPrefs.DeleteAll();
    }
}
