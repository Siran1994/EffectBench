using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 扩展方法 for UnityEngine.Vector3.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// 找到离给定点最近的索引
    /// </summary>
    /// <param name="position">世界坐标.</param>
    /// <param name="otherPositions">其它世界坐标.</param>
    /// <returns>最近索引.</returns>
    public static int GetClosest(this Vector3 position, IEnumerable<Vector3> otherPositions)
    {

        var shortestDistance = Mathf.Infinity;
        int index = 0 , t = 0;
        foreach (var otherPosition in otherPositions)
        {
            var distance = (position - otherPosition).sqrMagnitude;
            if (distance < shortestDistance)
            {
                index = t;
                shortestDistance = distance;
            }
            t++;
        }
        return index;
    }

    /// <summary>
    /// 在直线上的投影点
    /// </summary>
    /// <param name="position"></param>
    /// <param name="AP">直线上点A</param>
    /// <param name="BP">直线上点B</param>
    /// <returns></returns>
    public static Vector3 PointPosition(this Vector3 position,Vector3 AP, Vector3 BP)
    {
        Vector3 v = BP - AP;
        if (v.magnitude <= 0.001f)
            return AP;
        return AP + v * ( Vector3.Dot(v , position - AP) / Vector3.Dot(v,v) );
    }

    /// <summary>
    /// 点乘
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Vector3 Mul(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

}
