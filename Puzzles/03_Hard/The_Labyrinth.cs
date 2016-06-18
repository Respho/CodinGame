using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

class Player
{
    static int C, R, A;
    static char[,] Map = null;
    static int[,] Dists = null;
    static Point[,] Froms = null;
    static Point Start = new Point();
    static Point Ctrl = new Point();
    static Point Unexplored = new Point();
    static bool IsEscape = false;

    enum Direction { UP, DOWN, LEFT, RIGHT };

    static Direction getDirection(Point from, Point to)
    {
        if (manhattan(from, to) != 1) throw new Exception("getDirection " + from.X + "," + from.Y + " " + to.X + "," + to.Y);
        if (to.Y > from.Y) return Direction.DOWN;
        if (to.Y < from.Y) return Direction.UP;
        if (to.X > from.X) return Direction.RIGHT;
        return Direction.LEFT;
    }

    static Point meets(int x, int y)
    {
        if (Dists[x, y] > 0)
        {
            if (check(x - 1, y)) return new Point(x - 1, y);
            if (check(x + 1, y)) return new Point(x + 1, y);
            if (check(x, y - 1)) return new Point(x, y - 1);
            if (check(x, y + 1)) return new Point(x, y + 1);
        }
        return new Point(0, 0);
    }
    static bool check(int x, int y)
    {
        if (0 <= x && x < C && 0 <= y && y < R && Dists[x, y] < 0)
        {
            return true;
        }
        return false;
    }

    //Returns next point and shortest distance
    static int paint(int x, int y, int round, Point from)
    {
        if (0 <= x && x < C && 0 <= y && y < R &&
            Dists[x, y] == 0 && Map[x, y] == '.')
        {
            Dists[x, y] = round + (round > 0 ? 1 : -1);
            Froms[x, y] = new Point(from.X, from.Y);
            return 1;
        }
        return 0;
    }
    static Tuple<Point, int> findPath(Point from, Point to)
    {
        //
        if (manhattan(from, to) == 1) return new Tuple<Point, int>(to, 1);

        //Clear
        for (int j = 0; j < R; j++)
        {
            for (int i = 0; i < C; i++)
            {
                Dists[i, j] = 0;
                Froms[i, j] = new Point();
            }
        }

        //
        Dists[to.X, to.Y] = 1;
        Dists[from.X, from.Y] = -1;
        Point Meet = new Point();
        int distance = 1000;
        bool found = false;
        int round = 0;
        while (!found)
        {
            round++;
            int countPainted = 0;
            //Iter through cells
            for (int j = 0; j < R; j++)
            {
                for (int i = C - 1; i >= 0; i--)
                {
                    //Paint one step
                    if (Dists[i, j] == round)
                    {
                        countPainted += paint(i - 1, j, round, new Point(i, j));
                        countPainted += paint(i + 1, j, round, new Point(i, j));
                        countPainted += paint(i, j - 1, round, new Point(i, j));
                        countPainted += paint(i, j + 1, round, new Point(i, j));
                    }
                    if (Dists[i, j] == -round)
                    {
                        countPainted += paint(i - 1, j, -round, new Point(i, j));
                        countPainted += paint(i + 1, j, -round, new Point(i, j));
                        countPainted += paint(i, j - 1, -round, new Point(i, j));
                        countPainted += paint(i, j + 1, -round, new Point(i, j));
                    }
                }
            }
            //
            if (countPainted == 0) return new Tuple<Point, int>(new Point(0, 0), 1000);

            //Check
            for (int j = 0; j < R; j++)
            {
                for (int i = C - 1; i >= 0; i--)
                {
                    Meet = meets(i, j);
                    if (Meet.X > 0 && Meet.Y > 0)
                    {
                        distance = -Dists[Meet.X, Meet.Y] + Dists[i, j] - 1;
                        found = true;
                        if (from.Equals(Meet)) return new Tuple<Point, int>(new Point(i, j), distance);
                        break;
                    }
                }
                if (found) break;
            }
        }

        //Trace back to origin
        while (true)
        {
            Point p = Froms[Meet.X, Meet.Y];
            if (p.Equals(from)) break;
            Meet = new Point(p.X, p.Y);
        }

        return new Tuple<Point, int>(Meet, distance);
    }

    static Point unexplored(Point from)
    {
        List<Point> points = new List<Point>();
        for (int j = 0; j < R; j++)
        {
            for (int i = C - 1; i >= 0; i--)
            {
                if (Map[i, j] == '?')
                {
                    if (unexploredNeighbour(i - 1, j)) points.Add(new Point(i - 1, j));
                    if (unexploredNeighbour(i + 1, j)) points.Add(new Point(i + 1, j));
                    if (unexploredNeighbour(i, j - 1)) points.Add(new Point(i, j - 1));
                    if (unexploredNeighbour(i, j + 1)) points.Add(new Point(i, j + 1));
                }
            }
        }

        points = points.OrderBy(p => manhattan(from, p)).ToList();
        foreach (Point p in points)
        {
            Tuple<Point, int> r = findPath(from, p);
            if (r.Item2 < 1000) return p;
        }

        throw new Exception("No unexplored points");
    }
    static bool unexploredNeighbour(int x, int y)
    {
        if (0 <= x && x < C && 0 <= y && y < R && Map[x, y] == '.')
        {
            return true;
        }
        return false;
    }

    static int manhattan(Point from, Point to)
    {
        return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
    }

    static Direction getStep(Point Current)
    {
        //Find the control room and the shortest path back
        if ((Ctrl.X == 0) || (findPath(Ctrl, Start).Item2 > A))
        {
            if (Unexplored.X == 0 || Current.Equals(Unexplored))
            {
                Unexplored = unexplored(Current);
            }
            Console.Error.WriteLine("Unexplored " + Unexplored.X + "," + Unexplored.Y);
            Point next = findPath(Current, Unexplored).Item1;
            return getDirection(Current, next);
        }

        //Going for the Ctrl
        if (!IsEscape)
        {
            Point next = findPath(Current, Ctrl).Item1;
            if (Ctrl.Equals(next)) IsEscape = true;
            return getDirection(Current, next);
        }

        //Escaping
        Point step = findPath(Current, Start).Item1;
        return getDirection(Current, step);
    }

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        R = int.Parse(inputs[0]); // number of rows.
        C = int.Parse(inputs[1]); // number of columns.
        A = int.Parse(inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.
        Console.Error.WriteLine("Start " + C + "," + R + " " + A + " rounds");

        Map = new char[C, R];
        Dists = new int[C, R];
        Froms = new Point[C, R];

        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int KR = int.Parse(inputs[0]); // row where Kirk is located.
            int KC = int.Parse(inputs[1]); // column where Kirk is located.
            for (int i = 0; i < R; i++)
            {
                string ROW = Console.ReadLine(); // C of the characters in '#.TC?' (i.e. one line of the ASCII maze).
                //Console.Error.WriteLine(ROW);
                for (int x = 0; x < C; x++)
                {
                    Map[x, i] = ROW[x];
                    if (ROW[x] == 'T')
                    {
                        Start = new Point(x, i);
                        Map[x, i] = '.';
                    }
                    if (ROW[x] == 'C')
                    {
                        Ctrl = new Point(x, i);
                        Map[x, i] = '#';
                    }
                }
            }

            Console.Error.WriteLine("K=" + KC + "," + KR);

            Console.WriteLine(getStep(new Point(KC, KR)));
        }
    }
}