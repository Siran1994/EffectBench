#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
/*****************************************
	 文件:   EditorOnly.cs
	 作者:   Siran
	 日期:   2021/3/9 14:42:40
	 功能:   特殊标签不会打进包
 *****************************************/
#pragma warning disable 0108, 0649, 0414
public class EditorOnly : MonoBehaviour
{
    [HideInInspector]
    public string tag = "Untagged";
    void OnDrawGizmos()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
        {
            UnityEditor.Handles.Label(go.transform.position, tag);
        }
    }
}

[CustomEditor(typeof(EditorOnly))]
public class EditorOnlyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorOnly gizmos = target as EditorOnly;
        EditorGUI.BeginChangeCheck();
        gizmos.tag = EditorGUILayout.TagField("Tag for Objects:", gizmos.tag);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(gizmos);
        }
    }
}
#endif
