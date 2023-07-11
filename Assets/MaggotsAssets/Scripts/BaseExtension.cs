namespace Maggots
{
    public static class BaseExtension
    {
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static float ToFloat(this bool value)
        {
            return value ? 1f : 0f;
        }

        public static float RadianToAngle(this float value)
        {
            return value * 360f;
        }
    }
}

