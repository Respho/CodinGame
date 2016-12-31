using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronWindow
{
    public enum Turn { Front, Right, Left }
    public enum Direction { UP, RIGHT, DOWN, LEFT }

    public class GameState
    {
        //
        public static Point[] DirectionVectors = { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };
        public const int W = 30, H = 20;
        public const char BLANK = (char)46;
        public char[,] Board = new char[W, H];
        //
        public Point[] Players = { new Point(-1, -1), new Point(-1, -1), new Point(-1, -1), new Point(-1, -1) };
        public Direction[] Directions = new Direction[4];
        public Turn[] Turns = new Turn[4];

        public static int Manhattan(Point from, int x, int y)
        {
            return Math.Abs(x - from.X) + Math.Abs(y - from.Y);
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
                    if (neighbor.X != endPoint.X || neighbor.Y != endPoint.Y)
                    {
                        if (neighbor.X >= W || neighbor.X < 0 || neighbor.Y >= H || neighbor.Y < 0) continue;
                        if (state.Board[neighbor.X, neighbor.Y] != BLANK) continue;
                    }
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

        //Get a valid opponent
        public int GetOpponent(int playerNumber, int playerCount, ref string debug)
        {
            Point current = Players[playerNumber];
            int opponent = -1; int min = 99999;
            debug = "";
            //
            for (int i = 0; i < Players.Count(); i++)
            {
                if (playerNumber == i) continue;
                if (Players[i].X < 0) continue;
                List<Point> path = GetPath(this, Players[playerNumber], Players[i]);
                if (opponent == -1) opponent = i;
                //
                debug += "Opponent (" + Players[i].X + "," + Players[i].Y + ") is " + ((path.Count() > 0) ? "" + path.Count() + " apart" : "unreachable") + "\n";
                if ((path.Count() > 0) && (path.Count() < min))
                {
                    min = path.Count();
                    opponent = i;
                }
            }
            //
            return opponent;
        }

        //Get the DOF of a player
        public int GetDOF(int playerNumber)
        {
            Point current = Players[playerNumber];
            int dof = 0;
            foreach (Point v in DirectionVectors)
            {
                if (isValid(new Point(current.X + v.X, current.Y + v.Y))) dof++;
            }
            return dof;
        }

        //
        public static Direction GetDirection(Point from, Point to)
        {
            if (to.Y < from.Y) return Direction.UP;
            if (to.Y > from.Y) return Direction.DOWN;
            if (to.X < from.X) return Direction.LEFT;
            return Direction.RIGHT;
        }

        //Span future states
        public List<GameState> GetChildren(int playerNumber)
        {
            List<GameState> children = new List<GameState>();
            Point current = Players[playerNumber];
            foreach (Point v in DirectionVectors)
            {
                Point next = new Point(current.X + v.X, current.Y + v.Y);
                if (isValid(next))
                {
                    GameState newState = new GameState(this);
                    newState.Board[next.X, next.Y] = (char)(playerNumber + 48);
                    newState.Players[playerNumber] = next;
                    children.Add(newState);
                }
            }
            return children;
        }

        public string GetVisual(List<Point> territoryPlayer, List<Point> territoryOpponent)
        {
            char[,] vis = (char[,])Board.Clone();
            foreach (Point p in territoryPlayer)
            {
                vis[p.X, p.Y] = 'p';
            }
            foreach (Point p in territoryOpponent)
            {
                vis[p.X, p.Y] = 'o';
            }
            string lines = "";
            for (int i = 0; i < H; i++)
            {
                string line = "";
                for (int j = 0; j < W; j++)
                {
                    line += vis[j, i];
                }
                lines += line + "\n";
            }
            return lines;
        }

        //
        public GameState()
        {
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Board[i, j] = BLANK;
                }
            }
        }
        public GameState(GameState state)
        {
            //
            Board = (char[,])state.Board.Clone();
            //
            for (int i = 0; i < 4; i++)
            {
                Players[i] = state.Players[i];
                Directions[i] = state.Directions[i];
                Turns[i] = state.Turns[i];
            }
        }

        //
        public Point GetAnyPoint(int playerNumber)
        {
            Point current = Players[playerNumber];
            if (isValid(new Point(current.X + 1, current.Y))) return new Point(current.X + 1, current.Y);
            if (isValid(new Point(current.X - 1, current.Y))) return new Point(current.X - 1, current.Y);
            if (isValid(new Point(current.X, current.Y + 1))) return new Point(current.X, current.Y + 1);
            if (isValid(new Point(current.X, current.Y - 1))) return new Point(current.X, current.Y - 1);
            return new Point();
        }
        //
        bool isValid(Point p)
        {
            return (p.X >= 0 && p.X < W && p.Y >= 0 && p.Y < H) && Board[p.X, p.Y] == BLANK;
        }
    }

    public class TronPlayer
    {
        System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();
        //
        public int PlayerCount, PlayerNumber;
        public List<GameState> States = new List<GameState>();
        public string Debug;

        public TronPlayer(int playerCount, int playerNumber)
        {
            PlayerCount = playerCount;
            PlayerNumber = playerNumber;
            States.Add(new GameState());
        }

        void update(string serialization)
        {
            //
            Debug = "";
            GameState currentState = States[States.Count() - 1];
            debug("Serialization " + serialization.Trim());
            string[] lines = serialization.Trim().Split('\n');
            for (int k = 0; k < PlayerCount; k++)
            {
                string[] inputs = lines[k + 1].Split(' ');
                int x0 = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                int y0 = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                int x1 = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                int y1 = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
                //
                char player = (char)(k + 48);
                //Clear
                if (x0 < 0)
                {
                    for (int i = 0; i < GameState.W; i++)
                    {
                        for (int j = 0; j < GameState.H; j++)
                        {
                            if (currentState.Board[i, j] == player)
                                currentState.Board[i, j] = GameState.BLANK;
                        }
                    }
                    currentState.Players[k] = new Point(-1, -1);
                    continue;
                }
                //Update
                currentState.Board[x0, y0] = player;
                currentState.Board[x1, y1] = player;
                Direction dir = GameState.GetDirection(new Point(x0, y0), new Point(x1, y1));
                currentState.Directions[k] = dir;
                currentState.Players[k] = new Point(x1, y1);
            }
        }

        string compute()
        {
            //
            GameState currentState = States[States.Count() - 1];
            //
            int dof = currentState.GetDOF(PlayerNumber);
            if (dof > 1)
            {
                //
                Timer.Reset();
                Timer.Start();
                //
                string opponentDebug = "";
                int opponent = currentState.GetOpponent(PlayerNumber, PlayerCount, ref opponentDebug);
                debug(opponentDebug.Trim());
                int depth = 3;
                //SpanTree(null, current, playerNumber, opponentNumber, 3 (me them me), 0 (current))
                GameTree root = GameTree.SpanTree(null, currentState, PlayerNumber, opponent, depth, 0);
                //alphabeta(origin, depth, -∞, +∞, TRUE)
                TronMinimax tronMinimax = new TronMinimax(PlayerNumber, opponent);
                GameTree next = tronMinimax.MinimaxAlphaBeta(root, depth, double.MinValue, double.MaxValue, true);
                //
                string vis = currentState.GetVisual(next.Score.TerritoryPlayer, next.Score.TerritoryOpponent);
                debug("Score - " + next.Score.Score);
                //
                Timer.Stop();
                long time = Timer.ElapsedMilliseconds;
                debug("Time elapsed - " + time + "ms");
                debug(vis);

                //
                return next.Label.Trim('>').Split('>')[0];
            }
            else
            {
                Point target = currentState.GetAnyPoint(PlayerNumber);
                Direction dir = GameState.GetDirection(currentState.Players[PlayerNumber], target);
                return dir.ToString();
            }
        }

        public string GetMove(string serialization)
        {
            //
            States.Add(new GameState(States[States.Count() - 1]));
            //
            update(serialization);
            //
            return compute();
        }

        //Rename this method to Main() when running on CG
        static void MainX(string[] args)
        {
            TronPlayer player = null;
            //
            while (true)
            {
                //
                string line = Console.ReadLine();
                string[] inputs = line.Split(' ');
                int N = int.Parse(inputs[0]); // total number of players (2 to 4).
                int P = int.Parse(inputs[1]); // your player number (0 to 3).
                if (player == null) player = new TronPlayer(N, P);
                //
                string serialization = "";
                serialization += line + "\n";
                for (int i = 0; i < N; i++)
                {
                    line = Console.ReadLine();
                    serialization += line + "\n";
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
            //Console.Error.WriteLine(message);
        }
    }
}

