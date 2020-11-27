using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static List<int> widths = new List<int>();
    static List<int> heights = new List<int>();

    static Dictionary<int, int> getIntervalCounts(List<int> list)
    {
        var intervals = new Dictionary<int, int>();
        int count = list.Count();
        for (int stepSize = 1; stepSize < count; stepSize++)
        {
            for (int startIndex = 0; startIndex + stepSize < count; startIndex++)
            {
                int endIndex = startIndex + stepSize;
                int interval = list[endIndex] - list[startIndex];
                if (intervals.ContainsKey(interval))
                    intervals[interval]++;
                else
                    intervals[interval] = 1;
            }
        }
        //
        return intervals;
    }

    static int countSquares()
    {
        var intervalCountsW = getIntervalCounts(widths);
        var intervalCountsH = getIntervalCounts(heights);

        int total = 0;
        foreach (int interval in intervalCountsW.Keys)
        {
            total += intervalCountsW[interval] * (intervalCountsH.ContainsKey(interval) ? intervalCountsH[interval] : 0);
        }
        return total;
    }

    static int calc(string line1, string line2, string line3)
    {
        string[] inputs;
        inputs = line1.Split(' ');
        int w = int.Parse(inputs[0]);
        int h = int.Parse(inputs[1]);
        int countX = int.Parse(inputs[2]);
        int countY = int.Parse(inputs[3]);

        //Width markers
        inputs = line2.Split(' ');
        widths.Add(0);
        for (int i = 0; i < countX; i++)
        {
            int x = int.Parse(inputs[i]);
            widths.Add(x);
        }
        widths.Add(w);

        //Height markers
        inputs = line3.Split(' ');
        heights.Add(0);
        for (int i = 0; i < countY; i++)
        {
            int y = int.Parse(inputs[i]);
            heights.Add(y);
        }
        heights.Add(h);

        return countSquares();
    }

    static void Main()
    {
        string line1 = Console.ReadLine();
        Console.Error.WriteLine(line1);
        string line2 = Console.ReadLine();
        Console.Error.WriteLine(line2);
        string line3 = Console.ReadLine();
        Console.Error.WriteLine(line3);
        //
        Console.WriteLine(calc(line1, line2, line3));
    }
}
