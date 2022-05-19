using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saltyfish.Util.Random
{
    using SysRandom = System.Random;
    using UnityRandom = UnityEngine.Random;

    public enum KBRandomType
    {

        Start,    //遍历用 占位开始

        Default,  //默认使用

        Unit,     //刷怪位置的种子
        Map,      //当场游戏地图生成的种子

        End,      //遍历用 占位结束
    }

    public interface IWeightElement
    {
       int GetWeight();
    }

    public static class GameRand
    {
        public const int iMinSeed = 0;

        public const int iMaxSeed = int.MaxValue;

        public static int CurSeed { get; set; }

        public static int NewSeed
        {
            get
            {
                SysRandom random = new SysRandom((int)DateTime.Now.Ticks);
                return random.Next(iMinSeed, iMaxSeed);
            }
        }

        private static Dictionary<KBRandomType, SysRandom> m_RandomGenerators = new Dictionary<KBRandomType, SysRandom>();

        private static SysRandom m_DummyRand;

        public static SysRandom DummyRand
        {
            get
            {
                if(m_DummyRand == null)
                    m_DummyRand = new SysRandom((int)DateTime.Now.Ticks);
                return m_DummyRand;
            }
        }

        public static SysRandom NewRandomGenerator(KBRandomType rType, int seed)
        {
            if(rType == KBRandomType.Default)
                seed = (int)DateTime.Now.Ticks;
            SysRandom rand = new SysRandom(seed);
            if (m_RandomGenerators.ContainsKey(rType))
                m_RandomGenerators[rType] = rand;
            else
                m_RandomGenerators.Add(rType, rand);
            return rand;
        }

        public static SysRandom GetRandomGenerator(KBRandomType rType)
        {
            SysRandom result = null;
            if (rType > KBRandomType.Start && rType < KBRandomType.End)
            {
                m_RandomGenerators.TryGetValue(rType, out result);
            }
            else
            {
                Debug.LogErrorFormat("Invalid KBRandomType:{0}, Use Dummy Random instead." , rType.ToString());
            }
            if(result == null) result = DummyRand;
            return result;
        }
        
        public static void Init(int seed)
        {
            CurSeed = seed;
            m_RandomGenerators.Clear();
            for (var rType = KBRandomType.Start + 1; rType < KBRandomType.End; ++rType)
            {
                NewRandomGenerator(rType, seed);
            }
        }

        public static int Range(int min, int max, KBRandomType rType = KBRandomType.Default)
        {
            var rand = GetRandomGenerator(rType);
            int result =  rand.Next(min, max);
            return result;
        }

        public static int InclusiveRange(int min, int max, KBRandomType rType = KBRandomType.Default)
        {
            return Range(min, max + 1, rType);
        }

        public static float Range(float min, float max, KBRandomType rType = KBRandomType.Default)
        {
            var rand = GetRandomGenerator(rType);
            float result = (float)rand.NextDouble() * (max - min) + min;
            return result;
        }

        public static float UnityRange(float min, float max)
        {
            return UnityRandom.Range(min, max);
        }

        public static int PickWeightIndex(IList<uint> weights, KBRandomType rType = KBRandomType.Default, bool removePicked = false)
        {
            int index = -1;
            if (weights == null || weights.Count == 0)
                return index;
            int sum = 0;
            for (int i = 0; i < weights.Count; ++i) sum += (int)weights[i];
            var val = InclusiveRange(0, sum, rType);
            uint currentWeight = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                currentWeight += weights[i];
                if (val <= currentWeight)
                {
                    index = i;
                    break;
                }
            }
            //if(removePicked && index >= 0 && index < weights.Count)
            //{

            //}
            return index;
        }

        public static T PickWeightElement<T>(IList<T> weights, KBRandomType rType = KBRandomType.Default, bool removePicked = false) where T:IWeightElement
        {
            T res = default(T);
            if (weights == null || weights.Count == 0)
                return res;
            int sum = 0;
            for (int i = 0; i < weights.Count; ++i) sum += weights[i].GetWeight();
            var val = InclusiveRange(0, sum, rType);
            int currentWeight = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                currentWeight += weights[i].GetWeight();
                if (val <= currentWeight)
                {
                    res = weights[i];
                    break;
                }
            }
            if (removePicked && res != null)
                weights.Remove(res);
            return res;
        }

        public static bool Probability_100(float chance)
        {
            if (chance >= 100)
                return true;
            if (chance <= 0)
                return false;
            return chance >= InclusiveRange(0, 100);
        }

        private static int SeedClamp(int seed)
        {
            while(iMaxSeed > seed) seed = seed >> 1;

            return Mathf.Clamp(seed, iMinSeed, iMaxSeed);
        }
    }
}
