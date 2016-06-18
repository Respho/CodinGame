using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class CodinGame
{
    //
    static int W, H;
    static bool[,] Bitmap;
    //
    static List<List<int>> hBuckets = null, vBuckets = null;
    static int[] vIntensities = null;
    //
    struct Line { public int start, end; };

    static List<List<int>> getBuckets(List<int> list)
    {
        List<List<int>> buckets = new List<List<int>>();
        int temp = -10000;
        List<int> tempBucket = null;
        foreach (int i in list)
        {
            if (i - temp > 1)
            {
                tempBucket = new List<int>();
                buckets.Add(tempBucket);
            }
            tempBucket.Add(i);
            temp = i;
        }
        return buckets;
    }

    static void init(int w, int h, string rle)
    {
        W = w; H = h;
        //white = false;
        Bitmap = new bool[W, H];
        int x = 0, y = 0; bool isWhite = true;
        string[] tokens = rle.Split(' ');
        foreach (string t in tokens)
        {
            if (t.Equals("W") || t.Equals("B")) continue;
            //
            int num = int.Parse(t);
            for (int i = 0; i < num; i++)
            {
                if (!isWhite) Bitmap[x, y] = true;
                x++;
                if (x >= W)
                {
                    x = 0; y++;
                }
            }
            isWhite = !isWhite;
        }

        //
        List<int> hlines = new List<int>();
        for (int j = 0; j < H; j++)
        {
            int temp = 0;
            for (int i = 0; i < W; i++)
            {
                if (Bitmap[i, j]) temp++;
            }
            if (temp > (W * 3 / 4)) hlines.Add(j);
        }
        List<int> vlines = new List<int>();
        vIntensities = new int[W];
        for (int i = 0; i < W; i++)
        {
            int temp = 0;
            for (int j = 0; j < H; j++)
            {
                if (Bitmap[i, j]) temp++;
            }
            vIntensities[i] = temp;
            if (temp > (H / 4)) vlines.Add(i);
        }

        //Clear lines
        foreach (int j in hlines)
        {
            for (int i = 0; i < W; i++)
            {
                Bitmap[i, j] = false;
            }
        }

        //
        hBuckets = getBuckets(hlines);
        vBuckets = getBuckets(vlines);
    }

    static Line findLongestX(int x)
    {
        int max = 0, maxStart = 0;
        int l = 0, start = -1;
        for (int j = 0; j < H; j++)
        {
            if (Bitmap[x, j])
            {
                if (start == -1) start = j;
                l++;
            }
            else
            {
                if (l > max)
                {
                    max = l; maxStart = start;
                }
                start = -1; l = 0;
            }
        }
        return new Line() { start = maxStart, end = maxStart + max };
    }

    static double surveyBox(int x, int y, int size)
    {
        int count = 0;
        string debug = "";
        for (int j = y; j < y + size; j++)
        {
            for (int i = x; i < x + size; i++)
            {
                if (Bitmap[i, j]) count++;
                debug += Bitmap[i, j] ? "0" : ".";
            }
            debug += "\n\r";
        }

        //Console.WriteLine("" + x + "," + y + " " + size + " " + 1.0 * count / (size * size) + "\n\r");
        //Console.WriteLine(debug);
        return 1.0 * count / (size * size);
    }

    static string getPitch(int x)
    {
        double firstLine = hBuckets.First().Average();
        double lastLine = hBuckets.Last().Average();
        double interval = (lastLine - firstLine) / 8;
        int boxSize = Convert.ToInt32(Math.Floor(interval * 2) - 1);

        //C is 0
        double max = 0.0; int pitch = -1;
        double c = lastLine + (interval * 2);
        for (int i = 11; i >= 0; i--)
        {
            double p = c - (interval * (i + 1));
            int y = Convert.ToInt32(Math.Ceiling(p));
            double coverage = surveyBox(x, y, boxSize);
            if (coverage > max)
            {
                max = coverage; pitch = i;
            }
        }
        string pitchName = "" + "CDEFGABCDEFG"[pitch];
        pitchName += max > 0.4 ? "Q" : "H";
        //
        return pitchName;
    }

    static void Process(int W, int H, string rle)
    {
        //
        init(W, H, rle);

        //
        double firstLine = hBuckets.First().Average();
        double lastLine = hBuckets.Last().Average();
        double interval = (lastLine - firstLine) / 8;
        int boxSize = Convert.ToInt32(Math.Floor(interval * 2) - 1);

        //
        string result = "";
        foreach (List<int> b in vBuckets)
        {
            int l = b.Min() - 1;
            int r = b.Max() + 1;
            if (vIntensities[l] > vIntensities[r])
            {
                result += getPitch(l - boxSize);
            }
            else
            {
                result += getPitch(r);
            }

            result += " ";
        }

        Console.WriteLine(result.Trim());
    }

    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int w = int.Parse(inputs[0]);
        int h = int.Parse(inputs[1]);
        string IMAGE = Console.ReadLine();

        Process(w, h, IMAGE);
    }
}
