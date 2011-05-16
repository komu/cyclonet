using System;
namespace CycloNet.Physics.Demos
{
    public class TimingData
    {
        public static DateTime LastFrameTimestamp;

        public static void Update()
        {
            LastFrameTimestamp = DateTime.Now;
        }
    }
}

