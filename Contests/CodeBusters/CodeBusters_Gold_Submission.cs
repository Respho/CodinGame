//
// Submitted 24 hours before contest, ranked 383 in Gold and 476 overall
//
// Final contest submission
//

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
    const int PastFOW = 14;
    //
    const int W = 16001, H = 9001;
    const int LimVisibility = 2200, LimVisibilitySquaredScaled = 22 * 22;
    const int LimBaseRange = 1600;
    const int LimBusterSpeed = 800, LimGhostSpeed = 400;
    const int LimBustMax = 1760, LimBustMin = 900, LimStunMax = 1760;
    const int LimBusterStunDuration = 10, LimBusterStunTimer = 20;
    const int GhostTypeID = -1, StateCarrying = 1, StateStunned = 2;
    //
    public int BustersPerPlayer, GhostCount, TeamID;
    public Point Home = new Point(0, 0);
    public int Round = 0;
    public List<string> BusterIDs = new List<string>();
    public List<string> EnemyBusterIDs = new List<string>();
    public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
    public Dictionary<int, Dashboard> Dashboards = new Dictionary<int, Dashboard>();
    public List<StunEvent> StunEvents = new List<StunEvent>();

    #region Dispatch
    public enum DispatchActionType
    {
        Stunned,
        Stun,      //Stun an enemy
        Release,   //Release a ghost
        Return,    //Returning with a ghost
        Intercept, //Intercept enemy team for a ghost
        Herd,      //Herd a ghost(s) back to base
        Bust,      //Bust a ghost
        Contest,   //Contest a ghost
        Chase,     //Chase a ghost
        Find,      //Find a ghost from memory
        Roam
    }
    public class DispatchAction
    {
        public DispatchActionType Type;
        public Point Target;
        public int EntityID;
    }
    #endregion

    #region Dashboard
    public class Dashboard
    {
        //
        public List<string> StunnedBusterIDs = new List<string>();
        public List<string> StunnedEnemyBusterIDs = new List<string>();
        //0,0->19, 1,3->0
        public Dictionary<string, int> StunTimers = new Dictionary<string, int>();
        public List<int> DroppedGhostsIDs = new List<int>();
        public List<int> ContestedGhostsIDs = new List<int>();
        //
        public Dictionary<string, DispatchAction> DispatchActions = new Dictionary<string, DispatchAction>();
        //
        public List<string> Predictions = new List<string>();
        public List<string> Updates = new List<string>();
        public List<string> Strategies = new List<string>();
    }
    void updateContestedGhosts()
    {
        List<Entity> visibleGhosts = Entities
            .Where(d => !d.Value.Captured && d.Value.Type == GhostTypeID && d.Value.Positions.ContainsKey(Round))
            .Where(d => d.Value.Positions[Round].Value > 1)
            .Select(d => d.Value).ToList();
        foreach (Entity ghost in visibleGhosts)
        {
            Dashboards[Round].ContestedGhostsIDs.Add(ghost.ID);
        }
    }
    #endregion

    #region Stun
    void updateStun()
    {
        //Attribute stuns
        foreach (string busterID in BusterIDs)
        {
            Entity buster = Entities[busterID];
            EntityState es = buster.Positions[Round];
            int previousRound = Round - 1;
            if (es.State == StateStunned && es.Value == LimBusterStunDuration)
            {
                es = buster.Positions[previousRound];
                int enemyTeamID = 1 - TeamID;
                List<Entity> staticEnemyBusters = Entities.Where(e => e.Value.Type == enemyTeamID)
                    .Where(e => e.Value.Positions.ContainsKey(Round))
                    .Where(e => e.Value.Positions.ContainsKey(previousRound))
                    .Where(e => e.Value.Positions[Round].X == e.Value.Positions[previousRound].X)
                    .Where(e => e.Value.Positions[previousRound].State != StateStunned)
                    .Where(e => Distance(es.X, es.Y, e.Value.Positions[Round].X, e.Value.Positions[Round].Y) <= LimStunMax)
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
            }
        }

        //Update timers
        foreach (string busterID in BusterIDs.Union(EnemyBusterIDs).ToList())
        {
            //Opening
            if (!Entities.ContainsKey(busterID))
            {
                Dashboards[Round].StunTimers[busterID] = 0;
                continue;
            }

            //For a stunned buster, the countdown is the duration
            int stunnedCountdown = 0;
            if (Round > 0)
            {
                stunnedCountdown = Math.Max(0, Dashboards[Round - 1].StunTimers[busterID] - 1);
            }
            if (Entities[busterID].Positions.ContainsKey(Round))
            {
                EntityState es = Entities[busterID].Positions[Round];
                if (es.State == 2)
                {
                    stunnedCountdown = es.Value;
                }
            }

            //For a non stunned buster, the countdown is the previous stun event
            int stunTimer = 0;
            if (StunEvents.Exists(s => s.BusterID == busterID))
            {
                int latestRound = StunEvents.Where(s => s.BusterID == busterID).OrderByDescending(s => s.Round).First().Round;
                int difference = LimBusterStunTimer - (Round - latestRound);
                stunTimer = Math.Max(0, difference);
            }

            //
            Dashboards[Round].StunTimers[busterID] = Math.Max(stunTimer, stunnedCountdown);
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
            if ((Round - latestRound) < LimBusterStunTimer) return false;
        }
        Console.Error.WriteLine("StunEvents Dump");
        foreach (StunEvent e in StunEvents)
        {
            Console.Error.WriteLine("" + e.Round + " " + e.BusterID + " " + e.StunnedBusterID);
        }
        return true;
    }
    public struct StunEvent
    {
        public int Round;
        public string BusterID, StunnedBusterID;
    }
    #endregion

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

    public static int Distance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1; int dy = y2 - y1;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        return Convert.ToInt32(Math.Round(distance));
    }

    int distanceFromHome(int x, int y)
    {
        return Distance(x, y, Home.X, Home.Y);
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
            //Buster 0 Idle
            //Buster 1 Carrying ghost ID
            //Buster 2 Stunned
            //Buster 3 Busting
            //Ghost stamina
            State,
            //Buster carrying ghost ID
            //Buster stunned number of moves left
            //Ghost the number of busters trapping it
            Value;
    }

    public void Update(int et, int eid, int x, int y, int st, int v)
    {
        //
        if (!Dashboards.ContainsKey(Round)) Dashboards[Round] = new Dashboard();

        //
        string line = "";
        line += (et < 0 ? "Ghost " : et + ",") + (eid < 10 ? " " + eid : "" + eid);
        line += " (" + (x < 1000 ? " " + x : "" + x) + "," + (y < 1000 ? " " + y : "" + y) + ") ";
        line += et < 0 ? "Stamina " + (st < 10 ? " " + st : "" + st) : (st == 1 ? "Carrying " : (st == 3 ? "Busting " : (st == 2 ? "Stunned " : "")) + v);
        Dashboards[Round].Updates.Add(line);

        //
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
        //Clear bases
        for (int x = width; x < width + offset * 2; x++)
        {
            for (int y = height; y < height + offset * 2; y++)
            {
                exploringBoard[x, y] = true;
            }
        }
        for (int x = 0; x < offset * 2; x++)
        {
            for (int y = 0; y < offset * 2; y++)
            {
                exploringBoard[x, y] = true;
            }
        }
        //
        int pastSteps = Math.Min(PastFOW, Round + 1);
        for (int x = 0; x < width + offset * 2; x++)
        {
            for (int y = 0; y < height + offset * 2; y++)
            {
                if (exploringBoard[x, y]) continue;
                //Iterate for each buster for each step
                bool covered = false;
                for (int i = 0; i < pastSteps; i++)
                {
                    foreach (string busterID in BusterIDs)
                    {
                        EntityState buster = Entities[busterID].Positions[Round - i];
                        int dx = buster.X / 100 + offset - x; int dy = buster.Y / 100 + offset - y;
                        if (dx > 22 || dx < -22 || dy > 22 || dy < -22) continue;
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
        //
        for (int y = 0; y < height + offset * 2; y++)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < width + offset * 2; x++)
            {
                sb.Append(((x == offset) || (y == offset) || (x == width + offset) || (y == height + offset)) ? "#" : (exploringBoard[x, y] ? "." : "X"));
            }
            //Console.Error.WriteLine(sb.ToString());
        }
    }
    private Point getExplore(string busterID)
    {
        if (exploring.ContainsKey(busterID))
        {
            EntityState es = Entities[busterID].Positions[Round];
            Point target = exploring[busterID];
            if (Distance(es.X, es.Y, target.X, target.Y) > 50)
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
                        if (dx > 22 || dx < -22 || dy > 22 || dy < -22) continue;
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
        updateContestedGhosts();
        updateExploringBoard();

        //Dispatch stunned
        Dashboard dashboard = Dashboards[Round];
        dashboard.Strategies.Add("Strategies for round " + Round);
        foreach (string busterID in BusterIDs)
        {
            int state = Entities[busterID].Positions[Round].State;
            if (state == StateStunned)
            {
                DispatchAction action = new DispatchAction() { Type = DispatchActionType.Stunned };
                dashboard.DispatchActions[busterID] = action;
            }
        }

        //Dispatch stun - for every non-stunned enemy buster, find the corresponding stun-capable buster
        if (dashboard.StunTimers.Count(d => d.Value == 0) > 0) //Has available stun quota
        {
            int enemyTeamID = 1 - TeamID;
            List<Entity> roamingEnemies = Entities
                .Where(d => d.Value.Type == enemyTeamID && d.Value.Positions.ContainsKey(Round))
                .Where(d => d.Value.Positions[Round].State != StateStunned)
                .OrderByDescending(e => e.Value.Positions[Round].State == StateCarrying ? 10 : e.Value.Positions[Round].State) //Stun ghost carrying busters first
                .Select(d => d.Value).ToList();
            foreach (Entity enemyBuster in roamingEnemies)
            {
                List<string> nonDispatchedAvailableBusters = BusterIDs
                    .Where(bid => !dashboard.DispatchActions.ContainsKey(bid))
                    .Where(bid => dashboard.StunTimers[bid] == 0).ToList();
                foreach (string busterID in nonDispatchedAvailableBusters)
                {
                    EntityState es = Entities[busterID].Positions[Round];
                    EntityState esEnemy = enemyBuster.Positions[Round];
                    int distance = Distance(es.X, es.Y, esEnemy.X, esEnemy.Y);
                    if (distance < LimStunMax)
                    {
                        DispatchAction action = new DispatchAction() { Type = DispatchActionType.Stun, EntityID = enemyBuster.ID };
                        dashboard.DispatchActions[busterID] = action;
                        addStun(busterID, "" + (1 - TeamID) + "," + enemyBuster.ID);
                        break;
                    }
                }
            }
        }

        //Release and Return
        foreach (string busterID in BusterIDs.Where(bid => !dashboard.DispatchActions.ContainsKey(bid)))
        {
            EntityState es = Entities[busterID].Positions[Round];
            if (es.State == StateCarrying)
            {
                int distance = distanceFromHome(es.X, es.Y);
                //Release
                if (distance < LimBaseRange)
                {
                    DispatchAction releaseAction = new DispatchAction() { Type = DispatchActionType.Release, EntityID = es.Value };
                    dashboard.DispatchActions[busterID] = releaseAction;
                    string key = "-1" + "," + es.Value;
                    Entities[key].Captured = true;
                    continue;
                }
                //Return
                DispatchAction returnAction = new DispatchAction() { Type = DispatchActionType.Return, Target = Home, EntityID = es.Value };
                dashboard.DispatchActions[busterID] = returnAction;
            }
        }

        //Intercept

        //Bust
        int LimBustStamina = 16;
        int limBustStamina = LimBustStamina;
        if (Round > 40) limBustStamina = 100;
        if (Round > 20 && GhostCount < 10) limBustStamina = 100;
        if (Entities.Count(e => e.Value.Captured) > (GhostCount / 4)) limBustStamina = 100;
        List<Entity> visibleGhosts = Entities
            .Where(d => !d.Value.Captured && d.Value.Type == GhostTypeID && d.Value.Positions.ContainsKey(Round))
            .Where(d => d.Value.Positions[Round].State < limBustStamina)
            .OrderBy(d => d.Value.Positions[Round].State)
            .Select(d => d.Value).ToList();
        foreach (Entity ghost in visibleGhosts)
        {
            foreach (string busterID in BusterIDs.Where(bid => !dashboard.DispatchActions.ContainsKey(bid)))
            {
                EntityState es = Entities[busterID].Positions[Round];
                EntityState esGhost = ghost.Positions[Round];
                int distance = Distance(es.X, es.Y, esGhost.X, esGhost.Y);
                if (distance > LimBustMin && distance < LimBustMax)
                {
                    DispatchAction action = new DispatchAction() { Type = DispatchActionType.Bust, EntityID = ghost.ID };
                    dashboard.DispatchActions[busterID] = action;
                    if (esGhost.State < 10)
                        break;
                    else
                        continue;
                }
                if (distance > LimBustMax && distance < LimVisibility)
                {
                    DispatchAction action = new DispatchAction() { Type = DispatchActionType.Chase, Target = new Point(esGhost.X, esGhost.Y), EntityID = ghost.ID };
                    dashboard.DispatchActions[busterID] = action;
                    continue;
                }
                if (distance > LimBustMax && distance < 6000 && esGhost.State > 15)
                {
                    DispatchAction action = new DispatchAction() { Type = DispatchActionType.Chase, Target = new Point(esGhost.X, esGhost.Y), EntityID = ghost.ID };
                    dashboard.DispatchActions[busterID] = action;
                    continue;
                }
                if (esGhost.State <= 1)
                {
                    DispatchAction action = new DispatchAction() { Type = DispatchActionType.Chase, Target = new Point(esGhost.X, esGhost.Y), EntityID = ghost.ID };
                    dashboard.DispatchActions[busterID] = action;
                    continue;
                }
                if (distance < LimBustMin)
                {
                    DispatchAction action = new DispatchAction() { Type = DispatchActionType.Chase, Target = new Point(es.X, es.Y), EntityID = ghost.ID };
                    dashboard.DispatchActions[busterID] = action;
                    continue;
                }
            }
        }

        //Contest
        foreach (int ghostID in dashboard.ContestedGhostsIDs)
        {
            EntityState es = Entities.First(e => e.Value.Type == GhostTypeID && e.Value.ID == ghostID).Value.Positions[Round];
            int busters = es.Value;
            foreach (string busterID in BusterIDs.Where(bid => !dashboard.DispatchActions.ContainsKey(bid)))
            {
                DispatchAction action = new DispatchAction() { Type = DispatchActionType.Contest, Target = new Point(es.X, es.Y), EntityID = ghostID };
                dashboard.DispatchActions[busterID] = action;
                break;
            }
        }

        //Find
        foreach (string busterID in BusterIDs.Where(bid => !dashboard.DispatchActions.ContainsKey(bid)))
        {
            Point random = getExplore(busterID);
            DispatchAction action = new DispatchAction() { Type = DispatchActionType.Roam, Target = random };
            dashboard.DispatchActions[busterID] = action;
        }

        // MOVE x y | BUST id | RELEASE | STUN
        List<string> moves = new List<string>();
        foreach (string busterID in BusterIDs)
        {
            DispatchAction action = dashboard.DispatchActions[busterID];
            if (action.Type == DispatchActionType.Stunned)
            {
                EntityState stunnedBuster = Entities[busterID].Positions[Round];
                moves.Add("MOVE " + stunnedBuster.X + " " + stunnedBuster.Y + " Stunned");
            }
            if (action.Type == DispatchActionType.Stun)
            {
                moves.Add("STUN " + action.EntityID + " Stun! " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Return)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Return " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Release)
            {
                moves.Add("RELEASE" + " Release " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Intercept)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Intercept! " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Bust)
            {
                moves.Add("BUST " + action.EntityID + " Bust " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Contest)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Contest " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Chase)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Chase " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Herd)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Herd " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Find)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Find " + action.EntityID);
            }
            if (action.Type == DispatchActionType.Roam)
            {
                moves.Add("MOVE " + action.Target.X + " " + action.Target.Y + " Roam");
            }
        }

        //
        foreach (string line in dashboard.Predictions)
        {
            Console.Error.WriteLine(line);
        }
        foreach (string line in dashboard.Updates)
        {
            Console.Error.WriteLine(line);
        }
        //
        string stunTimer = "";
        foreach (string busterID in BusterIDs)
        {
            stunTimer += "(" + busterID + ")->" + dashboard.StunTimers[busterID] + " ";
        }
        stunTimer += "|| ";
        foreach (string busterID in EnemyBusterIDs)
        {
            stunTimer += "(" + busterID + ")->" + dashboard.StunTimers[busterID] + " ";
        }
        Console.Error.WriteLine("Timers " + stunTimer);
        //
        string contestedGhosts = "";
        foreach (int ghostID in dashboard.ContestedGhostsIDs)
        {
            EntityState es = Entities.First(e => e.Value.Type == GhostTypeID && e.Value.ID == ghostID).Value.Positions[Round];
            int busters = es.Value;
            contestedGhosts += "" + ghostID + "->" + busters + " ";
        }
        Console.Error.WriteLine("Contested " + contestedGhosts);
        //
        foreach (string line in dashboard.Strategies)
        {
            Console.Error.WriteLine(line);
        }
        //
        Round++; return moves;

        /*
        //
        Dictionary<int, string> ghostChasedBy = new Dictionary<int, string>();
        Dictionary<int, string> busterStunnedBy = new Dictionary<int, string>();

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
            int stunBusterID = -1;
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
                int d = Distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
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
                int d = Distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
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
                int d = Distance(buster.X, buster.Y, g.Positions[Round].X, g.Positions[Round].Y);
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
        */
    }
}

class Program
{
    static void Test(string[] args)
    {
        //2 Busters
        Player player = new Player(2, 1, 0);

        List<Point> players = new List<Point>() { new Point(1000, 1000), new Point(1000, 1000) };

        //
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        while (true)
        {
            for (int i = 0; i < players.Count(); i++)
            {
                player.Update(0, i, players[i].X, players[i].Y, 0, 0);
            }

            //Process the moves
            stopWatch.Reset();
            stopWatch.Start();
            //
            List<string> moves = player.GetMoves();
            //
            stopWatch.Stop();
            long time = stopWatch.ElapsedMilliseconds;
            //
            Console.Error.WriteLine("Time " + time + "ms");
            for (int i = 0; i < players.Count(); i++)
            {
                string[] tokens = moves[i].Split(' ');
                int x = int.Parse(tokens[1]);
                int y = int.Parse(tokens[2]);
                int distance = Player.Distance(players[i].X, players[i].Y, x, y);
                if (distance <= 800)
                {
                    players[i] = new Point(x, y);
                }
                else
                {
                    int dx = x - players[i].X;
                    int dy = y - players[i].Y;
                    int finalX = Convert.ToInt32(Math.Round(800.0 * dx / distance + players[i].X));
                    int finalY = Convert.ToInt32(Math.Round(800.0 * dy / distance + players[i].Y));
                    players[i] = new Point(finalX, finalY);
                }
                Console.Error.WriteLine("Player " + i + " target " + x + "," + y + " actual " + players[i].X + "," + players[i].Y);
            }
        }
    }

    static void Main(string[] args)
    {
        int bustersPerPlayer = int.Parse(Console.ReadLine()); // the amount of busters you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right

        Player player = new Player(bustersPerPlayer, ghostCount, myTeamId);

        while (true)
        {
            //
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
