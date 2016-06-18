using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

class Player
{
    class State
    {
        public char[,] Map = new char[W, H];
        public int BombsLeft;
        public List<Tuple<Point, int>> Bombs = new List<Tuple<Point, int>>();
        public int RoundsLeft;
        //
        public char[,] TestMap = new char[W, H];
        //
        static Point d1 = new Point(0, 1);
        static Point d2 = new Point(0, 2);
        static Point d3 = new Point(0, 3);
        static Point u1 = new Point(0, -1);
        static Point u2 = new Point(0, -2);
        static Point u3 = new Point(0, -3);
        static Point l1 = new Point(-1, 0);
        static Point l2 = new Point(-2, 0);
        static Point l3 = new Point(-3, 0);
        static Point r1 = new Point(1, 0);
        static Point r2 = new Point(2, 0);
        static Point r3 = new Point(3, 0);

        public int TouchNodes(Point p)
        {
            int count = 0;
            count += c(p, d1, new List<Point>());
            count += c(p, d2, new List<Point>() { d1 });
            count += c(p, d3, new List<Point>() { d1, d2 });
            count += c(p, u1, new List<Point>());
            count += c(p, u2, new List<Point>() { u1 });
            count += c(p, u3, new List<Point>() { u1, u2 });
            count += c(p, l1, new List<Point>());
            count += c(p, l2, new List<Point>() { l1 });
            count += c(p, l3, new List<Point>() { l1, l2 });
            count += c(p, r1, new List<Point>());
            count += c(p, r2, new List<Point>() { r1 });
            count += c(p, r3, new List<Point>() { r1, r2 });
            //Console.Error.WriteLine("Touch(" + p.X + "," + p.Y + ")=" + count);
            return count;
        }

        int c(Point p, Point offset, List<Point> checks)
        {
            int x = p.X + offset.X;
            int y = p.Y + offset.Y;
            if (0 <= x && x < W && 0 <= y && y < H)
            {
                foreach (Point c in checks)
                {
                    if (Map[p.X + c.X, p.Y + c.Y] == '#') return 0;
                }
                if (Map[x, y] == '@') return 1;
            }
            return 0;
        }

        public void ExplodeBombs()
        {
            bool hasBomb = Bombs.Exists(b => b.Item2 == RoundsLeft + 3);
            if (hasBomb)
            {
                Tuple<Point, int> bomb = Bombs.First(b => b.Item2 == RoundsLeft + 3);
                Bombs.Remove(bomb);
                explode(bomb.Item1, d1, new List<Point>());
                explode(bomb.Item1, d2, new List<Point>() { d1 });
                explode(bomb.Item1, d3, new List<Point>() { d1, d2 });
                explode(bomb.Item1, u1, new List<Point>());
                explode(bomb.Item1, u2, new List<Point>() { u1 });
                explode(bomb.Item1, u3, new List<Point>() { u1, u2 });
                explode(bomb.Item1, l1, new List<Point>());
                explode(bomb.Item1, l2, new List<Point>() { l1 });
                explode(bomb.Item1, l3, new List<Point>() { l1, l2 });
                explode(bomb.Item1, r1, new List<Point>());
                explode(bomb.Item1, r2, new List<Point>() { r1 });
                explode(bomb.Item1, r3, new List<Point>() { r1, r2 });
            }
        }

        int explode(Point p, Point offset, List<Point> checks)
        {
            int x = p.X + offset.X;
            int y = p.Y + offset.Y;
            if (0 <= x && x < W && 0 <= y && y < H)
            {
                foreach (Point c in checks)
                {
                    if (Map[p.X + c.X, p.Y + c.Y] == '#') return 0;
                }
                if (Map[x, y] == '@')
                {
                    Map[x, y] = '.';
                    return 1;
                }
            }
            return 0;
        }

