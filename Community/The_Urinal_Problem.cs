using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        string B = Console.ReadLine();

        int max = 0; int index = 0;
        for (int i = 0; i < B.Length; i++)
        {
            if (B[i] == '!') continue;
            int mind = 100;
            for (int x = 0; x < B.Length; x++)
            {
                if (B[x] == 'U') continue;
                int d = Math.Abs(x - i);
                mind = Math.Min(d, mind);
            }
            Console.Error.WriteLine("p " + i + " mind " + mind);
            if (mind > max)
            {
                index = i;
                max = mind;
            }
        }

        Console.Error.WriteLine("max " + max);
        Console.WriteLine(index);
    }
}