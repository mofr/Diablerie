namespace Diablerie.Engine.IO.D2Formats
{
    public class DirectionMapping
    {
        private static readonly int[] _dirs1 = new int[] { 0 };
        private static readonly int[] _dirs4 = new int[] { 0, 1, 2, 3 };
        private static readonly int[] _dirs8 = new int[] { 4, 0, 5, 1, 6, 2, 7, 3 };
        private static readonly int[] _dirs16 = new int[] { 4, 8, 0, 9, 5, 10, 1, 11, 6, 12, 2, 13, 7, 14, 3, 15 };
        private static readonly int[] _dirs32 = new int[] { 4, 16, 8, 17, 0, 18, 9, 19, 5, 20, 10, 21, 1, 22, 11, 23, 6, 24, 12, 25, 2, 26, 13, 27, 7, 28, 14, 29, 3, 30, 15, 31 };


        public static int MapToInternal(int directionCount, int directionIndex)
        {
            switch (directionCount)
            {
                case 1: return _dirs1[directionIndex];
                case 4: return _dirs4[directionIndex];
                case 8: return _dirs8[directionIndex];
                case 16: return _dirs16[directionIndex];
                case 32: return _dirs32[directionIndex];
                default: return 0;
            }
        }
    }
}
