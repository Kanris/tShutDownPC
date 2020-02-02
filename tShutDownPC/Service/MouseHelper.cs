using System;
using System.Drawing;

namespace tShutDownPC.Service
{
    public static class MouseHelper
    {
        //public static int Time { get; set; } = 60;
        //private static int Counter { get; set; } = 0;

        private static Point PrevPoint { get; set; } = System.Windows.Forms.Cursor.Position;
        private static Point CurrPoint { get; set; } = System.Windows.Forms.Cursor.Position;


        public static bool ComparePoints(ref int counter, int time)
        {
            CurrPoint = System.Windows.Forms.Cursor.Position;

            Console.WriteLine($"[{PrevPoint.X},{PrevPoint.Y}] ==> [{CurrPoint.X},{CurrPoint.Y}] ({counter})");

            if (PrevPoint != CurrPoint)
            {
                PrevPoint = CurrPoint;
                counter = 0;
                return false;
            }

            counter++;

            if (counter >= time)
            {
                counter = 0;
                return true;
            }

            return false;

        }
    }
}
