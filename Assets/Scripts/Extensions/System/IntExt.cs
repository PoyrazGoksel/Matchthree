namespace Extensions.System
{
    public static class IntExt
    {
        public static bool ToBool(this int thisInt)
        {
            return thisInt == 1;
        }

        public static float ToFloat(this int thisInt)
        {
            return thisInt;
        }
    }
}