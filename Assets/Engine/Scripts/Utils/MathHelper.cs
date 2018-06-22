namespace MB_Engine
{
    public static class MathHelper
    {
        public static bool In(int from, int to, int value)
        {
            return value >= from && value < to;
        }
    }
}