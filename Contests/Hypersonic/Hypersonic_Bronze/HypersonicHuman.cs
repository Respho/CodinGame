using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypersonicWindow
{
    public class HypersonicHuman
    {
        public int Team;
        public string Debug;

        public const int W = 13, H = 11;
        char[,] Board = new char[W, H];
        List<Entity> Entities;

        Point Direction; bool Bomb;

        public enum EntityType { Player, Bomb }

        public class Entity
        {
            public EntityType Type;
            public System.Drawing.Point Position;
            public int Team;
            public int Rounds;
            public int Range;
        }

        public HypersonicHuman(int team)
        {
            Team = team;
        }

        public void SetMove(Point direction, bool bomb)
        {
            Direction = direction; Bomb = bomb;
        }

        public string GetMove(string serialization)
        {
            //
            Debug = "";
            string[] lines = serialization.Split('\n');
            //
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Board[i, j] = lines[j][i];
                }
            }
            //
            Entities = new List<Entity>();
            int entitiesCount = int.Parse(lines[11]);
            for (int i = 0; i < entitiesCount; i++)
            {
                string[] tokens = lines[i + 12].Split(' ');
                Entities.Add(new Entity()
                {
                    Type = (tokens[0] == "0" ? EntityType.Player : EntityType.Bomb),
                    Team = int.Parse(tokens[1]),
                    Position = new Point(int.Parse(tokens[2]), int.Parse(tokens[3])),
                    Rounds = int.Parse(tokens[4]),
                    Range = int.Parse(tokens[5])
                });
            }

            debug("test");

            //
            Point current = Entities.Single(e => e.Team == Team && e.Type == EntityType.Player).Position;
            Point newPos = new Point(current.X + Direction.X, current.Y + Direction.Y);
            string action = Bomb ? "BOMB" : "MOVE";
            string move = action + " " + newPos.X.ToString() + " " + newPos.Y.ToString();

            //
            return move;
        }

        void debug(string message)
        {
            Debug += message + "\n";
            //
            //Console.WriteLine(message);
        }
    }
}
