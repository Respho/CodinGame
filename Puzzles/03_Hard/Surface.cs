using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    class Lake
    {
        public static List<Lake> Lakes = new List<Lake>();
        static int SerialCount = 0;

        public int Serial;
        public HashSet<int> Points;
        public Lake()
        {
            Serial = SerialCount++;
            Points = new HashSet<int>();
        }

        public static int Index(int i, int j)
        {
            return j * L + i;
        }
        public static string UnIndex(int x)
        {
            return (x % L).ToString() + "," + (x / L).ToString();
        }

        public void MergeInto(Lake lake)
        {
            Lakes.Remove(this);
            foreach (int i in this.Points)
            {
                lake.Points.Add(i);
            }
        }
    }

    static void process(char[,] map)
    {
        //
        Lake[] lakesRow = new Lake[L];
        for (int j = 0; j < H; j++)
        {
            for (int i = 0; i < L; i++)
            {
                if (map[i, j] == '#') continue;

                //
                bool leftWater = (i > 0) && map[i - 1, j] == 'O';
                bool topWater = (j > 0) && map[i, j - 1] == 'O';
                //
                if (leftWater && topWater)
                {
                    Lake lakeT = lakesRow[i];
                    Lake lakeL = lakesRow[i - 1];
                    if (lakeT.Serial != lakeL.Serial)
                    {
                        //Merge
                        lakeL.MergeInto(lakeT);
                        //Update
                        for (int x = 0; x < L; x++)
                        {
                            if ((lakesRow[x] != null) && (lakesRow[x].Serial == lakeL.Serial))
                                lakesRow[x] = lakeT;
                        }
                    }
                    lakeT.Points.Add(Lake.Index(i, j));
                    continue;
                }
                if (!leftWater && !topWater)
                {
                    //add
                    Lake lake = new Lake();
                    lake.Points.Add(Lake.Index(i, j));
                    Lake.Lakes.Add(lake);
                    lakesRow[i] = lake;
                    continue;
                }
                if (leftWater)
                {
                    lakesRow[i] = lakesRow[i - 1];
                    lakesRow[i].Points.Add(Lake.Index(i, j));
                }
                if (topWater)
                {
                    lakesRow[i].Points.Add(Lake.Index(i, j));
                }
            }
        }
    }

    static int L, H;

    static void Main(string[] args)
    {
        L = int.Parse(Console.ReadLine());
        H = int.Parse(Console.ReadLine());
        char[,] map = new char[L, H];
        for (int i = 0; i < H; i++)
        {
            string row = Console.ReadLine();
            for (int c = 0; c < L; c++)
            {
                map[c, i] = row[c];
            }
        }

        //
        process(map);

        //
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int X = int.Parse(inputs[0]);
            int Y = int.Parse(inputs[1]);

            bool any = Lake.Lakes.Any(l => l.Points.Contains(Lake.Index(X, Y)));
            if (any) Console.WriteLine(Lake.Lakes.First(l => l.Points.Contains(Lake.Index(X, Y))).Points.Count());
            else Console.WriteLine("0");
        }
    }
}