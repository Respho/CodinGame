using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    //
    const int RandomIterations = 100;
    const int PastFOW = 5;
    //
    const int W = 16001, H = 9001, BaseDistance = 1600;
    public int BustersPerPlayer, GhostCount, TeamID;
    public Point Home = new Point(0, 0);
    public int Round = 0;
    public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
    public List<string> BusterIDs = new List<string>();
    public Player(int bpp, int count, int id)
    {
        BustersPerPlayer = bpp; GhostCount = count; TeamID = id;
        if (TeamID == 1) Home = new Point(W - 1, H - 1);
        for (int i = 0; i < BustersPerPlayer; i++)
        {
            BusterIDs.Add("" + TeamID + "," + (TeamID * BustersPerPlayer + i));
        }
    }

    int distance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1; int dy = y2 - y1;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        return Convert.ToInt32(Math.Round(distance));
    }

    int distanceFromHome(int x, int y)
    {
        return distance(x, y, Home.X, Home.Y);
    }

    public struct EntityState
    {
        public int X, Y, TeamID, State, Value;
    }

    public class Entity
    {
        public int Type, ID;
        public Entity(int type, int id)
        {
            Type = type;
            ID = id;
        }
        //Ghost
        public bool Captured = false;
        //
        public Dictionary<int, EntityState> Positions = new Dictionary<int, EntityState>();
        public void Update(int round, int x, int y, int st, int v)
        {
            Positions[round] = new EntityState() { X = x, Y = y, State = st, Value = v };
        }
    }

    public void Update(int et, int eid, int x, int y, int st, int v)
    {
        string key = "" + et + "," + eid;
        if (!Entities.ContainsKey(key))
        {
            Console.Error.WriteLine("Creating key " + key);
            Entities[key] = new Entity(et, eid);
        }
        Entities[key].Update(Round, x, y, st, v);
    }

    private Random random = new Random();
    private Point getRandom()
    {
        bool[,] board = new bool[160, 90];
        int distSquaredScaled = 22 * 22;
        int past = Math.Min(PastFOW, Round + 1);
        for (int x = 0; x < 160; x++)
        {
            for (int y = 0; y < 90; y++)
            {
                bool covered = false;
                for (int i = 0; i < past; i++)
                {
                    foreach (string busterID in BusterIDs)
                    {
                        EntityState buster = Entities[busterID].Positions[Round - i];
                        int dx = buster.X / 100 - x; int dy = buster.Y / 100 - y;
                        int distanceSq = dx * dx + dy * dy;
                        if (distanceSq < distSquaredScaled)
                        {
                            board[x, y] = true;
                            covered = true;
                            break;
                        }
                    }
                    if (covered) break;
                }
            }
        }
        //
        /*
        for (int y = 0; y < 90; y += 2)
        {
            for (int x = 0; x < 160; x += 2)
            {
                Console.Error.Write(board[x, y] ? "." : "X");
            }
            Console.Error.WriteLine();
        }
        */
        //
        Point best = new Point();
        int bestCoverage = 0;
        for (int i = 0; i < RandomIterations; i++)
        {
            int randomX = random.Next(W); int randomY = random.Next(H);
            int coverage = 0;
            for (int x = 0; x < 160; x++)
            {
                for (int y = 0; y < 90; y++)
                {
                    if (!board[x, y])
                    {
                        int dx = randomX / 100 - x; int dy = randomY / 100 - y;
                        int distanceSq = dx * dx + dy * dy;
                        if (distanceSq < distSquaredScaled) coverage++;
                    }
                }
            }
            //
            if (coverage > bestCoverage)
            {
                best = new Point(randomX, randomY);
                bestCoverage = coverage;
            }
        }
        //
        return best;
    }

    public List<string> GetMoves()
    {
        // MOVE x y | BUST id | RELEASE
        List<string> moves = new List<string>();
        foreach (string busterID in BusterIDs)
        {
            //
            EntityState buster = Entities[busterID].Positions[Round];
            bool carrying = (buster.State == 1);
            if (carrying)
            {
                int distance = distanceFromHome(buster.X, buster.Y);
                if (distance < BaseDistance)
                {
                    moves.Add("RELEASE");
                    //
                    int gid = buster.Value;
                    string key = "-1" + "," + gid;
                    Entities[key].Captured = true;
                }
                else
                {
                    moves.Add("MOVE " + Home.X + " " + Home.Y + " Returning");
                }
                continue;
            }

            //
            List<Entity> visibleGhosts = Entities.Where(d => !d.Value.Captured && d.Value.Type == -1 && d.Value.Positions.ContainsKey(Round)).Select(d => d.Value).ToList();
            bool inRange = false;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d > 900 && d < 1760)
                {
                    inRange = true;
                    int ghostID = g.ID;
                    moves.Add("BUST " + ghostID);
                    break;
                }
            }
            if (inRange) continue;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d > 1760)
                {
                    inRange = true;
                    int ghostID = g.ID;
                    moves.Add("MOVE " + g.Positions[Round].X + " " + g.Positions[Round].Y + " Chase");
                    break;
                }
            }
            if (inRange) continue;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d < 900)
                {
                    inRange = true;
                    int ghostID = g.ID;
                    moves.Add("MOVE " + buster.X + " " + buster.Y + " Wait");
                    break;
                }
            }
            if (inRange) continue;

            //
            Point random = getRandom();
            moves.Add("MOVE " + random.X + " " + random.Y + " Random");
        }
        //
        Round++;
        return moves;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int bustersPerPlayer = int.Parse(Console.ReadLine()); // the amount of busters you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right

        Player player = new Player(bustersPerPlayer, ghostCount, myTeamId);

        // game loop
        while (true)
        {
            int entities = int.Parse(Console.ReadLine()); // the number of busters and ghosts visible to you
            for (int i = 0; i < entities; i++)
            {
                string line = Console.ReadLine();
                string[] inputs = line.Split(' ');
                int entityId = int.Parse(inputs[0]); // buster id or ghost id
                int x = int.Parse(inputs[1]);
                int y = int.Parse(inputs[2]); // position of this buster / ghost
                int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
                int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
                int value = int.Parse(inputs[5]); // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.
                //
                //Console.Error.WriteLine(line);
                player.Update(entityType, entityId, x, y, state, value);
            }

            //
            List<string> moves = player.GetMoves();
            foreach (string move in moves)
            {
                Console.WriteLine(move);
            }
        }
    }
}