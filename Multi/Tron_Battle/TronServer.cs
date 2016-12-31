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
        char[] PlayerSetup = { 'H', 'P', 'P', 'P' };

        #region Basics
        public static Dictionary<string, Point> Directions = new Dictionary<string, Point>();
        public const int W = 30, H = 20, Scale = 16;
        TronHuman human = null;
        TronPlayer[] Players = new TronPlayer[4];
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
            Players[0] = new TronPlayer(PlayerSetup.Length, 0);
            Players[1] = new TronPlayer(PlayerSetup.Length, 1);
            Players[2] = new TronPlayer(PlayerSetup.Length, 2);
            Players[3] = new TronPlayer(PlayerSetup.Length, 3);
            States.Add(ServerGameState.Initial());
        }

        string Debug = "";
        void debug(string message)
        {
            Debug += message + "\n";
        }

        public ServerGameState GetCurrent()
        {
            return States[StatePointer];
        }
        #endregion

        public void PlayOneMove()
        {
            //
            Debug = "";
            ServerGameState latest = States[States.Count() - 1];
            if (latest.GameHasEnded()) return;
            //
            ServerGameState state = new ServerGameState(latest);
            States.Add(state);
            StatePointer = States.Count() - 1;
            //
            for (int i = 0; i < PlayerSetup.Length; i++)
            {
                //End of game
                if (state.GameHasEnded()) continue;
                //Skip if player is dead
                if (state.Players[i][0].X < 0) continue;
                //
                string move = "";
                if (PlayerSetup[i] == 'H')
                {
                    debug("human.GetMove()");
                    move = human.GetMove(state.GetSerialization(i));
                }
                else if (PlayerSetup[i] == 'P')
                {
                    debug("player.GetMove()");
                    move = Players[i].GetMove(state.GetSerialization(i));
                    state.PlayerDebug += Players[i].Debug + "\n";
                }
                state.Moves += move + "\n";
                //
                debug("updateGameState()");
                updateGameState(state, move, i);
            }
            //
            state.ServerDebug = Debug;
        }

        private void updateGameState(ServerGameState state, string move, int playerNumber)
        {
            //Update path
            List<Point> trail = state.Players[playerNumber];
            Point current = trail[trail.Count() - 1];
            if (Directions.ContainsKey(move.Trim()))
            {
                Point direction = Directions[move.Trim()];
                Point next = new Point(current.X + direction.X, current.Y + direction.Y);
                if (next.X < W && next.X >= 0 && next.Y < H && next.Y >= 0)
                {
                    if (state.Board[next.X, next.Y] == '.')
                    {
                        state.Board[next.X, next.Y] = playerNumber.ToString()[0];
                        trail.Add(next);
                        state.Results += "Player " + playerNumber + " moved to " + next.X + "," + next.Y + "\n";
                        return;
                    }
                }
            }

            //Losing
            char p = playerNumber.ToString()[0];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (state.Board[i, j] == p) state.Board[i, j] = '.';
                }
            }
            state.Players[playerNumber] = new List<Point>() { new Point(-1, -1) };
            state.Results += "Player " + playerNumber + " lost";
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
        public List<Point>[] Players = null;
        public string PlayerDebug = "", Moves = "", Results = "";
        public string ServerDebug = "";

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

        public static ServerGameState Initial()
        {
            //
            char[,] board = getBlankBoard();
            List<Point>[] players = new List<Point>[4];
            //
            board[10, 10] = '0';
            players[0] = new List<Point>() { new Point(10, 10) };
            board[25, 18] = '1';
            players[1] = new List<Point>() { new Point(25, 18) };
            board[3, 3] = '2';
            players[2] = new List<Point>() { new Point(3, 3) };
            board[27, 4] = '3';
            players[3] = new List<Point>() { new Point(27, 4) };
            //
            return new ServerGameState(players, board, "", "", "", "");
        }

        public ServerGameState(List<Point>[] players, char[,] board, string serverDebug, string playerDebug, string moves, string results)
        {
            Players = players;
            Board = board;
            ServerDebug = serverDebug; PlayerDebug = playerDebug; Moves = moves; Results = results;
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
            Players = new List<Point>[state.Players.Length];
            for (int k = 0; k < Players.Length; k++)
            {
                List<Point> trail = state.Players[k];
                List<Point> newTrail = new List<Point>();
                foreach (Point p in trail)
                {
                    newTrail.Add(p);
                }
                Players[k] = newTrail;
            }
        }

        #region Public
        public bool GameHasEnded()
        {
            int players = 0;
            foreach (List<Point> trail in Players)
            {
                if (trail[0].X >= 0) players++;
            }
            return players <= 1;
        }
        public string GetSerialization(int playerNumber)
        {
            //
            string lines = Players.Length.ToString() + " " + playerNumber.ToString() + "\n";
            for (int k = 0; k < Players.Length; k++)
            {
                Point curr = Players[k][Players[k].Count() - 1];
                Point prev = Players[k][Math.Max(Players[k].Count() - 2, 0)];
                lines += prev.X + " " + prev.Y + " " + curr.X + " " + curr.Y + "\n";
            }
            return lines;
        }
        public Bitmap GetState()
        {
            //
            if (State != null) return State;

            //
            List<Color> colors = new List<Color>() { Color.FromArgb(255, 180, 0, 0), Color.FromArgb(255, 0, 180, 0), Color.FromArgb(255, 180, 0, 180), Color.FromArgb(255, 180, 180, 0) };
            State = new Bitmap(W * Scale, H * Scale);
            using (Graphics g = Graphics.FromImage(State))
            {
                //
                g.DrawImage(new Bitmap(@"Tron.png"), 0, 0, 480, 320);
                //
                for (int k = 0; k < Players.Length; k++)
                {
                    //
                    Pen pen = new Pen(colors[k], 10);
                    pen.Alignment = PenAlignment.Center;
                    Brush brush = new SolidBrush(colors[k]);
                    //
                    List<Point> trail = Players[k];
                    if (trail.Count() > 1)
                    {
                        for (int i = 1; i < trail.Count(); i++)
                        {
                            Point prev = trail[i - 1];
                            Point curr = trail[i];
                            g.DrawLine(pen, prev.X * 16 + 8, prev.Y * 16 + 8, curr.X * 16 + 8, curr.Y * 16 + 8);
                            g.FillEllipse(brush, new Rectangle(new Point(curr.X * 16 + 2, curr.Y * 16 + 2), new Size(11, 11)));
                        }
                    }
                    //
                    Point spot = Players[k][Players[k].Count() - 1];
                    g.FillEllipse(brush, new Rectangle(new Point(spot.X * 16 + 2, spot.Y * 16 + 2), new Size(11, 11)));
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
