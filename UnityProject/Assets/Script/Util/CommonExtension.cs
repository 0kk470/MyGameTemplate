using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Saltyfish.Util
{
    public static class CommonExtension
    {
        #region Collections Extension

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }


        public static bool RemoveBy<T>(this List<T> listToRemove, Predicate<T> predicate)
        {
            var idx = listToRemove.FindIndex(predicate);
            if (idx != -1)
            {
                listToRemove.RemoveAt(idx);
                return true;
            }
            return false;
        }

        public static void FastRemove<T>(this List<T> list, T v)
        {
            int i = list.IndexOf(v);
            if (i == -1)
                return;
            if (i != list.Count - 1)
            {
                list[i] = list[list.Count - 1];
            }
            list.RemoveAt(list.Count - 1);
        }

        public static void FastRemoveAt<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            if (index != list.Count - 1)
            {
                list[index] = list[list.Count - 1];
            }
            list.RemoveAt(list.Count - 1);
        }

        public static T PickRandomElement<T>(this ICollection<T> elementSet, bool removePicked = true)
        {
            T res = default(T);
            if (elementSet == null || elementSet.Count == 0)
            {
                Debug.LogError("Null or Empty Collections");
                return res;
            }
            var randomIdx = UnityExtension.InclusiveRange(0, elementSet.Count - 1);
            res = elementSet.ElementAt(randomIdx);
            if (removePicked)
            {
                elementSet.Remove(res);
            }
            return res;
        }

        /// <summary>
        /// fisher yates algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                var randomIdx = UnityExtension.InclusiveRange(0, i);
                if (randomIdx != i)
                {
                    T tmp = list[i];
                    list[i] = list[randomIdx];
                    list[randomIdx] = tmp;
                }
            }
        }

        public static void PrintCollections<T>(this IEnumerable<T> collects)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            int cnt = 0;
            foreach (var ele in collects)
            {
                if (cnt == 0)
                    sb.Append(ele.ToString());
                else
                    sb.AppendFormat(", {0}", ele.ToString());
                cnt++;
            }
            sb.Append(" ]");
            sb.AppendFormat(" Size: {0}", cnt);
            Debug.Log(sb);
        }

        public static void PrintCollections<T>(this IEnumerable<T> collects, Func<T, string> OnPrintString)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            int cnt = 0;
            foreach (var ele in collects)
            {
                var str = OnPrintString(ele);
                if (cnt == 0)
                    sb.Append(str);
                else
                    sb.AppendFormat(", {0}", str);
                cnt++;
            }
            sb.Append(" ]");
            sb.AppendFormat(" Size: {0}", cnt);
            Debug.Log(sb);
        }

        public static List<T> ToList<T>(this T[,] array)
        {
            if(array == null)
                return null;
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            var result = new List<T>(width * height);

            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    result.Add(array[w,h]);
                }
            }
            return result;
        }
        #endregion

        public static float ToPercent(this float val)
        {
            return val * 0.01f;
        }
    }
}
