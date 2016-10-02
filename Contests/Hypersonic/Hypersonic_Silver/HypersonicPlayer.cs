using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypersonicWindow
{
    public class GameState
    {
        public struct Option
        {
            public Point Location;
            public bool Bomb;
        }
        public enum EntityType { Player = 0, Bomb = 1, Item = 2 }
        public class Entity
        {
            public EntityType Type;
            public System.Drawing.Point Position;
            public int Team;   //ignored if item
            public int Rounds; //1 = extra range, 2 = extra bomb
            public int Range;  //explosion range of player's bombs / bomb's explosion range / ignored
        }
        public const int W = 13, H = 11;
        public char[,] Board = new char[13, 11];
        public List<Entity> Entities = new List<Entity>();
        public List<Point> Blasts = new List<Point>();
        public Option Strategy;

        public GameState(char[,] board, List<Entity> entities)
        {
            Board = board; Entities = entities;
        }

        public GameState(GameState state)
        {
            //
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Board[i, j] = state.Board[i, j];
                }
            }
            //
            foreach (Entity e in state.Entities)
            {
                Entities.Add(new Entity() { Type = e.Type, Team = e.Team, Position = e.Position, Rounds = e.Rounds, Range = e.Range });
            }
        }

        public string Visualize()
        {
            //
            string board = "";
            for (int j = 0; j < H; j++)
            {
                string line = "";
                for (int i = 0; i < W; i++)
                {
                    line += Board[i, j];
                }
                board += line + "\n";
            }
            board = board.Replace("1", "R").Replace("2", "B");
            //
            StringBuilder sb = new StringBuilder(board);
            foreach (Point p in Blasts)
            {
                int index = p.X + p.Y * (W + 1);
                sb[index] = '*';
            }
            //
            foreach (Entity entity in Entities)
            {
                int index = entity.Position.X + entity.Position.Y * (W + 1);
                if (entity.Type == EntityType.Bomb)
                {
                    sb[index] = "012345678"[entity.Rounds];
                }
                else if (entity.Type == EntityType.Item)
                {
                    sb[index] = "xrb"[entity.Rounds];
                }
            }
            //
            return sb.ToString();
        }
    }

    public class HypersonicPlayer
    {
        System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();
        public static List<Point> Directions = new List<Point>();
        public int Team;
        public string Debug;
        public List<GameState> States = new List<GameState>();

        public HypersonicPlayer(int team)
        {
            if (Directions.Count() == 0)
            {
                Directions.Add(new Point(1, 0));
                Directions.Add(new Point(-1, 0));
                Directions.Add(new Point(0, 1));
                Directions.Add(new Point(0, -1));
            }
            Team = team;
        }

        public static int Manhattan(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        /*
        http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
        OPEN = priority queue containing START
        CLOSED = empty set
        while lowest rank in OPEN is not the GOAL:
            current = remove lowest rank item from OPEN
            add current to CLOSED
            for neighbors of current:
            cost = g(current) + movementcost(current, neighbor)
            if neighbor in OPEN and cost less than g(neighbor):
                remove neighbor from OPEN, because new path is better
            if neighbor in CLOSED and cost less than g(neighbor): ⁽²⁾
                remove neighbor from CLOSED
            if neighbor not in OPEN and neighbor not in CLOSED:
                set g(neighbor) to cost
                add neighbor to OPEN
                set priority queue rank to g(neighbor) + h(neighbor)
                set neighbor's parent to current
        */
        public static List<Point> GetPath(GameState state, Point startPoint, Point endPoint)
        {
            Dictionary<Point, Point> Froms = new Dictionary<Point, Point>();
            Dictionary<Point, int> Steps = new Dictionary<Point, int>();
            Dictionary<Point, int> Distances = new Dictionary<Point, int>();
            //
            HashSet<Point> OPEN = new HashSet<Point>() { startPoint };
            HashSet<Point> CLOSED = new HashSet<Point>();

            //
            int Step = 0;
            Froms[startPoint] = startPoint;
            Steps[startPoint] = Step;
            Distances[startPoint] = 0;
            //while lowest rank in OPEN is not the GOAL
            while ((OPEN.Count() > 0) && (OPEN.OrderBy(x => Distances[x]).First() != endPoint))
            {
                //current = remove lowest rank item from OPEN
                //add current to CLOSED
                Point current = OPEN.OrderBy(x => Distances[x]).First();
                OPEN.Remove(current);
                CLOSED.Add(current);
                //for neighbors of current
                List<Point> neighbors = new List<Point>();
                neighbors.Add(new Point(current.X + 1, current.Y));
                neighbors.Add(new Point(current.X - 1, current.Y));
                neighbors.Add(new Point(current.X, current.Y + 1));
                neighbors.Add(new Point(current.X, current.Y - 1));
                foreach (Point neighbor in neighbors)
                {
                    //Is valid neighbor?
                    if (neighbor.X >= GameState.W || neighbor.X < 0 || neighbor.Y >= GameState.H || neighbor.Y < 0) continue;
                    if (state.Board[neighbor.X, neighbor.Y] != '.') continue;
                    if (state.Entities.Exists(e => e.Position.Equals(neighbor) && e.Type == GameState.EntityType.Bomb)) continue;
                    //cost = g(current) + movementcost(current, neighbor)
                    int cost = Distances[current] + 1;
                    //if neighbor in OPEN and cost less than g(neighbor)
                    if (OPEN.Contains(neighbor) && (cost < Distances[neighbor]))
                    {
                        //remove neighbor from OPEN, because new path is better
                        OPEN.Remove(neighbor);
                    }
                    //if neighbor in CLOSED and cost less than g(neighbor)
                    if (CLOSED.Contains(neighbor) && (cost < Distances[neighbor]))
                    {
                        //remove neighbor from CLOSED
                        CLOSED.Remove(neighbor);
                    }
                    //if neighbor not in OPEN and neighbor not in CLOSED:
                    if (!OPEN.Contains(neighbor) && !CLOSED.Contains(neighbor))
                    {
                        //set g(neighbor) to cost
                        Distances[neighbor] = cost;
                        //add neighbor to OPEN
                        OPEN.Add(neighbor);
                        //set neighbor's parent to current
                        Froms[neighbor] = current;
                        Steps[neighbor] = Steps[current] + 1;
                    }
                }
            }

            //
            List<Point> Result = new List<Point>();
            //Impossible
            if (!Froms.ContainsKey(endPoint))
            {
                return Result;
            }
            //
            Point currNode = endPoint;
            while (currNode != startPoint)
            {
                Result.Add(currNode);
                currNode = Froms[currNode];
            }
            //
            return Result;
        }

        private List<GameState> computeFutureStates(Point plantBomb)
        {
            const int Exploded = 99;
            GameState currentState = States[States.Count() - 1];
            GameState.Entity my = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team);
            List<GameState> futureStates = new List<GameState>();
            for (int count = 0; count < 8; count++)
            {
                //Calculate bomb explosions and blast map
                GameState futureState = new GameState(currentState);
                futureState.Entities = futureState.Entities.Where(e => e.Type != GameState.EntityType.Player).ToList();
                //Countdown
                foreach (GameState.Entity bomb in futureState.Entities.Where(e => e.Type == GameState.EntityType.Bomb)) bomb.Rounds--;
                //Add a bomb
                if (count == 0 && plantBomb.X >= 0 && plantBomb.Y >= 0)
                {
                    futureState.Entities.Add(new GameState.Entity() { Type = GameState.EntityType.Bomb, Team = Team, Position = plantBomb, Range = my.Range, Rounds = 8 });
                }
                //Explode the bombs
                List<Point> boxes = new List<Point>();
                while (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Bomb && e.Rounds == 0))
                {
                    GameState.Entity bomb = futureState.Entities.First(e => e.Type == GameState.EntityType.Bomb && e.Rounds == 0);
                    bomb.Rounds = Exploded;
                    //
                    foreach (Point p in Directions)
                    {
                        //Start from 0 to include blast map and trigger bombs at the same location
                        for (int i = 0; i < bomb.Range; i++)
                        {
                            //
                            Point candidate = new Point(bomb.Position.X + p.X * i, bomb.Position.Y + p.Y * i);
                            if (candidate.X < 0 || candidate.X >= GameState.W || candidate.Y < 0 || candidate.Y >= GameState.H)
                                break;
                            futureState.Blasts.Add(candidate);
                            //Trigger nearby bombs
                            if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Bomb && e.Position == candidate && e.Rounds != Exploded))
                            {
                                futureState.Entities.First(e => e.Type == GameState.EntityType.Bomb && e.Position == candidate && e.Rounds != Exploded).Rounds = 0;
                                if (i > 0) break;
                            }
                            //Mark boxes and stop the blast
                            if ("012".Contains(futureState.Board[candidate.X, candidate.Y]))
                            {
                                boxes.Add(candidate);
                                break;
                            }
                            //Stop at items
                            if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Item && e.Position == candidate))
                            {
                                GameState.Entity item = futureState.Entities.First(e => e.Type == GameState.EntityType.Item && e.Position == candidate);
                                futureState.Entities.Remove(item);
                                break;
                            }
                            //Stop at walls
                            if (futureState.Board[candidate.X, candidate.Y] == 'X')
                            {
                                break;
                            }
                        }
                    }
                }
                //Remove bombs
                futureState.Entities = futureState.Entities.Where(e => e.Rounds != Exploded).ToList();

                //Boxes and items
                foreach (Point box in boxes.Distinct())
                {
                    //Drop items
                    if ("12".Contains(futureState.Board[box.X, box.Y]))
                    {
                        futureState.Entities.Add(new GameState.Entity()
                        {
                            Type = GameState.EntityType.Item,
                            Team = 0,
                            Position = box,
                            Rounds = int.Parse("" + futureState.Board[box.X, box.Y]),
                            Range = 0
                        });
                    }
                    //Remove box
                    futureState.Board[box.X, box.Y] = '.';
                }

                //
                futureStates.Add(futureState);
                currentState = futureState;
            }

            //Visualize blast maps
            for (int i = 0; i < 8; i++)
            {
                //debug("Future state " + i + "\n" + futureStates[i].Visualize());
            }

            //
            return futureStates;
        }

        class MapPoint
        {
            public Point Position;
            public List<Point> Trail = new List<Point>();
            public int Depth;
        }

        private Tuple<List<MapPoint>, List<MapPoint>> computeMap(List<GameState> futureStates)
        {
            //
            GameState currentState = States[States.Count() - 1];
            GameState.Entity my = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team);
            //
            List<Point> Movements5 = new List<Point>();
            Movements5.Add(new Point(1, 0));
            Movements5.Add(new Point(-1, 0));
            Movements5.Add(new Point(0, 1));
            Movements5.Add(new Point(0, -1));
            Movements5.Add(new Point(0, 0));
            //
            int depth = 0;
            int count = 0; const int Limit = 200;
            List<MapPoint> map = new List<MapPoint>();
            List<MapPoint> culledMap = new List<MapPoint>();
            map.Add(new MapPoint() { Position = my.Position, Trail = new List<Point>(), Depth = depth });
            Dictionary<int, int> culling = new Dictionary<int, int>();
            bool timeout = false;
            //
            while (depth < 8 && !timeout)
            {
                //Wipe out cells covered by bomb blasts
                GameState futureState = futureStates[depth];
                if (map.Exists(p => p.Depth == depth && futureState.Blasts.Contains(p.Position)))
                {
                    culling[depth] = map.Count(p => p.Depth == depth && futureState.Blasts.Contains(p.Position));
                    map = map.Where(p => !(p.Depth == depth && futureState.Blasts.Contains(p.Position))).ToList();
                    culledMap = new List<MapPoint>();
                    foreach (MapPoint mp in map.Where(mpp => mpp.Depth == depth)) culledMap.Add(mp);
                }
                //Stop if certain death
                if (map.Count(p => p.Depth == depth) == 0)
                {
                    return new Tuple<List<MapPoint>, List<MapPoint>>(null, null);
                }
                //
                if (count > Limit)
                {
                    timeout = true;
                    break;
                }
                //Spawn children
                List<MapPoint> newPoints = new List<MapPoint>();
                foreach (MapPoint mapPoint in map.Where(m => m.Depth == depth))
                {
                    foreach (Point d in Movements5)
                    {
                        Point candidate = new Point(mapPoint.Position.X + d.X, mapPoint.Position.Y + d.Y);
                        //Out of bounds
                        if (candidate.X < 0 || candidate.X >= GameState.W || candidate.Y < 0 || candidate.Y >= GameState.H) continue;
                        //A bomb
                        if (d != new Point(0, 0))
                        {
                            if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Bomb && e.Position == candidate)) continue;
                        }
                        //A box or a wall
                        if (futureState.Board[candidate.X, candidate.Y] != '.') continue;
                        //Add
                        List<Point> trail = new List<Point>();
                        foreach (Point t in mapPoint.Trail) trail.Add(t);
                        trail.Add(mapPoint.Position);
                        newPoints.Add(new MapPoint() { Position = candidate, Trail = trail, Depth = depth + 1 });
                        count++;
                    }
                }
                foreach (MapPoint p in newPoints) map.Add(p);
                depth++;
            }

            //Visualize the map
            debug("culledMap.Count() " + culledMap.Count());
            if (culledMap.Count() > 0)
            {
                foreach (MapPoint point in culledMap)
                {
                    string line = "(" + point.Position.X + ", " + point.Position.Y + ") ";
                    foreach (Point p in point.Trail)
                    {
                        line += "" + p.X + "," + p.Y + "->";
                    }
                    //debug(line);
                }
            }
            bool dump = false;
            if (dump)
            {
                debug("map.Count() " + map.Count());
                int maxDepth = -1;
                if (map.Count() > 0) maxDepth = map.Max(p => p.Depth);
                for (int d = 0; d <= maxDepth; d++)
                {
                    //debug("Points for d=" + d + (culling.ContainsKey(d) ? ", " + culling[d] + " culled" : ""));
                    foreach (MapPoint point in map.Where(p => p.Depth == d))
                    {
                        string line = "(" + point.Position.X + ", " + point.Position.Y + ") ";
                        foreach (Point p in point.Trail)
                        {
                            line += "" + p.X + "," + p.Y + "->";
                        }
                        debug(line);
                    }
                }

            }

            //
            return new Tuple<List<MapPoint>, List<MapPoint>>(map, culledMap);
        }

        const int Invalid = -100;
        private Tuple<double, Point> calcScore(GameState.Option option, List<GameState> futureStates, List<MapPoint> map)
        {
            GameState currentState = States[States.Count() - 1];
            GameState.Entity my = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team);
            Point p = option.Location;

            //
            if (option.Bomb && (option.Location == new Point(0, 0) || option.Location == new Point(12, 0) || option.Location == new Point(12, 10) || option.Location == new Point(0, 10)))
                return new Tuple<double, Point>(Invalid, my.Position);

            //The tile is not worth visiting if it has a bomb, it will kill you
            if (currentState.Entities.Exists(e => e.Position.Equals(p) && e.Type == GameState.EntityType.Bomb))
                return new Tuple<double,Point>(Invalid, my.Position);
            //Is the tile reachable?
            int distance = 0; bool reachable = false; Point nextStep = my.Position;
            if (my.Position == p) reachable = true;
            if (map.Exists(mp => mp.Position == p))
            {
                MapPoint best = map.Where(mp => mp.Position == p).OrderBy(mp => mp.Depth).First();
                distance = best.Depth;
                nextStep = best.Trail.Count() < 2 ? p : best.Trail[1];
                reachable = true;
            }
            //The tile is not covered by the map, so it's either not free or is very far, check with A* provided the tile is empty
            else if (currentState.Board[p.X, p.Y] == '.')
            {
                List<Point> path = GetPath(currentState, my.Position, p);
                distance = path.Count();
                if (distance > 0)
                {
                    reachable = true;
                    nextStep = path[path.Count() - 1];
                }
            }
            if (!reachable) return new Tuple<double, Point>(Invalid, my.Position);

            //Calc bombing score
            GameState futureState = futureStates[Math.Min(distance, 7)];
            int score = 0;
            if (option.Bomb)
            {
                foreach (Point d in Directions)
                {
                    for (int i = 1; i < my.Range; i++)
                    {
                        //
                        Point candidate = new Point(p.X + d.X * i, p.Y + d.Y * i);
                        if (candidate.X < 0 || candidate.X >= GameState.W || candidate.Y < 0 || candidate.Y >= GameState.H)
                            break;
                        //Mark boxes
                        if ("012".Contains(futureState.Board[candidate.X, candidate.Y]))
                        {
                            //Powerups count more
                            if ("12".Contains(futureState.Board[candidate.X, candidate.Y])) score++;
                            score++; break;
                        }
                        //Attack other players
                        if (currentState.Entities.Exists(e => e.Type == GameState.EntityType.Player && e.Team != Team && e.Position == candidate))
                        {
                            if (Manhattan(my.Position, candidate) >= 2) score++;
                        }
                        //Stop at items
                        if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Item && e.Position == candidate))
                        {
                            break;
                        }
                        //Stop at walls
                        if (futureState.Board[candidate.X, candidate.Y] == 'X')
                        {
                            break;
                        }
                    }
                }
            }
            //Calc power-up score
            if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Item && e.Position == p)) score += 3;
            //
            if (score == 0)
            {
                return new Tuple<double, Point>(Invalid, my.Position);
            }

            //Factor in the distance
            int waitTurns = 0;
            if (option.Bomb)
            {
                int countBombs = currentState.Entities.Count(e => e.Type == GameState.EntityType.Bomb && e.Team == Team);
                if (countBombs >= my.Rounds)
                {
                    waitTurns = currentState.Entities.Where(e => e.Type == GameState.EntityType.Bomb && e.Team == Team)
                        .OrderBy(e => e.Rounds).First().Rounds;
                }
            }
            int delay = Math.Max(waitTurns, distance);

            //
            return new Tuple<double,Point>(1.5 * score - delay, nextStep);
        }

        private string compute()
        {
            //
            GameState currentState = States[States.Count() - 1];
            Point myPosition = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team).Position;
            //
            Timer.Reset();
            Timer.Start();
            //
            List<GameState> futureStatesBomb = computeFutureStates(myPosition);
            Tuple<List<MapPoint>, List<MapPoint>> mapsBomb = computeMap(futureStatesBomb);
            //
            List<GameState> futureStates = computeFutureStates(new Point(-1, -1));
            Tuple<List<MapPoint>, List<MapPoint>> maps = computeMap(futureStates);
            if (maps.Item1 == null)
            {
                maps = new Tuple<List<MapPoint>, List<MapPoint>>(new List<MapPoint>(), new List<MapPoint>());
            }
            //
            double planningTime = Timer.ElapsedMilliseconds;
            debug("Planning - " + planningTime + "ms");
            //
            List<Point> around = new List<Point>();
            for (int i = 0; i < GameState.W; i++)
            {
                for (int j = 0; j < GameState.H; j++)
                {
                    around.Add(new Point(i, j));
                }
            }
            around = around.OrderBy(p => Manhattan(p, myPosition)).ToList();
            //
            int pointer = 0;
            List<Tuple<GameState.Option, Tuple<double, Point>>> options = new List<Tuple<GameState.Option, Tuple<double, Point>>>();
            while ((options.Count() < 20) || (Timer.ElapsedMilliseconds < 90 && pointer < around.Count()))
            {
                Point p = around[pointer];
                GameState.Option bomb = new GameState.Option() { Location = p, Bomb = true };
                GameState.Option move = new GameState.Option() { Location = p, Bomb = false };
                options.Add(new Tuple<GameState.Option, Tuple<double, Point>>(bomb, calcScore(bomb, futureStates, maps.Item1)));
                options.Add(new Tuple<GameState.Option, Tuple<double, Point>>(move, calcScore(move, futureStates, maps.Item1)));
                pointer++;
            }
            //
            options = options.OrderByDescending(o => o.Item2.Item1).ToList();
            GameState.Option strategy = options[0].Item1;
            currentState.Strategy = strategy;

            //
            Timer.Stop();
            long time = Timer.ElapsedMilliseconds;
            debug("Options  - " + time + "ms, " + options.Count() + " items");
            for (int i = 0; i < 8; i++)
            {
                debug((options[i].Item1.Bomb ? "BOMB " : "MOVE ") + options[i].Item1.Location.X + "," + options[i].Item1.Location.Y + " => " + options[i].Item2);
            }

            //Determine action
            string action = "MOVE";
            if (States.Count() >= 2 && States[States.Count() - 2].Strategy.Bomb && States[States.Count() - 2].Strategy.Location == myPosition) action = "BOMB";
            debug("Original action is " + action);
            //Target is the strategy's <Score, Nextstep>.Nextstep
            Point target = options[0].Item2.Item2;
            //Evasion
            if (mapsBomb.Item2 == null)
            {
                action = "MOVE";
                debug("Cannot BOMB and run, change to MOVE");
            }
            //
            List<MapPoint> culled = (action == "BOMB") ? mapsBomb.Item2 : maps.Item2;
            if (culled != null && culled.Count() > 0)
            {
                List<Point> validMoves;
                bool hasTrail1 = culled.First().Trail.Count() > 1;
                if (hasTrail1)
                {
                    validMoves = culled.Select(trail => trail.Trail[1]).Distinct().ToList();
                }
                else
                {
                    validMoves = culled.Select(trail => trail.Position).Distinct().ToList();
                }
                if (!validMoves.Contains(target))
                {
                    debug("Evasion from " + target + " to " + validMoves[0]);
                    target = validMoves[0];
                }
            }

            //Leaving a bomb
            if (Manhattan(target, myPosition) == 1 &&
                (currentState.Entities.Exists(b => b.Type == GameState.EntityType.Bomb && b.Position == myPosition) || action == "BOMB"))
            {
                //
                List<Point> availableDirections = new List<Point>();
                int range =  (action == "BOMB") ? currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team).Range : currentState.Entities.Where(b => b.Type == GameState.EntityType.Bomb && b.Position == myPosition).Select(b => b.Range).Max();
                int rounds = (action == "BOMB") ? 8 : currentState.Entities.Where(b => b.Type == GameState.EntityType.Bomb && b.Position == myPosition).Select(b => b.Rounds).Min();
                //Check if in each direction has a corridor and turnaround
                foreach (Point d in Directions)
                {
                    //Corridor must be long enough
                    bool existsCorridor = true;
                    for (int c = 1; c <= range; c++)
                    {
                        Point check = new Point(myPosition.X + d.X * c, myPosition.Y + d.Y * c);
                        if (check.X < 0 || check.X >= GameState.W || check.Y < 0 || check.Y >= GameState.H)
                        {
                            existsCorridor = false;
                            break;
                        }
                        if (currentState.Board[check.X, check.Y] != '.') existsCorridor = false;
                    }
                    //Or have turnaround within time
                    int mustHaveTurnWithin = rounds - 2;
                    bool hasTurnAround = false;
                    for (int offset = 1; offset <= mustHaveTurnWithin; offset++)
                    {
                        //Corridor must exist
                        Point check = new Point(myPosition.X + d.X * offset, myPosition.Y + d.Y * offset);
                        if (check.X < 0 || check.X >= GameState.W || check.Y < 0 || check.Y >= GameState.H) break;
                        if (currentState.Board[check.X, check.Y] != '.') break;
                        //Sides
                        bool checkHorizontal = (check.X != myPosition.X);
                        Point upperLeft = checkHorizontal ? new Point(check.X, check.Y + 1) : new Point(check.X + 1, check.Y);
                        Point lowerRight = checkHorizontal ? new Point(check.X, check.Y - 1) : new Point(check.X - 1, check.Y);
                        bool noUpperLeft = upperLeft.X < 0 || upperLeft.X >= GameState.W || upperLeft.Y < 0 || upperLeft.Y >= GameState.H;
                        bool noLowerRight = lowerRight.X < 0 || lowerRight.X >= GameState.W || lowerRight.Y < 0 || lowerRight.Y >= GameState.H;
                        if ((!noUpperLeft && (currentState.Board[upperLeft.X, upperLeft.Y] == '.')) ||
                            (!noLowerRight && (currentState.Board[lowerRight.X, lowerRight.Y] == '.')))
                        {
                            hasTurnAround = true;
                            break;
                        }
                    }
                    //
                    if (existsCorridor || hasTurnAround) availableDirections.Add(new Point(myPosition.X + d.X, myPosition.Y + d.Y));
                }
                //
                if (availableDirections.Count() > 0 && (!availableDirections.Contains(target)))
                {
                    debug("Changed target from " + target + " to " + availableDirections[0]);
                    target = availableDirections[0];
                }
            }

            //
            int myBombsCount = currentState.Entities.Count(e => e.Team == Team && e.Type == GameState.EntityType.Bomb);
            if (myBombsCount >= 3) action = "MOVE";

            if (options[0].Item2.Item1 < -50) action = "MOVE";

            //
            return action + " " + target.X + " " + target.Y;
        }

        private void update(string serialization)
        {
            string[] lines = serialization.Split('\n');
            //
            char[,] board = new char[GameState.W, GameState.H];
            for (int i = 0; i < GameState.W; i++)
            {
                for (int j = 0; j < GameState.H; j++)
                {
                    board[i, j] = lines[j][i];
                }
            }
            //
            List<GameState.Entity> entities = new List<GameState.Entity>();
            int entitiesCount = int.Parse(lines[11]);
            for (int i = 0; i < entitiesCount; i++)
            {
                string[] tokens = lines[i + 12].Split(' ');
                entities.Add(new GameState.Entity()
                {
                    Type = (GameState.EntityType)Enum.Parse(typeof(GameState.EntityType), tokens[0]),
                    Team = int.Parse(tokens[1]),
                    Position = new Point(int.Parse(tokens[2]), int.Parse(tokens[3])),
                    Rounds = int.Parse(tokens[4]),
                    Range = int.Parse(tokens[5])
                });
            }
            //
            States.Add(new GameState(board, entities));
        }

        public string GetMove(string serialization)
        {
            //
            Debug = "";
            //debug(serialization);
            update(serialization);

            //
            string move = compute();
            return move;
        }

        //Rename this method to Main() when running on CG
        static void MainX(string[] args)
        {
            HypersonicPlayer player = new HypersonicPlayer(int.Parse(Console.ReadLine().Split(' ')[2]));
            //
            while (true)
            {
                //
                string serialization = "";
                for (int i = 0; i < 11; i++)
                {
                    serialization += Console.ReadLine() + "\n";
                }
                int entities = int.Parse(Console.ReadLine());
                serialization += entities.ToString() + "\n";
                for (int i = 0; i < entities; i++)
                {
                    serialization += Console.ReadLine() + "\n";
                }
                //
                string move = player.GetMove(serialization);
                Console.WriteLine(move);
            }
        }

        void debug(string message)
        {
            Debug += message + "\n";
            //
            Console.Error.WriteLine(message);
        }
    }
}
