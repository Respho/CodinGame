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

        private Dictionary<Point, int> computeMap()
        {
            Dictionary<Point, int> map = new Dictionary<Point, int>();
            return map;
        }

        const int Invalid = -100;
        private double calcScore(GameState.Option option)
        {
            Point p = option.Location;
            GameState currentState = States[States.Count() - 1];
            GameState.Entity my = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team);
            if (my.Position == p) return Invalid;
            //Is the tile empty?
            if (currentState.Board[p.X, p.Y] != '.') return Invalid;
            if (currentState.Entities.Exists(e => e.Position.Equals(p) && e.Type == GameState.EntityType.Bomb)) return Invalid;
            //Is the tile reachable?
            int distance = GetPath(currentState, my.Position, p).Count();
            if (distance == 0) return Invalid;
            //Calc future state - explode all the bombs and mark all the boxes
            GameState futureState = new GameState(currentState);
            //Place a bomb in future state if previous strategy is to bomb
            if (States.Count() >= 2 && States[States.Count() - 2].Strategy.Bomb)
            {
                futureState.Entities.Add(new GameState.Entity() { Team = Team, Type = GameState.EntityType.Bomb, Position = my.Position, Range = my.Range, Rounds = 8 });
            }
            //
            foreach (GameState.Entity bomb in futureState.Entities.Where(e => e.Type == GameState.EntityType.Bomb))
            {
                foreach (Point d in Directions)
                {
                    for (int i = 1; i < bomb.Range; i++)
                    {
                        //
                        Point candidate = new Point(bomb.Position.X + d.X * i, bomb.Position.Y + d.Y * i);
                        if (candidate.X < 0 || candidate.X >= GameState.W || candidate.Y < 0 || candidate.Y >= GameState.H)
                            break;
                        //Mark boxes and stop the blast
                        if ("012".Contains(futureState.Board[candidate.X, candidate.Y]))
                        {
                            futureState.Board[candidate.X, candidate.Y] = '.';
                            break;
                        }
                        //Stop at items
                        if (futureState.Entities.Exists(e => e.Type == GameState.EntityType.Item && e.Position == candidate))
                        {
                            GameState.Entity item = futureState.Entities.First(e => e.Type == GameState.EntityType.Item && e.Position == candidate);
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
            //Calc bombing score
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
                            if ("12".Contains(futureState.Board[candidate.X, candidate.Y])) score++;
                            score++; break;
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
            if (currentState.Entities.Exists(e => e.Type == GameState.EntityType.Item && e.Position == p)) score += 3;
            //
            if (score == 0)
            {
                if (option.Bomb) return 2 * Invalid;
                return Invalid;
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
            return 1.5 * score - delay;
        }

        private string compute()
        {
            //
            Timer.Reset();
            Timer.Start();
            //
            GameState currentState = States[States.Count() - 1];
            Point myPosition = currentState.Entities.First(e => e.Type == GameState.EntityType.Player && e.Team == Team).Position;
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
            List<Tuple<GameState.Option, double>> options = new List<Tuple<GameState.Option, double>>();
            while (Timer.ElapsedMilliseconds < 85 && pointer < around.Count())
            {
                Point p = around[pointer];
                GameState.Option bomb = new GameState.Option() { Location = p, Bomb = true };
                GameState.Option move = new GameState.Option() { Location = p, Bomb = false };
                options.Add(new Tuple<GameState.Option, double>(bomb, calcScore(bomb)));
                options.Add(new Tuple<GameState.Option, double>(move, calcScore(move)));
                pointer++;
            }
            //
            options = options.OrderByDescending(o => o.Item2).ToList();
            GameState.Option strategy = options[0].Item1;
            currentState.Strategy = strategy;

            //
            Timer.Stop();
            long time = Timer.ElapsedMilliseconds;
            debug("Options - " +  options.Count() + " items, " + time + "ms");
            for (int i = 0; i < 8; i++)
            {
                debug((options[i].Item1.Bomb ? "BOMB " : "MOVE ") + options[i].Item1.Location.X + "," + options[i].Item1.Location.Y + " => " + options[i].Item2);
            }

            //
            string action = "MOVE";
            if (States.Count() >= 2 && States[States.Count() - 2].Strategy.Bomb && States[States.Count() - 2].Strategy.Location == myPosition) action = "BOMB";
            Point target = strategy.Location;
            if (myPosition != target)
            {
                List<Point> path = GetPath(currentState, myPosition, target);
                target = path[path.Count() - 1];
            }

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
