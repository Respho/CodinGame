using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

class Player
{
    const int MAXDEPTH = 25000;
    const int BLACKLIST_LIFETIME = 5;
    const int WIDTH = 35; const int HEIGHT = 20;
    const int MAX = 13; const int MIN = 3;
    static Random _random = new Random();

    public enum Direction { Stay, Up, Right, Down, Left }
    public enum Turn { Stay, Front, Right, Left, Back }

    //Is the next point going straight/left/right/back for the player
    static Turn getTurn(int fromX, int fromY, Direction dir, int toX, int toY)
    {
        return Turn.Front;
    }
    static Direction getDirection(Point from, Point to)
    {
        if (from.Equals(to)) return Direction.Stay;
        if (from.X > to.X) return Direction.Left;
        if (from.X < to.X) return Direction.Right;
        if (from.Y > to.Y) return Direction.Down;
        if (from.Y < to.Y) return Direction.Up;
        return Direction.Stay;
    }
    static Point getPoint(Point from, Direction dir)
    {
        return new Point(1, 1);
    }

    static int manhattan(Point from, int x, int y)
    {
        return Math.Abs(x - from.X) + Math.Abs(y - from.Y);
    }

    public struct Black
    {
        public int X, Y, Life;
        public Black(int x, int y)
        {
            X = x; Y = y; Life = BLACKLIST_LIFETIME;
        }
    }

    public struct Opponent
    {
        public Point Pos;
        public int Cells, BIT;
        public Direction Direction;
    }

    public class State
    {
        public int Serial, Round;
        public Point Pos;
        public int Cells, BIT;
        public Direction Direction;
        public Opponent[] Opponents;
        public char[,] Board = new char[WIDTH, HEIGHT];
        public int CellsLeft = 0;
    }

    public class Objective
    {
        public int x, y;
        public int width, height;
        public int x2, y2;
        public bool Valid;
        public int ValueIn, ValuePeri;
        public int ValueDetail, CostDetail;
        public double ScoreRough, ScoreDetail, Risk;
        public State State;

        public string Print()
        {
            string result = "o " + x + "," + y + " " + width + "x" + height + " " + ValueDetail + "/" + CostDetail + " " +
                ScoreDetail.ToString("#.##") + " " + Risk.ToString("#.##");
            return result;
        }

        public Objective(int x1, int y1, int w, int h, State state)
        {
            x = x1; y = y1;
            width = w; height = h;
            x2 = x + width - 1; y2 = y + height - 1;
            this.Valid = false;
            ValueIn = 0; ValuePeri = 0; CostDetail = 0;
            ScoreRough = -1000; ScoreDetail = -1000; Risk = 0;
            this.State = state;
        }

