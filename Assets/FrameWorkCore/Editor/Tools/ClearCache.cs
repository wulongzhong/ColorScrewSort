using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearCache : MonoBehaviour
{
    [MenuItem("Tools/��������", false, 1)]
    static private void FindScriptRef()
    {
        PlayerPrefs.DeleteAll();
    }
}
