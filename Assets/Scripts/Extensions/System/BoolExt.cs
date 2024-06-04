namespace Extensions.System
{
    public static class BoolExt
    {
        public static int ToInt(this bool thisBool)
        {
            return thisBool ? 1 : 0;
        }
    }
}