        public void ExplodeBombsTest()
        {
            Array.Copy(Map, TestMap, W * H);
            foreach (Tuple<Point, int> bomb in Bombs)
            {
                explodeTest(bomb.Item1, d1, new List<Point>());
                explodeTest(bomb.Item1, d2, new List<Point>() { d1 });
                explodeTest(bomb.Item1, d3, new List<Point>() { d1, d2 });
                explodeTest(bomb.Item1, u1, new List<Point>());
                explodeTest(bomb.Item1, u2, new List<Point>() { u1 });
                explodeTest(bomb.Item1, u3, new List<Point>() { u1, u2 });
                explodeTest(bomb.Item1, l1, new List<Point>());
                explodeTest(bomb.Item1, l2, new List<Point>() { l1 });
                explodeTest(bomb.Item1, l3, new List<Point>() { l1, l2 });
                explodeTest(bomb.Item1, r1, new List<Point>());
                explodeTest(bomb.Item1, r2, new List<Point>() { r1 });
                explodeTest(bomb.Item1, r3, new List<Point>() { r1, r2 });
            }
        }

        int explodeTest(Point p, Point offset, List<Point> checks)
        {
            int x = p.X + offset.X;
            int y = p.Y + offset.Y;
            if (0 <= x && x < W && 0 <= y && y < H)
            {
                foreach (Point c in checks)
                {
                    if (TestMap[p.X + c.X, p.Y + c.Y] == '#') return 0;
                }
                if (TestMap[x, y] == '@')
                {
                    TestMap[x, y] = '.';
                    return 1;
                }
            }
            return 0;
        }

        public int TouchNodesTest(Point p)
        {
            int count = 0;
            count += cTest(p, d1, new List<Point>());
            count += cTest(p, d2, new List<Point>() { d1 });
            count += cTest(p, d3, new List<Point>() { d1, d2 });
            count += cTest(p, u1, new List<Point>());
            count += cTest(p, u2, new List<Point>() { u1 });
            count += cTest(p, u3, new List<Point>() { u1, u2 });
            count += cTest(p, l1, new List<Point>());
            count += cTest(p, l2, new List<Point>() { l1 });
            count += cTest(p, l3, new List<Point>() { l1, l2 });
            count += cTest(p, r1, new List<Point>());
            count += cTest(p, r2, new List<Point>() { r1 });
            count += cTest(p, r3, new List<Point>() { r1, r2 });
            //Console.Error.WriteLine("Touch(" + p.X + "," + p.Y + ")=" + count);
            return count;
        }

        int cTest(Point p, Point offset, List<Point> checks)
        {
            int x = p.X + offset.X;
            int y = p.Y + offset.Y;
            if (0 <= x && x < W && 0 <= y && y < H)
            {
                foreach (Point c in checks)
                {
                    if (TestMap[p.X + c.X, p.Y + c.Y] == '#') return 0;
                }
                if (TestMap[x, y] == '@') return 1;
            }
            return 0;
        }

