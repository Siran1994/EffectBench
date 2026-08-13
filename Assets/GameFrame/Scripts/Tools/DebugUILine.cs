#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*****************************************
	 文件:   DeBugUILine.cs
	 作者:   Siran
	 日期:   2021/4/27 18:48:4
	 功能:   检测UI是否开启射线检测
 *****************************************/
[HelpURL("https://github.com/Siran1994")]

public class DebugUILine : MonoBehaviour
{
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

            }
        }
    }
}
#endif
