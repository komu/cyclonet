using System;
using OpenTK;

namespace CycloNet.Physics
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random, float min, float max)
        {
            return (float) random.NextDouble(min, max);
        }

        public static double NextDouble(this Random random, float min, float max)
        {
            return min + random.NextDouble() * (max - min);
        }

        public static Vector3 RandomVector3(this Random random, Vector3 min, Vector3 max)
        {
            return new Vector3(random.NextFloat(min.X, max.X),
                               random.NextFloat(min.Y, max.Y),
                               random.NextFloat(min.Z, max.Z));
        }
    }
}