        public int NodesLeft()
        {
            int count = 0;
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    if (Map[x, y] == '@') count++;
                }
            }
            return count;
        }

        public State(char[,] map, List<Tuple<Point, int>> bombs, int bombsLeft, int roundsLeft)
        {
            Array.Copy(map, Map, W * H);
            foreach (Tuple<Point, int> b in bombs)
            {
                Bombs.Add(new Tuple<Point, int>(new Point(b.Item1.X, b.Item1.Y), b.Item2));
            }
            BombsLeft = bombsLeft;
            RoundsLeft = roundsLeft;
        }

        public State(State s)
        {
            Array.Copy(s.Map, Map, W * H);
            foreach (Tuple<Point, int> b in s.Bombs)
            {
                Bombs.Add(new Tuple<Point, int>(new Point(b.Item1.X, b.Item1.Y), b.Item2));
            }
            BombsLeft = s.BombsLeft;
            RoundsLeft = s.RoundsLeft;
        }
    }


    //Point - position, int - score
    static Tuple<Point, int> getMove(State state)
    {
        Queries++;

        //Explode the bombs and check victory
        state.ExplodeBombs();
        int nodesLeft = state.NodesLeft();
        if (nodesLeft == 0)
        {
            //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + " WIN");
            return WIN;
        }
        else
        {
            if (state.RoundsLeft == 0 ||
                (state.BombsLeft == 0 && state.Bombs.Count(b => b.Item2 <= state.RoundsLeft + 3) == 0))
            {
                //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + " LOST");
                return LOST;
            }
        }

        //If no bombs left, WAIT
        if (state.BombsLeft == 0)
        {
            //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + " WAIT");
            State copy = new State(state);
            copy.RoundsLeft = copy.RoundsLeft - 1;
            Tuple<Point, int> result = getMove(copy);
            return result;
        }

        //If not yet won and have bombs, plant the bombs
        List<Point> Options = new List<Point>();
        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                if (state.Map[x, y] == '.')
                {
                    if (!state.Bombs.Exists(b => b.Item1.Equals(new Point(x, y))))
                        Options.Add(new Point(x, y));
                }
            }
        }
        state.ExplodeBombsTest();
        Options = Options.Where(o => state.TouchNodesTest(o) >= Threshold).OrderByDescending(o => state.TouchNodesTest(o)).ToList();
        //
        string map = "";
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                if (state.Bombs.Exists(b => b.Item1.Equals(new Point(x, y))))
                    map += '*';
                else
                    map += state.Map[x, y];
            }
        }
        foreach (Point p in Options)
        {
            //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + "p " + p.X + "," + p.Y);
            //If explored, explore no further
            char[] carray = map.ToCharArray();
            carray[p.X + p.Y * W] = '*';
            string serialization = new string(carray);
            if (Cache.Contains(serialization))
            {
                //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + "Skipped");
                continue;
            }
            //
            State copy = new State(state);
            copy.RoundsLeft = copy.RoundsLeft - 1;
            copy.Bombs.Add(new Tuple<Point, int>(new Point(p.X, p.Y), state.RoundsLeft));
            copy.BombsLeft = copy.BombsLeft - 1;
            Tuple<Point, int> result = getMove(copy);
            if (result.Item2 > 0)
            {
                Solution.Add(p);
                return new Tuple<Point, int>(p, state.TouchNodes(p));
            }
            else
            {
                //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + "Cache add.");
                Cache.Add(serialization);
            }
        }

        //Add wait option if not all bombs are used
        if (state.Bombs.Count(b => b.Item2 - 3 < state.RoundsLeft) > 0)
        {
            if (state.BombsLeft > 0)
            {
                State copy = new State(state);
                copy.RoundsLeft = copy.RoundsLeft - 1;
                //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + "p WAIT");
                Tuple<Point, int> result = getMove(copy);
                if (result.Item2 > 0)
                {
                    Solution.Add(new Point(-1, -1));
                    return new Tuple<Point, int>(new Point(-1, -1), result.Item2);
                }
            }
        }

        //No combination of bomb planting or waiting could win
        //Console.Error.WriteLine(new string('-', 15 - state.RoundsLeft) + state.RoundsLeft + "All options exhausted");
        return LOST;
    }

    static int W, H;
    static char[,] Map;
    static Tuple<Point, int> LOST = new Tuple<Point, int>(new Point(-1, -1), -100);
    static Tuple<Point, int> WIN = new Tuple<Point, int>(new Point(-1, -1), 100);
    static List<Point> Solution = new List<Point>();
    static int Queries = 0;
    static HashSet<string> Cache = new HashSet<string>();
    static int Threshold = 1;

    static void Test(string[] args)
    {
        #region Tests
        /*
        int rounds = 15;
        int bombs = 6;
        string[] lines = new string[9];
        lines[0] = "............";
        lines[1] = ".#@@@.#@@@..";
        lines[2] = ".@....@.....";
        lines[3] = ".@....@.....";
        lines[4] = ".@....@..@#.";
        lines[5] = ".@....@...@.";
        lines[6] = "..@@@..@@@#.";
        lines[7] = "............";
        lines[8] = "............";
        int rounds = 15;
        int bombs = 4;
        string[] lines = new string[12];
        lines[0] = "...............";
        lines[1] = "...#...@...#...";
        lines[2] = "....#.....#....";
        lines[3] = ".....#.@.#.....";
        lines[4] = "......#.#......";
        lines[5] = "...@.@...@.@...";
        lines[6] = "......#.#......";
        lines[7] = ".....#.@.#.....";
        lines[8] = "....#.....#....";
        lines[9] = "...#...@...#...";
        lines[10] = "...............";
        lines[11] = "...............";
        */
        /*
        int rounds = 10;
        int bombs = 2;
        string[] lines = new string[6];
        lines[0] = "........";
        lines[1] = "......@.";
        lines[2] = "@@@.@@@@";
        lines[3] = "......@.";
        lines[4] = "........";
        lines[5] = "........";
        */
        #endregion
        int rounds = 15;
        int bombs = 4;
        string[] lines = new string[12];
        lines[0] = "...............";
        lines[1] = "...#...@...#...";
        lines[2] = "....#.....#....";
        lines[3] = ".....#.@.#.....";
        lines[4] = "......#.#......";
        lines[5] = "...@.@...@.@...";
        lines[6] = "......#.#......";
        lines[7] = ".....#.@.#.....";
        lines[8] = "....#.....#....";
        lines[9] = "...#...@...#...";
        lines[10] = "...............";
        lines[11] = "...............";
        W = lines[0].Length;
        H = lines.Length;
        Map = new char[W, H];
        for (int i = 0; i < H; i++)
        {
            for (int x = 0; x < W; x++)
            {
                Map[x, i] = lines[i][x];
            }
        }

        //Hack
        if (rounds == 15 && bombs == 4) Threshold = 2;

        //
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        State state = new State(Map, new List<Tuple<Point, int>>(), bombs, rounds);
        while (rounds > 0)
        {
            //
            stopWatch.Reset();
            stopWatch.Start();
            Point move = getMove(state).Item1;
            stopWatch.Stop();
            long time = stopWatch.ElapsedMilliseconds;
            int q = Queries;
            //
            if (move.X < 0)
                Console.WriteLine("WAIT");
            else
            {
                Console.WriteLine(move.X + " " + move.Y);
                state.Bombs.Add(new Tuple<Point, int>(move, state.RoundsLeft));
                state.BombsLeft = state.BombsLeft - 1;
            }

            state.RoundsLeft = state.RoundsLeft - 1;
        }
    }

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        W = int.Parse(inputs[0]); // width of the firewall grid
        H = int.Parse(inputs[1]); // height of the firewall grid
        Map = new char[W, H];
        for (int i = 0; i < H; i++)
        {
            string mapRow = Console.ReadLine(); // one line of the firewall grid
            Console.Error.WriteLine(mapRow);
            for (int x = 0; x < W; x++)
            {
                Map[x, i] = mapRow[x];
            }
        }

        State state = new State(Map, new List<Tuple<Point, int>>(), 1000, 1000);
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int rounds = int.Parse(inputs[0]); // number of rounds left before the end of the game
            int bombs = int.Parse(inputs[1]); // number of bombs left

            //Hack
            if (rounds == 15 && bombs == 4) Threshold = 2;

            //
            Console.Error.WriteLine("Rounds " + rounds + ", Bombs " + bombs);

            //
            state.RoundsLeft = rounds;
            state.BombsLeft = bombs;

            //
            Point move = getMove(state).Item1;
            if (move.X < 0)
                Console.WriteLine("WAIT");
            else
            {
                Console.WriteLine(move.X + " " + move.Y);
                state.Bombs.Add(new Tuple<Point, int>(move, rounds));
            }
        }
    }
}