
/****************************************************
 * FileName:		MathExtensions.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2021-04-01-23:11:30
 * Version:			1.0
 * UnityVersion:	2019.4.8f1
 * Description:		Nothing
 * 
*****************************************************/

/****************************************************
 * FileName:		MathExtensions.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-07-27-22:27:06
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathExtensions 
{

    /// <summary>
    /// ��ˮ��
    /// </summary>
    /// <param name="num">����</param>
    /// <param name="k">ѡȡ��</param>
    /// <returns>��������</returns>
    public static List<int> ReservoirSampling(int num, int k)
    {
        k = Mathf.Clamp(k, 0, num);
        List<int> sample = new List<int>();
        for (int i = 0; i < k; i++)
            sample.Add(i);
        for (int i = k; i < num; i++)
        {
            var t = Random.Range(0, (i + 1));
            if (t < k)
                sample[t] = i;
        }
        return sample;
    }

    /// <summary>
    /// ��Ȩ��ˮ��
    /// </summary>
    /// <param name="weights">Ȩ��</param>
    /// <param name="k">ѡȡ��</param>
    /// <returns>��������</returns>
    public static List<int> WeightReservoirSampling(List<float> weights, int k)
    {
        if (weights == null)
            return null;
        k = Mathf.Clamp(k, 0, weights.Count);
        float wsum = 0;
        List<int> sample = new List<int>();

        List<(float, int)> sortList = new List<(float, int)>();
        for (int i = 0; i < weights.Count; i++)
            sortList.Add((weights[i], i));
        sortList.Sort((a, b) => b.Item1.CompareTo(a.Item2));

        for (int i = 0; i < k; i++)
        {
            sample.Add(sortList[i].Item2);
            wsum += sortList[i].Item1 / k;
        }
        for (int i = k; i < weights.Count; i++)
        {
            wsum += sortList[i].Item1 / k;
            var p = sortList[i].Item1 / wsum;
            if (Random.value <= p)
                sample[Random.Range(0, k)] = sortList[i].Item2;
        }
        return sample;
    }

    /// <summary>
    /// ���ֲ���
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int SearchFun(int[] array, int value)
    {
        if (array == null)
            return -1; 
        int mid = -1, low = 0, high = array.Length - 1;
        while (low < high)
        {
            mid = (low + high) / 2;
            if (array[mid] == value)
                return mid;
            if (array[mid] > value) 
                high = mid - 1;
            else
                low = mid + 1;  
        }
        return mid;
    }

    /// <summary>
    /// ���ֲ���
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int SearchFun(List<int> array, int value)
    {
        if (array == null)
            return -1;
        int mid = -1, low = 0, high = array.Count - 1;
        while (low < high)
        {
            mid = (low + high) >> 1 ;
            if (array[mid] == value)
                return mid;
            if (array[mid] > value)
                high = mid ;
            else if (array[low + 1] > value)
                low = mid ;
            else
                low = mid - 1;
        }
        return mid;
    }

}
