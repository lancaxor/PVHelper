using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PVHelper
{
    class Point
    {
        private static int id = 0;
        private int PointId;
        private int x, y;
        public String Coords { get { return (this.x.ToString() + ";" + this.y.ToString()); } set { SetCoords(value); } }
        public int ID { get { return this.PointId; } }
        public static bool ZeroID { set { if (value) id = 0; } }

        public Point()
        {
            this.PointId=id;
            id++;
        }

        public double GetDistance(int x, int y)
        {
            return Math.Sqrt((this.x - x) * (this.x - x) + (this.y - y) * (this.y - y));
        }

        public bool SetCoords(String coords)
        {
            int tmpX = 0, tmpY = 0;
            if (!(Int32.TryParse(coords.Substring(0, coords.IndexOf(';')), out tmpX) &&
                Int32.TryParse(coords.Substring(coords.IndexOf(';') + 1), out tmpY)))
            {
                this.x = 0;
                this.y = 0;
                return false;
            }
            this.x = tmpX;
            this.y = tmpY;
            return true;
        }
    }
}
