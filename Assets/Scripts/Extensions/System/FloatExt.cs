using UnityEngine;

namespace Extensions.System
{
    public static class FloatExt
    {
        public static int CeilToInt(this float thisFloat)
        {
            return Mathf.CeilToInt(thisFloat);
        }
        public static int FloorToInt(this float thisFloat)
        {
            return Mathf.FloorToInt(thisFloat);
        }
        public static int RoundToInt(this float thisFloat)
        {
            return Mathf.RoundToInt(thisFloat);
        }

        public static float Sign(this float thisFloat)
        {
            return Mathf.Sign(thisFloat);
        }
        
        public static float Abs(this float thisFloat)
        {
            return Mathf.Abs(thisFloat);
        }
    }
}