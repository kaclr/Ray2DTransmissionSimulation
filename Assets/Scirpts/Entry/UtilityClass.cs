using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class UtilityClass
{
    public static float ToAngle(this Vector3 v)
    {
        float ag;
        if (v.magnitude > Mathf.Epsilon)
        {
            if (Math.Abs(v.x) < Mathf.Epsilon)
            {
                return 90 * Mathf.Sign(v.y);
            }

            ag = Mathf.Atan(v.y / v.x) / Mathf.PI * 180;
            if (v.x < 0)
            {
                ag = 180 + ag;
            }
        }
        else
        {
            ag = Random.Range(0f, 360f);
        }

        return ag;
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector3 ToVector3(this float ag)
    {
        return (Vector3.right * Mathf.Cos(ag * Mathf.Deg2Rad) + Vector3.up * Mathf.Sin(ag * Mathf.Deg2Rad));
    }

    //字符串转V3
    public static Vector3 ToVector3(this string str)
    {
        string[] strs = str.Split(' ');
        return new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]));
    }

    public static string[] ToArray(this string str)
    {
        string[] arr = { str };
        return arr;
    }

    //字符串转整型列表
    public static List<int> StringToInts(string str)
    {
        List<int> output = new List<int>();

        if (string.IsNullOrEmpty(str))
        {
            return output;
        }
        try
        {
            string[] div = str.Split('|');
            for (int i = 0; i < div.Length; i++)
            {
                if (string.IsNullOrEmpty(div[i]))
                {
                    continue;
                }

                string[] range = div[i].Split('=');

                if (range.Length == 1)
                {
                    output.Add(int.Parse(range[0]));
                }
                else if (range.Length == 2)
                {
                    int start = int.Parse(range[0]);
                    int end = int.Parse(range[1]);

                    for (int j = start; j <= end; j++)
                    {
                        output.Add(j);
                    }
                }
                else
                {
                    throw new ArgumentException(str);
                }
            }
        }
        catch (FormatException e)
        {
            Debug.LogError("FormatException ->" + str);
        }
        return output;
    }

    //整型列表转字符串
    public static string IntsToString(List<int> ints)
    {
        if (ints.Count == 0)
        {
            return "";
        }

        List<int> stack = new List<int>();

        string output = "";
        for (int i = 0; i < ints.Count; i++)
        {
            //出栈，2个则分别写，三个以上用连号
            if (stack.Count > 0 && ints[i] - 1 != stack[stack.Count - 1])
            {
                if (output != "")
                {
                    output += "|";
                }

                if (stack.Count == 1)
                {
                    output += stack[0].ToString();
                }
                else if (stack.Count == 2)
                {
                    output += stack[0] + "|" + stack[1];
                }
                else
                {
                    output += stack[0] + "=" + stack[stack.Count - 1];
                }
                stack.Clear();
            }
            stack.Add(ints[i]);
        }

        //最后再出栈一次
        if (stack.Count > 0)
        {
            if (output != "")
            {
                output += "|";
            }

            if (stack.Count == 1)
            {
                output += stack[0].ToString();
            }
            else if (stack.Count == 2)
            {
                output += stack[0] + "|" + stack[1];
            }
            else
            {
                output += stack[0] + "=" + stack[stack.Count - 1];
            }
            stack.Clear();
        }
        return output;
    }

    //洗牌算法
    public static void Shuffle<T>(ref List<T> list)
    {
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int rand = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    //随机拿一个
    public static T RandomGet<T>(List<T> list, List<float> each = null)
    {
        if (each != null && list.Count != each.Count)
        {
            throw new ArgumentException(list.Count.ToString());
        }
        if (each == null)
        {
            int rand = Random.Range(0, list.Count);
            return list[rand];
        }
        else
        {
            float sum = 0f;
            for (int i = 0; i < each.Count; i++)
            {
                sum += each[i];
            }
            float rand = Random.Range(0f, sum);
            for (int i = 0; i < each.Count; i++)
            {
                rand -= each[i];
                if (rand <= 0f)
                {
                    return list[i];
                }
            }
        }
        return default(T);
    }

    //几率抽取，第二个参数是抽取0~n个的概率，第三个参数是对应列表中的每一个的概率
    public static List<T> RandomGetList<T>(List<T> list, List<float> countProbability, List<float> each = null)
    {
        if (each != null && list.Count != each.Count)
        {
            throw new ArgumentException();
        }
        //缺少概率则补0
        while (list.Count > countProbability.Count - 1)
        {
            countProbability.Add(0f);
        }
        //这里使用索引而不是复制list
        List<int> indexs = new List<int>();
        List<float> each2 = new List<float>();
        List<int> counts = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            indexs.Add(i);
            if (each != null)
            {
                each2.Add(each[i]);
            }

            counts.Add(i);
        }
        counts.Add(counts.Count);

        //抽取需要随机抽的次数
        int count = RandomGet(counts, countProbability);
        List<T> output = new List<T>();

        //平均抽
        if (each == null)
        {
            for (int i = 0; i < count; i++)
            {
                int index = RandomGet<int>(indexs);
                output.Add(list[index]);
                indexs.Remove(index);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                int index = RandomGet<int>(indexs, each2);
                int at = indexs.IndexOf(index);
                output.Add(list[index]);
                indexs.RemoveAt(at);
                each2.RemoveAt(at);
            }
        }
        return output;
    }
}