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
        public Point[] Players = new Point[4];
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
                    newState.Board[next.X, next.Y] = (char)(playerNumber + 47);
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
        public int PlayerNumber, PlayerCount;
        public List<GameState> States = new List<GameState>();
        public string Debug;

        public TronPlayer(int playerNumber, int playerCount)
        {
            PlayerNumber = playerNumber;
            PlayerCount = playerCount;
            States.Add(new GameState());
        }

        void update(int p, Point p0, Point p1)
        {
            //
            GameState currentState = States[States.Count() - 1];
            //Clear
            if (p0.X == -1)
            {
                for (int i = 0; i < GameState.W; i++)
                {
                    for (int j = 0; j < GameState.H; j++)
                    {
                        if (currentState.Board[i, j] == p)
                            currentState.Board[i, j] = GameState.BLANK;
                    }
                }
                currentState.Players[p] = p0;
                return;
            }
            currentState.Board[p0.X, p0.Y] = (char)(p + 48);
            currentState.Board[p1.X, p1.Y] = (char)(p + 48);
            Direction dir = GameState.GetDirection(p0, p1);
            currentState.Directions[p] = dir;
            currentState.Players[p] = p1;
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

                //SpanTree(null, current, playerNumber, opponentNumber, 3 (me them me), 0 (current))
                GameTree root = GameTree.SpanTree(null, currentState, PlayerNumber, 1 - PlayerNumber, 8, 0);
                //alphabeta(origin, depth, -∞, +∞, TRUE)
                TronMinimax tronMinimax = new TronMinimax(PlayerNumber, 1 - PlayerNumber);
                GameTree next = tronMinimax.MinimaxAlphaBeta(root, 3, double.MinValue, double.MaxValue, true);
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
            Debug = "";
            //
            GameState latest = States[States.Count() - 1];
            GameState state = new GameState(latest);
            States.Add(state);

            string[] lines = serialization.Trim().Split('\n');
            for (int i = 1; i <= PlayerCount; i++)
            {
                string[] inputs = lines[i].Split(' ');
                int x0 = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                int y0 = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                int x1 = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                int y1 = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
                //
                update(i - 1, new Point(x0, y0), new Point(x1, y1));
                debug("update " + lines[i]);
            }

            //
            string move = compute();
            return move;
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
                if (player == null) player = new TronPlayer(P, N);
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
            Console.Error.WriteLine(message);
        }
    }
}

