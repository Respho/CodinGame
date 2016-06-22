using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

class Player
{
    //Max number of distance calculations
    const int MaxLoad = 100000;
    //
    static int W, H, N, ScaleFactor, SearchX0, SearchX1, SearchY0, SearchY1;
    static List<Point> Points = new List<Point>();
    static bool[,] Board = null;
    static bool[,] DetailedBoard = null;

    static void Next(string input)
    {
        //Initialization and scaling down
        if (Board == null) init();

        //In the first move, jump to the opposite side
        if (input.Equals("UNKNOWN"))
        {
            //moveTo(W - Points.Last().X - 1, H - Points.Last().Y - 1);
            moveTo(W / 2, H / 2);
            return;
        }

        //Update the board and jump to the new centroid
        updateBoard(input);
        Point p = updateCentroid();
        //Overshoot the centroid a bit to gain more area
        Point target = getTarget(p);
        //moveTo(p.X, p.Y);
        moveTo(target.X, target.Y);
    }

    static void updateBoard(string input)
    {
        Point p1 = Points[Points.Count() - 1];
        Point p2 = Points[Points.Count() - 2];
        if (ScaleFactor > 1)
        {
            p1 = new Point(p1.X / ScaleFactor, p1.Y / ScaleFactor);
            p2 = new Point(p2.X / ScaleFactor, p2.Y / ScaleFactor);
        }
        //
        if (input.Equals("WARMER"))
        {
            for (int i = SearchX0; i < SearchX1; i++)
            {
                for (int j = SearchY0; j < SearchY1; j++)
                {
                    if (Board[i, j])
                    {
                        int d1x = p1.X - i; int d1y = p1.Y - j;
                        int d1 = d1x * d1x + d1y * d1y;
                        int d2x = p2.X - i; int d2y = p2.Y - j;
                        int d2 = d2x * d2x + d2y * d2y;
                        if (d1 > d2) Board[i, j] = false;
                    }
                }
            }
        }
        else if (input.Equals("COLDER"))
        {
            for (int i = SearchX0; i < SearchX1; i++)
            {
                for (int j = SearchY0; j < SearchY1; j++)
                {
                    if (Board[i, j])
                    {
                        int d1x = p1.X - i; int d1y = p1.Y - j;
                        int d1 = d1x * d1x + d1y * d1y;
                        int d2x = p2.X - i; int d2y = p2.Y - j;
                        int d2 = d2x * d2x + d2y * d2y;
                        if (d1 < d2) Board[i, j] = false;
                    }
                }
            }
        }
        else if (input.Equals("SAME"))
        {
            for (int i = SearchX0; i < SearchX1; i++)
            {
                for (int j = SearchY0; j < SearchY1; j++)
                {
                    if (Board[i, j])
                    {
                        int d1x = p1.X - i; int d1y = p1.Y - j;
                        int d1 = d1x * d1x + d1y * d1y;
                        int d2x = p2.X - i; int d2y = p2.Y - j;
                        int d2 = d2x * d2x + d2y * d2y;
                        if (d1 != d2) Board[i, j] = false;
                    }
                }
            }
        }
    }

