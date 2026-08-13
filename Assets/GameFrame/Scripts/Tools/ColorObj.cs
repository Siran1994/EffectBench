using UnityEditor;
using UnityEngine;
/*****************************************
	 文件:   ColorObj.cs
	 作者:   Siran
	 日期:   2020/11/16 14:25:11
	 功能:   高亮展示项目核心游戏对象
 *****************************************/
[DisallowMultipleComponent]
[AddComponentMenu("ColorObj")]
public class ColorObj : MonoBehaviour
{
    public static Texture Icon;

    [Header("对象名字体颜色")]
    public bool applyCustomTextColor = true;
    public Color gameObjectTextColor = Color.red;

    [Header("对象图标")]
    public Texture icon = Icon;

    [Header("背景色")]
    public bool applyBackgroundColor = true;
    public Color backgroundColor = Color.yellow;

    [Header("描述颜色")]
    public bool applyDescription = true;
    public Color descriptionTextColor = Color.green;
    public string description = "广告接入管理类(Singleton)";
}