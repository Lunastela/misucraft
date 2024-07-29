namespace Misucraft.Util
{
    public static class MathHelper
    {
        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        internal static double DegreesToRadians(double degrees)
        {
            return MathF.PI / 180f * degrees;
        }
    }
}