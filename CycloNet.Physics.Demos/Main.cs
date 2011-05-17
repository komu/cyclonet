using System;

namespace CycloNet.Physics.Demos
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var win = new Fireworks.FireworksDemo())
            //using (var win = new Ballistic.BallisticDemo())
            {
                win.Run();
            }
        }
    }
}

