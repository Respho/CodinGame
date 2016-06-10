using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

class Player
{
    const int WIDTH = 30, HEIGHT = 20;

    public enum Turn { Front, Right, Left }

    public enum Direction { UP, RIGHT, DOWN, LEFT }

    static Direction GetDirection(Point from, Point to)
    {
        if (to.Y < from.Y) return Direction.UP;
        if (to.Y > from.Y) return Direction.DOWN;
        if (to.X < from.X) return Direction.LEFT;
        return Direction.RIGHT;
    }

    static int manhattan(Point from, int x, int y)
    {
        return Math.Abs(x - from.X) + Math.Abs(y - from.Y);
    }

    public class State
    {
        public char[,] Map = new char[WIDTH, HEIGHT];
        public Point[] Players = new Point[4];
        public Direction[] Directions = new Direction[4];

        public const char BLANK = (char)99;

        public State()
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    Map[i, j] = BLANK;
                }
            }
        }

        //
        public Point GetPoint()
        {
            Direction currentDir = Directions[PlayerNumber];
            Point current = Players[PlayerNumber];
            switch (currentDir)
            {
                case Direction.UP:
                    current = new Point(current.X, current.Y - 1);
                    break;
                case Direction.DOWN:
                    current = new Point(current.X, current.Y + 1);
                    break;
                case Direction.LEFT:
                    current = new Point(current.X - 1, current.Y);
                    break;
                case Direction.RIGHT:
                    current = new Point(current.X + 1, current.Y);
                    break;
            }
            //
            if (current.X < 0 || current.X == WIDTH || current.Y < 0 || current.Y == HEIGHT) return new Point(-1, -1);
            if (Map[current.X, current.Y] != BLANK) return new Point(-1, -1);
            //
            return current;
        }
        //
        public Point GetAnyPoint()
        {
            Point current = Players[PlayerNumber];
            if (isValid(new Point(current.X + 1, current.Y))) return new Point(current.X + 1, current.Y);
            if (isValid(new Point(current.X - 1, current.Y))) return new Point(current.X - 1, current.Y);
            if (isValid(new Point(current.X, current.Y + 1))) return new Point(current.X, current.Y + 1);
            if (isValid(new Point(current.X, current.Y - 1))) return new Point(current.X, current.Y - 1);
            return new Point();
        }
        //
        bool isValid(Point p)
        {
            return (p.X >= 0 && p.X < WIDTH && p.Y >= 0 && p.Y < HEIGHT) && Map[p.X, p.Y] == BLANK;
        }
    }

    static State CurrentState = new State();
    static int PlayerCount;
    static int PlayerNumber;

    static void Update(int p, Point p0, Point p1)
    {
        //Clear
        if (p0.X == -1)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (CurrentState.Map[i, j] == p)
                        CurrentState.Map[i, j] = State.BLANK;
                }
            }
            CurrentState.Players[p] = p0;
            return;
        }
        CurrentState.Map[p0.X, p0.Y] = (char)p;
        CurrentState.Map[p1.X, p1.Y] = (char)p;
        CurrentState.Players[p] = p1;
    }

    static void PlayMove()
    {
        //Take point in direction
        Point p = CurrentState.GetPoint();
        //Take any point
        if (p.X == -1)
        {
            p = CurrentState.GetAnyPoint();
        }
        Console.Error.Write(p.X + "," + p.Y);
        Direction dir = GetDirection(CurrentState.Players[PlayerNumber], p);
        CurrentState.Directions[PlayerNumber] = dir;
        Console.WriteLine(dir.ToString());
    }

    static void Main(string[] args)
    {
        string[] inputs;

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int N = int.Parse(inputs[0]); // total number of players (2 to 4).
            int P = int.Parse(inputs[1]); // your player number (0 to 3).
            PlayerCount = N; PlayerNumber = P;
            for (int i = 0; i < N; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int x0 = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                int y0 = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                int x1 = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                int y1 = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
                //
                Update(i, new Point(x0, y0), new Point(x1, y1));
            }

            PlayMove();
        }
    }
}

