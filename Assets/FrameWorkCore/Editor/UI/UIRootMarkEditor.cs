using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRootMark))]
public class UIRootMarkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIRootMark uiRootMark = target as UIRootMark;

        if (GUILayout.Button("����CS�����"))
        {
            UICSCodeGenerate codeGenerate = new UICSCodeGenerate();
            codeGenerate.Generate(uiRootMark.gameObject);
        }

        if (GUILayout.Button("����CS��Ԥ����"))
        {
            UICSCodeGenerate.DoCreateUIPf(uiRootMark.gameObject);
        }
    }
}
