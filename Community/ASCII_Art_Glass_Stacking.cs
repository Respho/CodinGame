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
        int INPUT = int.Parse(Console.ReadLine());
        int N = 0;
        while (INPUT > N)
        {
            N++; INPUT -= N;
        }

        for (int i = 1; i <= N; i++)
        {
            string line = "";
            line = new string(' ', (N - i) * 3);
            for (int x = 1; x <= i; x++)
            {
                line += " ***  ";
            }
            line = line.TrimEnd(' ');
            line += new string(' ', (N * 6 - 1) - line.Length);
            Console.WriteLine(line);
            line = new string(' ', (N - i) * 3);
            for (int x = 1; x <= i; x++)
            {
                line += " * *  ";
            }
            line = line.TrimEnd(' ');
            line += new string(' ', (N * 6 - 1) - line.Length);
            Console.WriteLine(line);
            line = new string(' ', (N - i) * 3);
            for (int x = 1; x <= i; x++)
            {
                line += " * *  ";
            }
            line = line.TrimEnd(' ');
            line += new string(' ', (N * 6 - 1) - line.Length);
            Console.WriteLine(line);
            line = new string(' ', (N - i) * 3);
            for (int x = 1; x <= i; x++)
            {
                line += "***** ";
            }
             line = line.TrimEnd(' ');
            line += new string(' ', (N * 6 - 1) - line.Length);
           Console.WriteLine(line);
        }

    }
}