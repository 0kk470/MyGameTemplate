using cfg.Saltyfish;
using UnityEngine;

namespace Saltyfish.Util
{
    public static class CompareUtil
    {
        public static bool CompareHelper(int val, int toCompare, NumberCompareSymbol CompareSymbol)
        {
            switch (CompareSymbol)
            {
                case NumberCompareSymbol.Equal:
                    return val == toCompare;
                case NumberCompareSymbol.NotEqual:
                    return val != toCompare;
                case NumberCompareSymbol.Greater:
                    return val > toCompare;
                case NumberCompareSymbol.GreaterEqual:
                    return val >= toCompare;
                case NumberCompareSymbol.Less:
                    return val < toCompare;
                case NumberCompareSymbol.LessEqual:
                    return val <= toCompare;
                default:
                    break;
            }
            return true;
        }

        public static bool CompareHelper(float val, float toCompare, NumberCompareSymbol CompareSymbol)
        {
            switch (CompareSymbol)
            {
                case NumberCompareSymbol.Equal:
                    return Mathf.Approximately(val, toCompare);
                case NumberCompareSymbol.NotEqual:
                    return !Mathf.Approximately(val, toCompare);
                case NumberCompareSymbol.Greater:
                    return val > toCompare;
                case NumberCompareSymbol.GreaterEqual:
                    return val > toCompare || Mathf.Approximately(val, toCompare);
                case NumberCompareSymbol.Less:
                    return val < toCompare;
                case NumberCompareSymbol.LessEqual:
                    return val < toCompare || Mathf.Approximately(val, toCompare);
                default:
                    break;
            }
            return true;
        }

    }
}