    static Point updateCentroid()
    {
        //Find the centroid
        Console.Error.WriteLine("Centroid Search " + SearchX0 + "-" + SearchX1 + ", " + SearchY0 + "-" + SearchY1);
        double x = 0; double y = 0;
        int counter = 0;
        int sx0 = W, sx1 = 0, sy0 = H, sy1 = 0;
        for (int i = SearchX0; i < SearchX1; i++)
        {
            for (int j = SearchY0; j < SearchY1; j++)
            {
                if (Board[i, j])
                {
                    if (i < sx0) sx0 = i;
                    if (i > sx1) sx1 = i;
                    if (j < sy0) sy0 = j;
                    if (j > sy1) sy1 = j;
                    x += i; y += j;
                    counter++;
                }
            }
        }
        //
        x = x * ScaleFactor / counter;
        y = y * ScaleFactor / counter;
        SearchX0 = sx0; SearchX1 = sx1 + 1;
        SearchY0 = sy0; SearchY1 = sy1 + 1;
        Console.Error.WriteLine("Updated Search " + SearchX0 + "-" + SearchX1 + ", " + SearchY0 + "-" + SearchY1 + " Counter=" + counter);

        //Remap
        if (ScaleFactor > 1)
        {
            if (counter * ScaleFactor * ScaleFactor < MaxLoad)
            {
                Console.Error.WriteLine("Remapping...");
                int counter1 = 0;
                int counter2 = 0;
                //
                for (int j = 0; j < H / ScaleFactor; j++)
                {
                    for (int i = 0; i < W / ScaleFactor; i++)
                    {
                        if (Board[i, j])
                        {
                            counter1++;
                            for (int m = 0; m < ScaleFactor; m++)
                            {
                                for (int n = 0; n < ScaleFactor; n++)
                                {
                                    DetailedBoard[i * ScaleFactor + m, j * ScaleFactor + n] = true;
                                    counter2++;
                                }
                            }
                        }
                    }
                }
                //
                Board = DetailedBoard;
                SearchX0 *= ScaleFactor; SearchX1 *= ScaleFactor;
                SearchY0 *= ScaleFactor; SearchY1 *= ScaleFactor;
                ScaleFactor = 1;
                Console.Error.WriteLine("Done Remapping " + counter1 + " " + counter2);
                //Add this to have more than 3 moves left
                if (W == 8000) return new Point(0, 0);
            }
        }

        return new Point(Convert.ToInt32(Math.Round(x)), Convert.ToInt32(Math.Round(y)));
    }

    static void init()
    {
        //
        ScaleFactor = 1;
        int load = W * H;
        while (load > MaxLoad)
        {
            ScaleFactor++;
            load = W * H / ScaleFactor / ScaleFactor;
        }
        Board = new bool[W / ScaleFactor, H / ScaleFactor];
        SearchX0 = 0; SearchX1 = W / ScaleFactor; SearchY0 = 0; SearchY1 = H / ScaleFactor;

        //
        for (int i = 0; i < W / ScaleFactor; i++)
        {
            for (int j = 0; j < H / ScaleFactor; j++)
            {
                Board[i, j] = true;
            }
        }

        //
        DetailedBoard = new bool[W, H];
        Console.Error.WriteLine("ScaleFactor " + ScaleFactor);
    }

    static Point getTarget(Point p)
    {
        int diffX = Points[Points.Count() - 1].X - p.X;
        int diffY = Points[Points.Count() - 1].Y - p.Y;
        return new Point(Math.Max(0, Math.Min(W - 1, p.X - diffX / 3)), Math.Max(0, Math.Min(H - 1, p.Y - diffY / 3)));
    }

    static void moveTo(int x, int y)
    {
        //
        if (Points.Exists(p => p.X == x && p.Y == y) && ScaleFactor == 1)
        {
            Console.Error.WriteLine("Point (" + x + "," + y + ") already visited, find another");
            int mind = 100000; Point newP = new Point();
            for (int i = SearchX0; i < SearchX1; i++)
            {
                for (int j = SearchY0; j < SearchY1; j++)
                {
                    if (Board[i, j] && !Points.Exists(p => p.X == i && p.Y == j))
                    {
                        int manhattan = Math.Abs(i - x) + Math.Abs(j - y);
                        if (manhattan <= mind)
                        {
                            mind = manhattan;
                            newP = new Point(i, j);
                        }
                    }
                }
            }
            moveTo(newP.X, newP.Y);
            return;
        }
        //
        Points.Add(new Point(x, y));
        Console.WriteLine(x + " " + y);
    }

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        W = int.Parse(inputs[0]); // width of the building.
        H = int.Parse(inputs[1]); // height of the building.
        N = int.Parse(Console.ReadLine()); // maximum number of turns before game over.
        //
        inputs = Console.ReadLine().Split(' ');
        int X0 = int.Parse(inputs[0]);
        int Y0 = int.Parse(inputs[1]);
        Points.Add(new Point(X0, Y0));
        //
        Console.Error.WriteLine(W + "x" + H + " " + N + " moves (" + X0 + "," + Y0 + ")");

        while (true)
        {
            // Current distance to the bomb compared to previous distance (COLDER, WARMER, SAME or UNKNOWN)
            string bombDir = Console.ReadLine();
            //
            Next(bombDir);
        }
    }
}
