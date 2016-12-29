using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronWindow
{
    public class TronServer
    {
        #region Basics
        public static Dictionary<string, Point> Directions = new Dictionary<string, Point>();
        public const int W = 30, H = 20, Scale = 16;
        TronPlayer player = null;
        TronHuman human = null;
        public List<ServerGameState> States = new List<ServerGameState>();
        public int StatePointer;

        public TronServer()
        {
            if (Directions.Count() == 0)
            {
                Directions.Add("UP", new Point(0, -1));
                Directions.Add("DOWN", new Point(0, 1));
                Directions.Add("RIGHT", new Point(1, 0));
                Directions.Add("LEFT", new Point(-1, 0));
            }
            human = new TronHuman(0);
            player = new TronPlayer(1, 2);
            States.Add(ServerGameState.Initial());
        }

        string Debug = "";
        void debug(string message)
        {
            Debug += message + "\n";
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
        public static List<Point> GetPath(ServerGameState state, Point startPoint, Point endPoint)
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
                    if (neighbor.X >= W || neighbor.X < 0 || neighbor.Y >= H || neighbor.Y < 0) continue;
                    if (state.Board[neighbor.X, neighbor.Y] != '.') continue;
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

        public ServerGameState GetCurrent()
        {
            return States[StatePointer];
        }
        #endregion

        public void PlayOneMove()
        {
            ServerGameState latest = States[States.Count() - 1];
            if (latest.Players.Count() == 1) return;
            //
            ServerGameState state = new ServerGameState(latest);
            States.Add(state);
            StatePointer = States.Count() - 1;

            //
            Debug = "";
            //
            debug("human.GetMove()");
            string moveHuman = human.GetMove(state.GetSerialization());
            debug("updateGameState()");
            updateGameState(state, moveHuman, 0);
            if (latest.Players.Count() == 1) return;
            //
            debug("player.GetMove()");
            string movePlayer = player.GetMove(state.GetSerialization());
            debug("updateGameState()");
            updateGameState(state, movePlayer, 1);
            if (latest.Players.Count() == 1) return;
            //
            state.PlayerDebug = player.Debug;
            state.Moves = moveHuman + "\n" + movePlayer;
            //
            state.ServerDebug = Debug;
        }

        private void updateGameState(ServerGameState state, string move, int playerNo)
        {
            char player = playerNo.ToString()[0];
            List<Point> trail = state.Players[player];
            Point current = trail[trail.Count() - 1];

            //
            if (Directions.ContainsKey(move.Trim()))
            {
                Point direction = Directions[move.Trim()];
                Point next = new Point(current.X + direction.X, current.Y + direction.Y);
                if (next.X < W && next.X >= 0 && next.Y < H && next.Y >= 0)
                {
                    if (state.Board[next.X, next.Y] == '.')
                    {
                        state.Board[next.X, next.Y] = player;
                        trail.Add(next);
                        state.Results += "Moved to " + next.X + "," + next.Y + "\n";
                        return;
                    }
                }
            }

            //Losing
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (state.Board[i, j] == player) state.Board[i, j] = '.';
                }
            }
            state.Players.Remove(player);
            state.Results += "Lost\n";
        }

        public void Move(Point direction)
        {
            human.SetMove(direction);
            PlayOneMove();
        }

        public void Next()
        {
            StatePointer++;
            if (StatePointer >= States.Count()) StatePointer = States.Count() - 1;
        }

        public void Prev()
        {
            StatePointer--;
            if (StatePointer < 0) StatePointer = 0;
        }
    }

    public class ServerGameState
    {
        public const int W = 30, H = 20, Scale = 16;
        public char[,] Board = new char[W, H];
        public Bitmap State = null;
        public Dictionary<char, List<Point>> Players = new Dictionary<char, List<Point>>();
        public string ServerDebug, PlayerDebug, Moves, Results;
        public int ScorePlayer, ScoreHuman;

        static char[,] getBlankBoard()
        {
            char[,] board = new char[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    board[i, j] = '.';
                }
            }
            return board;
        }

        //Utility
        static void setCheckerBoard(char[,] board)
        {
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (i % 2 == 1 && j % 2 == 1)
                        board[i, j] = 'X';
                }
            }
        }

        //Utility
        static char[,] getBoard(string text)
        {
            char[,] board = new char[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    board[i, j] = text[j * GameState.W + i];
                }
            }
            return board;
        }

        public static ServerGameState Initial()
        {
            //
            char[,] board = getBlankBoard();
            Dictionary<char, List<Point>> players = new Dictionary<char, List<Point>>();
            //
            board[0, 0] = '0';
            List<Point> trail0 = new List<Point>();
            trail0.Add(new Point(10, 10));
            players['0'] = trail0;
            List<Point> trail1 = new List<Point>();
            trail1.Add(new Point(25, 18));
            players['1'] = trail1;
            //
            return new ServerGameState(players, board, "", "", "", "", 0, 0);
        }

        public ServerGameState(Dictionary<char, List<Point>> players, char[,] board, string serverDebug, string playerDebug, string moves, string results, int scorePlayer, int scoreHuman)
        {
            Players = players;
            Board = board;
            ServerDebug = serverDebug; PlayerDebug = playerDebug; Moves = moves; Results = results;
            ScorePlayer = scorePlayer; ScoreHuman = scoreHuman;
        }

        public ServerGameState(ServerGameState state)
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
            foreach (char players in state.Players.Keys)
            {
                List<Point> trail = state.Players[players];
                List<Point> newTrail = new List<Point>();
                foreach (Point p in trail)
                {
                    newTrail.Add(p);
                }
                Players[players] = newTrail;
            }
            //
            ScorePlayer = state.ScorePlayer; ScoreHuman = state.ScoreHuman;
        }

        #region Public
        public string GetSerialization()
        {
            //
            string lines = "2 1\n";
            foreach (char key in Players.Keys)
            {
                Point curr = Players[key][Players[key].Count() - 1];
                Point prev = Players[key][Math.Max(Players[key].Count() - 2, 0)];
                lines += prev.X + " " + prev.Y + " " + curr.X + " " + curr.Y + "\n";
            }
            return lines;
        }
        public Bitmap GetState()
        {
            //
            if (State != null) return State;

            //
            State = new Bitmap(W * Scale, H * Scale);
            Pen greenPen = new Pen(Color.FromArgb(255, 0, 180, 0), 10);
            Pen redPen = new Pen(Color.FromArgb(255, 180, 0, 0), 10);
            greenPen.Alignment = PenAlignment.Center;
            redPen.Alignment = PenAlignment.Center;
            using (Graphics g = Graphics.FromImage(State))
            {
                g.Clear(Color.FromArgb(255, 20, 80, 20));
                foreach (char key in Players.Keys)
                {
                    List<Point> trail = Players[key];
                    if (trail.Count() > 1)
                    {
                        for (int i = 1; i < trail.Count(); i++)
                        {
                            Point prev = trail[i - 1];
                            Point curr = trail[i];
                            g.DrawLine(key == '0' ? redPen : greenPen, prev.X * 16 + 8, prev.Y * 16 + 8, curr.X * 16 + 8, curr.Y * 16 + 8);
                        }
                    }
                }
            }

            //
            return State;
        }
        public string GetBoard()
        {
            return "Board";
        }
        #endregion
    }
}