        public void CalcValid()
        {
            //Inside
            for (int i = 0; i < State.Opponents.Length; i++)
            {
                if ((State.Opponents[i].Pos.X >= x) && (State.Opponents[i].Pos.X <= x2) &&
                    (State.Opponents[i].Pos.Y >= y) && (State.Opponents[i].Pos.Y <= y2))
                {
                    Valid = false; return;
                }
            }
            //Not colored and has value
            bool valid = true;
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (State.Board[i, j] == '.')
                    {
                        if (i != x && i != x2 && j != y && j != y2) ValueIn++;
                        else ValuePeri++;
                    }
                    else if (State.Board[i, j] != '0')
                    {
                        valid = false;
                        break;
                    }
                }
                if (!valid) break;
            }
            //
            Valid = valid && (ValueIn > 0) && (ValuePeri > 0);
        }

        int distanceToPerimeter(Point p)
        {
            if (isInside(p)) return Math.Min(Math.Min(p.X - x, x2 - p.X), Math.Min(p.Y - y, y2 - p.Y));
            int distX = ((p.X >= x) && (p.X <= x2)) ? 0 : Math.Min(Math.Abs(x - p.X), Math.Abs(x2 - p.X));
            int distY = ((p.Y >= y) && (p.Y <= y2)) ? 0 : Math.Min(Math.Abs(y - p.Y), Math.Abs(y2 - p.Y));
            return distX + distY;
        }

        bool isInside(Point p)
        {
            return (p.X > x) && (p.X < x2) && (p.Y > y) && (p.Y < y2);
        }

        public void CalcScoreRough()
        {
            if (!Valid) { ScoreRough = -1000; return; }

            //The travel cost is approximated as the distance to reach the perimeter
            int travelCost = distanceToPerimeter(State.Pos);
            //
            int perimeter = width + width + height + height - 4;
            int costFactored = Convert.ToInt32(ValuePeri) * 2;
            int perimeterCost = Math.Min(perimeter, costFactored);
            //
            int totalCost = travelCost + perimeterCost;
            //Path to perimeter has value
            int valueToPerimeter = travelCost / 2;
            int totalValue = valueToPerimeter + ValueIn + ValuePeri;
            //
            ScoreRough = 1.0 * totalValue / totalCost;
        }

        /// <summary>
        /// Super recursive function to find the max value when travelling
        /// </summary>
        Tuple<int, Point> valueTo(int fromX, int fromY, Point to)
        {
            int count = 0;
            //Check size
            int dist = manhattan(to, fromX, fromY);
            if (dist > 12)
            {
                //Count inside empty cells and return an estimate
                for (int i = Math.Min(fromX, to.X); i < Math.Max(fromX, to.X); i++)
                    for (int j = Math.Min(fromY, to.Y); j < Math.Max(fromY, to.Y); j++)
                        if (!(i == to.X && j == to.Y) && State.Board[i, j] == '.') count++;
                return new Tuple<int, Point>(count / 3, to);
            }
            //Degenerate cases
            if ((to.X == fromX) && (to.Y == fromY)) return new Tuple<int, Point>(0, to);
            if (fromX == to.X)
            {
                for (int j = Math.Min(fromY, to.Y); j < Math.Max(fromY, to.Y); j++)
                    if (State.Board[fromX, j] == '.') count++;
                return new Tuple<int, Point>(count, to);
            }
            if (fromY == to.Y)
            {
                for (int i = Math.Min(fromX, to.X); i < Math.Max(fromX, to.X); i++)
                    if (State.Board[i, fromY] == '.') count++;
                return new Tuple<int, Point>(count, to);
            }
            //Early exit if the whole area is filled
            for (int i = Math.Min(fromX, to.X); i < Math.Max(fromX, to.X); i++)
                for (int j = Math.Min(fromY, to.Y); j < Math.Max(fromY, to.Y); j++)
                    if (!(i == to.X && j == to.Y) && State.Board[i, j] == '.') count++;
            if (count == 0) return new Tuple<int, Point>(0, to);

            //Actual calculation
            int basisX = Math.Sign(to.X - fromX); int basisY = Math.Sign(to.Y - fromY);
            Point p = new Point();
            int moveX = valueTo(fromX + basisX, fromY, to).Item1;
            int moveY = valueTo(fromX, fromY + basisY, to).Item1;
            if (moveX >= moveY) { p.X = fromX + basisX; p.Y = fromY; }
            else { p.X = fromX; p.Y = fromY + basisY; }
            int max = (State.Board[fromX, fromY] == '.' ? 1 : 0) + Math.Max(moveX, moveY);
            return new Tuple<int, Point>(max, p);
        }

        /// <summary>
        /// Detail costing to reach the perimeter and complete the perimeter, returns the first point of contact
        /// </summary>
        Point costDetail()
        {
            //Look for unfilled points in the perimeter
            List<Point> points = new List<Point>();
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (i == x || i == x2 || j == y || j == y2)
                    {
                        if (State.Board[i, j] == '.')
                        {
                            points.Add(new Point(i, j));
                        }
                    }
                }
            }

            //Travel to each point
            Point closest = points.OrderBy(p => manhattan(p, State.Pos.X, State.Pos.Y)).First();
            Point poc = new Point(closest.X, closest.Y);
            int totalDist = 0;
            Point current = new Point(State.Pos.X, State.Pos.Y);
            while (points.Count > 0)
            {
                closest = points.OrderBy(p => manhattan(p, current.X, current.Y)).First();
                totalDist += manhattan(closest, current.X, current.Y);
                current.X = closest.X; current.Y = closest.Y;
                points.Remove(closest);
            }

            //
            CostDetail = totalDist;
            return poc;
        }

        public void CalcScoreDetail()
        {
            if (!Valid) { ScoreDetail = -1000; return; }

            //Calculate total cost
            Point poc = costDetail();
            int totalCost = CostDetail;

            //Calculate value to POC
            int valueToPOC = 0;
            if (!isInside(State.Pos))
            {
                //Coming from outside
                valueToPOC = valueTo(State.Pos.X, State.Pos.Y, poc).Item1;
            }
            //
            ValueDetail = valueToPOC + ValueIn + ValuePeri;

            //Risk analysis
            Risk = 0;
            int minDist = 100;
            foreach (Opponent o in State.Opponents)
            {
                int dist = distanceToPerimeter(o.Pos);
                minDist = Math.Min(minDist, dist);
            }
            if (distanceToPerimeter(State.Pos) >= minDist)
            {
                Risk += 0.2;
            }
            if (distanceToPerimeter(State.Pos) > 12)
            {
                Risk += 0.2;
            }
            //
            ScoreDetail = (1.0 - Risk) * ValueDetail / totalCost;
        }

        public Point CalcStep()
        {
            //Outside
            if (!isInside(State.Pos) && distanceToPerimeter(State.Pos) > 0)
            {
                //Find closest perimeter
                List<Point> points = new List<Point>();
                for (int i = x; i <= x2; i++)
                {
                    for (int j = y; j <= y2; j++)
                    {
                        if (i == x || i == x2 || j == y || j == y2)
                        {
                            if (State.Board[i, j] == '.')
                            {
                                points.Add(new Point(i, j));
                            }
                        }
                    }
                }
                Point closest = points.OrderBy(p => manhattan(p, State.Pos.X, State.Pos.Y)).First();
                //Find best path
                return valueTo(State.Pos.X, State.Pos.Y, closest).Item2;
            }

            //On the perimeter
            int bestDist = 100;
            Point step = new Point();
            for (int i = x; i <= x2; i++)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (i == x || i == x2 || j == y || j == y2)
                    {
                        if (State.Board[i, j] == '.')
                        {
                            int dist = manhattan(State.Pos, i, j);
                            if (dist < bestDist)
                            {
                                step.X = i; step.Y = j; bestDist = dist;
                            }
                        }
                    }
                }
            }
            //
            return step;
        }
    }

    public class Game
    {
        public Objective CurrentObjective = null;
        public int StateSerial = 0;
        public int SearchHeight = MAX, SearchWidth = MAX;
        public List<Point> InvalidSizes = new List<Point>();
        public List<State> States = new List<State>();
        public State CurrentState = null;
        public List<Black> BlackList = new List<Black>();

        public void Update(int r, Point pos, int b, Opponent[] opponents, string[] board)
        {
            State s = new State();
            s.Serial = StateSerial; s.Round = r; s.Pos = pos; s.BIT = b;
            s.Opponents = opponents;
            for (int i = 0; i < opponents.Length; i++)
            {
                s.Opponents[i].Direction = (StateSerial == 0 ? Direction.Stay :
                    getDirection(CurrentState.Opponents[i].Pos, opponents[i].Pos));
                if (opponents[i].Pos.Equals(pos)) BlackList.Add(new Black(pos.X, pos.Y));
            }
            s.Direction = (StateSerial == 0 ? Direction.Stay : getDirection(CurrentState.Pos, pos));
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    s.Board[i, j] = board[j][i];
                    if (s.Board[i, j] == '.') s.CellsLeft++;
                    if (s.Board[i, j] == '0') s.Cells++;
                    if (s.Board[i, j] == '1') s.Opponents[0].Cells++;
                    if (s.Board[i, j] == '2') s.Opponents[1].Cells++;
                    if (s.Board[i, j] == '3') s.Opponents[2].Cells++;
                }
            }
            //
            CurrentState = s;
            States.Add(CurrentState);
            StateSerial++;
        }

        bool existsInBlackList(int x, int y)
        {
            return BlackList.Exists(b => (b.X == x) && (b.Y == y) && (b.Life > 0));
        }

        void rotateBlackList()
        {
            for (int i = 0; i < BlackList.Count; i++)
            {
                Black b = BlackList[i]; b.Life--;
            }
        }

        bool isCurrentObjectiveValid()
        {
            if (CurrentObjective == null) return false;
            CurrentObjective = new Objective(CurrentObjective.x, CurrentObjective.y, CurrentObjective.width, CurrentObjective.height, CurrentState);
            CurrentObjective.CalcValid();
            CurrentObjective.CalcScoreRough();
            CurrentObjective.CalcScoreDetail();
            return CurrentObjective.Valid;
        }

        Objective getNewObjective()
        {
            //
            int depthCount = 0;
            List<Objective> objectives = new List<Objective>();
            if (CurrentState.Opponents.Length > 1)
            {
                SearchWidth = MAX - 1; SearchHeight = MAX - 1;
            }
            for (int s = SearchWidth; s >= MIN; s--)
            {
                if (!InvalidSizes.Exists(i => i.X == s && i.Y == s))
                {
                    List<Objective> list = new List<Objective>();
                    for (int i = 0; i <= WIDTH - s; i++)
                    {
                        for (int j = 0; j <= HEIGHT - s; j++)
                        {
                            Objective o = new Objective(i, j, s, s, CurrentState);
                            o.CalcValid();
                            o.CalcScoreRough();
                            list.Add(o);
                            depthCount++;
                        }
                    }
                    if (list.Count(o => o.Valid) == 0) InvalidSizes.Add(new Point(s, s));
                    else objectives.AddRange(list);
                    if (depthCount > MAXDEPTH) break;
                }
            }
            for (int w = SearchWidth; w >= MIN; w--)
            {
                for (int h = SearchHeight; h >= MIN; h--)
                {
                    if (w == h) continue;
                    if (!InvalidSizes.Exists(i => i.X == w && i.Y == h))
                    {
                        List<Objective> list = new List<Objective>();
                        for (int i = 0; i <= WIDTH - w; i++)
                        {
                            for (int j = 0; j <= HEIGHT - h; j++)
                            {
                                Objective o = new Objective(i, j, w, h, CurrentState);
                                o.CalcValid();
                                o.CalcScoreRough();
                                list.Add(o);
                                depthCount++;
                            }
                        }
                        if (list.Count(o => o.Valid) == 0) InvalidSizes.Add(new Point(w, h));
                        else objectives.AddRange(list);
                    }
                    if (depthCount > MAXDEPTH) break;
                }
                if (depthCount > MAXDEPTH) break;
            }
            //
            objectives = objectives.Where(o => o.Valid).OrderByDescending(o => o.ScoreRough).ToList();
            //
            objectives = objectives.Take(Math.Min(objectives.Count, 200)).ToList();
            foreach (Objective o in objectives) o.CalcScoreDetail();
            objectives = objectives.Where(o => o.ScoreDetail > 0.9).OrderByDescending(o => o.ScoreDetail).ToList();
            //
            return (objectives.Count() > 0 ? objectives.First() : null);
        }

        Point getCluster()
        {
            int[,] cluster = new int[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    cluster[i, j] = 0;
                }
            }
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (CurrentState.Board[i, j] == '.')
                    {
                        cluster[i, j]++;
                        if (i - 1 >= 0 && j - 1 >= 0) cluster[i - 1, j - 1]++;
                        if (i + 1 < WIDTH && j - 1 >= 0) cluster[i + 1, j - 1]++;
                        if (i - 1 >= 0 && j + 1 < HEIGHT) cluster[i - 1, j + 1]++;
                        if (i + 1 < WIDTH && j + 1 < HEIGHT) cluster[i + 1, j + 1]++;
                        if (i - 1 >= 0) cluster[i - 1, j]++;
                        if (j - 1 >= 0) cluster[i, j - 1]++;
                        if (i + 1 < WIDTH) cluster[i + 1, j]++;
                        if (j + 1 < HEIGHT) cluster[i, j + 1]++;
                    }
                }
            }
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (cluster[i, j] > 0)
                    {
                        if ((CurrentState.Board[i, j] != '.') || existsInBlackList(i, j))
                        {
                            cluster[i, j] = 0;
                            continue;
                        }
                        int dist = manhattan(CurrentState.Pos, i, j);
                        foreach (Opponent o in CurrentState.Opponents)
                        {
                            if (dist > manhattan(o.Pos, i, j))
                            {
                                cluster[i, j] = 0;
                                break;
                            }
                        }
                    }
                }
            }

            //
            cluster[CurrentState.Pos.X, CurrentState.Pos.Y] = 0;
            Point best = new Point(_random.Next(WIDTH), _random.Next(HEIGHT));
            double max = 0;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    double score = (1.0 * cluster[i, j]) / (manhattan(CurrentState.Pos, i, j) + cluster[i, j]);
                    if (score > max)
                    {
                        best.X = i; best.Y = j;
                        max = score;
                    }
                }
            }
            //
            if (max > 0.5)
            {
                string result = "c " + best.X + "," + best.Y + " " + string.Format("{0:N2}", max);
                Console.Error.WriteLine(result);
                return best;
            }

            //Fallback
            int bestDist = 100;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (CurrentState.Board[i, j] == '.')
                    {
                        int dist = manhattan(CurrentState.Pos, i, j);
                        if ((dist > 0) && !existsInBlackList(i, j))
                        {
                            if ((dist < bestDist) ||
                                (dist == bestDist && CurrentState.Direction == getDirection(CurrentState.Pos, new Point(i, j))))
                            {
                                best.X = i; best.Y = j;
                                bestDist = dist;
                            }
                        }
                    }
                }
            }
            return best;
        }

        public void Test()
        {
            Objective o = new Objective(4, 5, 8, 5, CurrentState);
            o.CalcValid();
            o.CalcScoreRough();
            o.CalcScoreDetail();
            Point s = o.CalcStep();
            Point c = getCluster();
        }

        void opening()
        {
            char[,] b = new char[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    b[i, j] = '.';
                }
            }
            //
            for (int x = 0; x < 12; x++)
            {
                int countSpaces = 0;
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        if (CurrentState.Board[i, j] != '.')
                        {
                            char c = CurrentState.Board[i, j];
                            b[i, j] = c;
                            if (i - 1 >= 0) b[i - 1, j] = c;
                            if (j - 1 >= 0) b[i, j - 1] = c;
                            if (i + 1 < WIDTH) b[i + 1, j] = c;
                            if (j + 1 < HEIGHT) b[i, j + 1] = c;
                        }
                        else countSpaces++;
                    }
                }
                Array.Copy(b, CurrentState.Board, WIDTH * HEIGHT);
                if (countSpaces < 10) break;
            }

            //Relax a little
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (CurrentState.Board[i, j] == '.')
                    {
                        b[i, j] = '.';
                        if (i - 1 >= 0) b[i - 1, j] = '.';
                        if (j - 1 >= 0) b[i, j - 1] = '.';
                        if (i + 1 < WIDTH) b[i + 1, j] = '.';
                        if (j + 1 < HEIGHT) b[i, j + 1] = '.';
                    }
                }
            }
            Array.Copy(b, CurrentState.Board, WIDTH * HEIGHT);

            //
            for (int j = 0; j < HEIGHT; j++)
            {
                string line = "";
                for (int i = 0; i < WIDTH; i++)
                {
                    line += CurrentState.Board[i, j];
                }
                Console.Error.WriteLine(line);
            }
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (CurrentState.Board[i, j] == '0') CurrentState.Board[i, j] = '.';
                }
            }
            CurrentState.Board[CurrentState.Pos.X, CurrentState.Pos.Y] = '0';
            Console.Error.WriteLine("------");
            for (int j = 0; j < HEIGHT; j++)
            {
                string line = "";
                for (int i = 0; i < WIDTH; i++)
                {
                    line += CurrentState.Board[i, j];
                }
                Console.Error.WriteLine(line);
            }
        }

        public Point GetStep()
        {
            //
            if (CurrentState.Round == 1) opening();

            //
            if (CurrentState.CellsLeft == 0) return CurrentState.Pos;

            //If current objective is invalid, find a new one
            if (!isCurrentObjectiveValid())
            {
                CurrentObjective = getNewObjective();
            }

            //Reset the search space after the opening
            if (CurrentState.Round == 1) InvalidSizes.Clear();

            //If there is an objective, get to it
            if (CurrentObjective != null)
            {
                Console.Error.WriteLine(CurrentObjective.Print());
                return CurrentObjective.CalcStep();
            }

            //Clustering
            Point best = getCluster();

            //Back in Time
            int maxOpponent = 0;
            if (CurrentState.BIT > 0)
            {
                foreach (Opponent o in CurrentState.Opponents) maxOpponent = Math.Max(o.Cells, maxOpponent);
                if ((maxOpponent > CurrentState.Cells) && (CurrentState.Round > 100))
                {
                    int difference = maxOpponent - CurrentState.Cells;
                    if (difference > (1.0 * CurrentState.CellsLeft / (0.5 + CurrentState.Opponents.Length)))
                    {
                        best.X = -1; best.Y = -1;
                        CurrentObjective = null;
                    }
                }
            }

            //
            rotateBlackList();
            return best;
        }
    }

    static void test(string[] args)
    {
        int opponentCount = 3;

        Game game = new Game();

        int gameRound = 1;
        int x = 13;
        int y = 12;
        int backInTimeLeft = 1;
        Opponent[] opponents = new Opponent[opponentCount];
        opponents[0].Pos = new Point(20, 19);
        opponents[0].BIT = 1;
        opponents[1].Pos = new Point(17, 17);
        opponents[1].BIT = 1;
        opponents[2].Pos = new Point(32, 13);
        opponents[2].BIT = 1;

        string[] board = new string[20];
        board[0]  = "...................................";
        board[1]  = "...................................";
        board[2]  = "...................................";
        board[3]  = "....................333............";
        board[4]  = "............................22.....";
        board[5]  = "....00000000.......................";
        board[6]  = "....0......0.......................";
        board[7]  = "....0......0.......................";
        board[8]  = "....0......0.......................";
        board[9]  = "0000000000..0000...................";
        board[10] = "0000000000...000...................";
        board[11] = "0000000000.0.000...................";
        board[12] = "0000000000.0..00...................";
        board[13] = "0000000000.........................";
        board[14] = "0000000000.........................";
        board[15] = "0000000000...........11............";
        board[16] = "0........0.........................";
        board[17] = "0000000000.........................";
        board[18] = "000000000..........................";
        board[19] = "...................................";

        game.Update(gameRound, new Point(x, y), backInTimeLeft, opponents, board);
        //game.CurrentObjective = new Objective(0, 9, 10, 10, game.CurrentState);
        Point next = game.GetStep();

        x = 9; y = 18;
        board[0] = "...................................";
        board[1] = "...................................";
        board[2] = "...................................";
        board[3] = "....................333............";
        board[4] = "............................22.....";
        board[5] = "....00000000.......................";
        board[6] = "....0......0.......................";
        board[7] = "....0......0.......................";
        board[8] = "....0......0.......................";
        board[9] = "0000000000.........................";
        board[10] = "0000000000.........................";
        board[11] = "0000000000.........................";
        board[12] = "0000000000.........................";
        board[13] = "0000000000.........................";
        board[14] = "0000000000.........................";
        board[15] = "0000000000...........11............";
        board[16] = "0000000000.........................";
        board[17] = "0000000000.........................";
        board[18] = "0000000000.........................";
        board[19] = "...................................";

        gameRound = 2;
        x = 12;
        y = 2;
        backInTimeLeft = 1;
        opponents[0].Pos = new Point(25, 2);
        opponents[1].Pos = new Point(31, 2);
        opponents[2].Pos = new Point(32, 6);
        board[0] = "22222222222222222222222222222222222";
        board[1] = "22222...2222222.2222222222222222222";
        board[2] = "22222...222222222222222222222222222";
        board[3] = "22222...22222222222233322...2222222";
        board[4] = "22222222222222222222222.....2222222";
        board[5] = "22220000000022222222222222..2222222";
        board[6] = "22220222222022222222222333332222222";
        board[7] = "22220222222022222222222222222222222";
        board[8] = "22220222222022222222222222222222222";
        board[9] = "0000000000..22222222222222222222222";
        board[10] = "00000000002222222222222222222222222";
        board[11] = "00000000002222222222222222222222222";
        board[12] = "00000000002222222222222222222222222";
        board[13] = "00000000002222222222222222222222222";
        board[14] = "00000000002222222222222222222222222";
        board[15] = "00000000002222222222211222222222222";
        board[16] = "00000000002222222222222222222222222";
        board[17] = "00000000002222222222222222222222222";
        board[18] = "00000000002222222222222222222222222";
        board[19] = "22222222222222222222222222222222222";

        game.Update(gameRound, new Point(x, y), backInTimeLeft, opponents, board);
        next = game.GetStep();

    }

    static void Main(string[] args)
    {
        string[] inputs;
        int opponentCount = int.Parse(Console.ReadLine());

        Game game = new Game();

        while (true)
        {
            int gameRound = int.Parse(Console.ReadLine());
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]); // Your x position
            int y = int.Parse(inputs[1]); // Your y position
            int backInTimeLeft = int.Parse(inputs[2]); // Remaining back in time
            Opponent[] opponents = new Opponent[opponentCount];
            for (int i = 0; i < opponentCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int opponentX = int.Parse(inputs[0]); // X position of the opponent
                int opponentY = int.Parse(inputs[1]); // Y position of the opponent
                int opponentBackInTimeLeft = int.Parse(inputs[2]); // Remaining back in time of the opponent
                opponents[i].Pos = new Point(opponentX, opponentY);
                opponents[i].BIT = opponentBackInTimeLeft;
            }
            string[] board = new string[20];
            for (int i = 0; i < 20; i++)
            {
                // One line of the map ('.' = free, '0' = you, otherwise the id of the opponent)
                string line = Console.ReadLine();
                board[i] = line;
                //Console.Error.WriteLine(line);
            }

            game.Update(gameRound, new Point(x, y), backInTimeLeft, opponents, board);

            Point next = game.GetStep();
            if (next.X >= 0)
            {
                Console.WriteLine(next.X + " " + next.Y);
            }
            else
            {
                Console.WriteLine("BACK 25");
            }
        }
    }
}
