using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    #region Opening
    //2b
    //0 1176 2024 0 0 -1
    //1 2024 1176 0 0 -1
    //4b
    //0 1176 2024 0 0 -1
    //1 2024 1176 0 0 -1
    //2 327 2873 0 0 -1
    //3 2873 327 0 0 -1
    const int OpeningMoves = 16;
    List<Point> SequenceNorth = new List<Point>() {
        new Point(2600, 0),
        //15 steps
        new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), 
        new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 0), new Point(10000, 800), new Point(10000, 1600),
        new Point(10000, 2400)
    };
    List<Point> SequenceWest = new List<Point>() {
        new Point(0, 3000),
        //15 steps
        new Point(0, 9000), new Point(0, 9000), new Point(0, 9000), new Point(0, 9000), new Point(0, 9000), new Point(0, 9000), new Point(0, 9000),
        new Point(5600, 9000), new Point(5600, 9000), new Point(5600, 9000), new Point(5600, 9000), new Point(5600, 9000), new Point(5600, 9000), new Point(5600, 9000),
        new Point(5600, 8000)
    };
    List<Point> SequenceSouth = new List<Point>();
    List<Point> SequenceEast = new List<Point>();

    //
    List<string> opening()
    {
        if (Round >= OpeningMoves) return null;
        //
        if (SequenceSouth.Count() == 0)
        {
            for (int i = 0; i < SequenceNorth.Count(); i++)
            {
                Point pointSouth = new Point(W - SequenceNorth[i].X - 1, H - SequenceNorth[i].Y - 1);
                SequenceSouth.Add(pointSouth);
                Point pointEast = new Point(W - SequenceWest[i].X - 1, H - SequenceWest[i].Y - 1);
                SequenceEast.Add(pointEast);
            }
        }
        //
        List<string> moves = new List<string>();
        for (int i = 0; i < BustersPerPlayer; i++)
        {
            //2b - west north
            //4b - west north west north
            Point target = new Point();
            if (TeamID == 0)
            {
                if (i % 2 == 0)
                {
                    target = SequenceWest[Round];
                }
                else
                {
                    target = SequenceNorth[Round];
                }
            }
            else
            {
                if (i % 2 == 0)
                {
                    target = SequenceEast[Round];
                }
                else
                {
                    target = SequenceSouth[Round];
                }
            }
            moves.Add("MOVE " + target.X + " " + target.Y + " Opening");
        }

        //
        return moves;
    }
    #endregion

    #region Stun
    void updateStun()
    {
        //
        foreach (string busterID in BusterIDs)
        {
            Entity buster = Entities[busterID];
            EntityState es = buster.Positions[Round];
            int previousRound = Round - 1;
            if (es.State == StateStunned && es.Value == LimBustStunDuration)
            {
                es = buster.Positions[previousRound];
                int enemyTeamID = 1 - TeamID;
                List<Entity> staticEnemyBusters = Entities.Where(e => e.Value.Type == enemyTeamID)
                    .Where(e => e.Value.Positions.ContainsKey(Round))
                    .Where(e => e.Value.Positions.ContainsKey(previousRound))
                    .Where(e => e.Value.Positions[Round].X == e.Value.Positions[previousRound].X)
                    .Select(e => e.Value).ToList();
                if (staticEnemyBusters.Count() == 1)
                {
                    string bid = "" + staticEnemyBusters[0].Type + "," + staticEnemyBusters[0].ID;
                    addStun(bid, busterID);
                }
                else
                {
                    Console.Error.WriteLine("Cannot attribute stunned " + busterID + " to buster");
                }
                /*
                List<Entity> nearbyBusters = Entities.Where(e => e.Value.Type == enemyTeamID)
                    .Where(e => e.Value.Positions.ContainsKey(previousRound))
                    .Where(e => distance(es.X, es.Y, e.Value.Positions[previousRound].X, e.Value.Positions[previousRound].Y) < LimStunMax)
                    .Select(e => e.Value).ToList();
                List<Entity> capableBusters = nearbyBusters.Where(b => stunTimer("" + b.Type + "," + b.ID)).ToList();
                if (capableBusters.Count() >= 1)
                {
                    string bid = "" + capableBusters[0].Type + "," + capableBusters[0].ID;
                    addStun(bid, busterID);
                }
                foreach (Entity b in nearbyBusters)
                {
                    Console.Error.WriteLine("" + b.ID + " distance " + distance(es.X, es.Y, b.Positions[previousRound].X, b.Positions[previousRound].X));
                }
                */
            }
        }
    }
    void addStun(string busterID, string stunnedBusterID)
    {
        //
        StunEvent stunEvent = new StunEvent() { Round = Round - 1, BusterID = busterID, StunnedBusterID = stunnedBusterID };
        StunEvents.Add(stunEvent);
        //
        Console.Error.WriteLine("Buster " + busterID + " has stunned " + stunnedBusterID);
    }
    bool isValidStunTimer(string busterID)
    {
        if (StunEvents.Exists(s => s.BusterID == busterID))
        {
            int latestRound = StunEvents.Where(s => s.BusterID == busterID).OrderByDescending(s => s.Round).First().Round;
            if ((Round - latestRound) < LimBustStunTimer) return false;
        }
        return true;
    }
    int canStun(string busterID)
    {
        //Can stun if buster has not used stun
        if (!isValidStunTimer(busterID)) return -1;
        //Can stun if not stunned and not carrying a ghost
        Entity buster = Entities[busterID];
        EntityState es = buster.Positions[Round];
        if (es.State > 0) return -1;
        //Can stun if target is in range, not yet stunned, carrying a ghost
        int enemyTeamID = 1 - TeamID;
        List<Entity> busters = Entities.Where(e => e.Value.Type == enemyTeamID)
            .Where(e => e.Value.Positions.ContainsKey(Round))
            .Where(e => distance(es.X, es.Y, e.Value.Positions[Round].X, e.Value.Positions[Round].Y) < LimStunMax)
            .Where(e => e.Value.Positions[Round].State < 2)
            .OrderByDescending(e => e.Value.Positions[Round].State) //Stun ghost carrying busters first
            .Select(e => e.Value).ToList();
        //
        if (busters.Count() > 0) return busters[0].ID;
        //
        return -1;
    }
    public struct StunEvent
    {
        public int Round;
        public string BusterID, StunnedBusterID;
    }
    #endregion

    //
    const int RandomIterations = 100;
    const int PastFOW = 13;
    const int GhostTypeID = -1, StateStunned = 2;
    const int LimVisibility = 2200, LimVisibilitySquaredScaled = 22 * 22;
    const int LimBaseRange = 1600;
    const int LimBustSpeed = 800, LimGhostSpeed = 400;
    const int LimBustMax = 1760, LimBustMin = 900, LimStunMax = 1760;
    const int LimBustStunDuration = 10, LimBustStunTimer = 20;
    //
    const int W = 16001, H = 9001;
    public int BustersPerPlayer, GhostCount, TeamID;
    public Point Home = new Point(0, 0);
    public int Round = 0;
    public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
    public List<StunEvent> StunEvents = new List<StunEvent>();
    public List<string> BusterIDs = new List<string>();
    public List<string> EnemyBusterIDs = new List<string>();
    public Player(int bpp, int count, int id)
    {
        BustersPerPlayer = bpp; GhostCount = count; TeamID = id;
        if (TeamID == 1) Home = new Point(W - 1, H - 1);
        for (int i = 0; i < BustersPerPlayer; i++)
        {
            BusterIDs.Add("" + TeamID + "," + (TeamID * BustersPerPlayer + i));
            EnemyBusterIDs.Add("" + (1 - TeamID) + "," + ((1 - TeamID) * BustersPerPlayer + i));
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

    public class Entity
    {
        //Team (0 or 1) or ghost -1
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

    public struct EntityState
    {
        public int X, Y, TeamID,
            //0 Idle
            //1 Carrying ghost ID
            //2 Stunned
            State,
            //Buster carrying ghost ID
            //Buster stunned number of moves left
            //Ghost always 0
            //Ghost number of busters trapping it
            Value;
    }

    public void Update(int et, int eid, int x, int y, int st, int v)
    {
        string key = "" + et + "," + eid;
        if (!Entities.ContainsKey(key))
        {
            //Console.Error.WriteLine("Creating key " + key);
            Entities[key] = new Entity(et, eid);
        }
        Entities[key].Update(Round, x, y, st, v);
    }

    #region Explore
    private Dictionary<string, Point> exploring = new Dictionary<string, Point>();
    private bool[,] exploringBoard = null;
    private Random random = new Random();

    private void updateExploringBoard()
    {
        int offset = 20;
        int width = 160, height = 90;
        exploringBoard = new bool[width + offset * 2, height + offset * 2];
        int pastSteps = Math.Min(PastFOW, Round + 1);
        //
        for (int x = 0; x < width + offset * 2; x++)
        {
            for (int y = 0; y < height + offset * 2; y++)
            {
                //Iterate for each buster for each step
                bool covered = false;
                for (int i = 0; i < pastSteps; i++)
                {
                    foreach (string busterID in BusterIDs)
                    {
                        EntityState buster = Entities[busterID].Positions[Round - i];
                        int dx = buster.X / 100 + offset - x; int dy = buster.Y / 100 + offset - y;
                        int distanceSq = dx * dx + dy * dy;
                        if (distanceSq < LimVisibilitySquaredScaled)
                        {
                            exploringBoard[x, y] = true;
                            covered = true;
                            break;
                        }
                    }
                    if (covered) break;
                }
            }
        }
        //Clear enemy base
        if (TeamID == 0)
        {
            for (int x = width; x < width + offset * 2; x++)
            {
                for (int y = height; y < height + offset * 2; y++)
                {
                    exploringBoard[x, y] = true;
                }
            }
        }
        else
        {
            for (int x = 0; x < offset * 2; x++)
            {
                for (int y = 0; y < offset * 2; y++)
                {
                    exploringBoard[x, y] = true;
                }
            }
        }
        //
        /*
        for (int x = 0; x < width + offset * 2; x++)
        {
            for (int y = 0; y < height + offset * 2; y++)
            {
                Console.Error.Write(exploringBoard[x, y] ? "." : "X");
            }
            Console.Error.WriteLine();
        }
        */
    }
    private Point getExplore(string busterID)
    {
        if (exploring.ContainsKey(busterID))
        {
            EntityState es = Entities[busterID].Positions[Round];
            Point target = exploring[busterID];
            if (distance(es.X, es.Y, target.X, target.Y) > 200)
            {
                //Target not reached
                return target;
            }
        }

        //Assign a new target
        Point p = getBest();
        exploring[busterID] = p;

        //Stamp the board
        int offset = 20;
        int width = 160, height = 90;
        for (int x = 0; x < width + offset * 2; x++)
        {
            for (int y = 0; y < height + offset * 2; y++)
            {
                int dx = p.X / 100 + offset - x; int dy = p.Y / 100 + offset - y;
                int distanceSq = dx * dx + dy * dy;
                if (distanceSq < LimVisibilitySquaredScaled) exploringBoard[x, y] = true;
            }
        }

        //
        return p;
    }
    private Point getBest()
    {
        //
        Point best = new Point();
        int bestCoverage = 0;
        int offset = 20;
        int width = 160, height = 90;
        for (int i = 0; i < RandomIterations; i++)
        {
            int randomX = random.Next(W); int randomY = random.Next(H);
            int coverage = 0;
            for (int x = 0; x < width + offset * 2; x++)
            {
                for (int y = 0; y < height + offset * 2; y++)
                {
                    if (!exploringBoard[x, y])
                    {
                        int dx = randomX / 100 + offset - x; int dy = randomY / 100 + offset - y;
                        int distanceSq = dx * dx + dy * dy;
                        if (distanceSq < LimVisibilitySquaredScaled) coverage++;
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
    #endregion

    public List<string> GetMoves()
    {
        //
        updateStun();
        updateExploringBoard();

        //
        /*
        List<string> openingMoves = opening();
        if (openingMoves != null)
        {
            Round++;
            return openingMoves;
        }
        */

        //
        List<Entity> visibleGhosts = Entities.Where(d => !d.Value.Captured && d.Value.Type == GhostTypeID && d.Value.Positions.ContainsKey(Round)).Select(d => d.Value).ToList();
        Dictionary<int, string> ghostChasedBy = new Dictionary<int, string>();
        Dictionary<int, string> busterStunnedBy = new Dictionary<int, string>();

        // MOVE x y | BUST id | RELEASE | STUN
        List<string> moves = new List<string>();
        foreach (string busterID in BusterIDs)
        {
            //
            EntityState buster = Entities[busterID].Positions[Round];
            bool carrying = (buster.State == 1);
            if (carrying)
            {
                int distance = distanceFromHome(buster.X, buster.Y);
                if (distance < LimBaseRange)
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
            int stunBusterID = canStun(busterID);
            if (stunBusterID >= 0)
            {
                if (!busterStunnedBy.ContainsKey(stunBusterID))
                {
                    busterStunnedBy[stunBusterID] = busterID;
                    addStun(busterID, "" + (1 - TeamID) + "," + stunBusterID);
                    moves.Add("STUN " + stunBusterID);
                    continue;
                }
            }

            //
            bool inRange = false;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d > LimBustMin && d < LimBustMax)
                {
                    ghostChasedBy[g.ID] = busterID;
                    inRange = true;
                    moves.Add("BUST " + g.ID);
                    break;
                }
            }
            if (inRange) continue;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d > LimBustMax)
                {
                    if (!ghostChasedBy.ContainsKey(g.ID))
                    {
                        ghostChasedBy[g.ID] = busterID;
                        inRange = true;
                        moves.Add("MOVE " + g.Positions[Round].X + " " + g.Positions[Round].Y + " Chase");
                        break;
                    }
                }
            }
            if (inRange) continue;
            foreach (Entity g in visibleGhosts)
            {
                int d = distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
                if (d < LimBustMin)
                {
                    inRange = true;
                    int ghostID = g.ID;
                    moves.Add("MOVE " + buster.X + " " + buster.Y + " Wait");
                    break;
                }
            }
            if (inRange) continue;

            //
            Point random = getExplore(busterID);
            moves.Add("MOVE " + random.X + " " + random.Y + " Explore");
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