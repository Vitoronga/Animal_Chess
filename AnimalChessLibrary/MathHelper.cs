using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    internal static class MathHelper
    {
        public static (int, int) AngleToCosSinTuple(int angle)
        {
            float radian = DegreeToRadian(angle);
            return ((int)MathF.Cos(radian), (int)MathF.Sin(radian));
        }

        public static float DegreeToRadian(int angle)
        {
            return angle * MathF.PI / 180;
        }
    }
}
