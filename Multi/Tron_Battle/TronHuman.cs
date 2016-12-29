using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronWindow
{
    public class TronHuman
    {
        public int Team;
        public string Debug;
        Point Direction;
        public TronHuman(int team)
        {
            Team = team;
        }
        public void SetMove(Point direction)
        {
            Direction = direction;
        }

        public string GetMove(string serialization)
        {
            //
            Debug = "";
            debug("test");

            //
            if (Direction.X > 0) return "RIGHT";
            if (Direction.X < 0) return "LEFT";
            if (Direction.Y > 0) return "DOWN";
            if (Direction.Y < 0) return "UP";
            //
            return "";
        }

        void debug(string message)
        {
            Debug += message + "\n";
            //
            //Console.WriteLine(message);
        }
    }
}
