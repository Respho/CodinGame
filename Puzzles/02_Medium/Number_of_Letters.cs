using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static void Main(string[] args)
    {
        string line = Console.ReadLine();
        Console.Error.WriteLine(line);
        //
        string[] inputs = line.Split(' ');
        long start = long.Parse(inputs[0]);
        long n = long.Parse(inputs[1]);

        // Write an answer using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        long index = 0;
        long current = start;
        bool found = false;
        while ((index < n) && !found)
        {
            string binary = Convert.ToString(current, 2);
            int length = binary.Length;
            int ones = binary.Count(c => c == '1');
            int zeroes = binary.Count(c => c == '0');
            int nextTerm = length * 3 + zeroes;
            if (nextTerm == current)
            {
                found = true;
                break;
            }
            index++;
            current = nextTerm;
            Console.Error.WriteLine("S(" + index + ")=" + current);
        }

        Console.WriteLine(current);
    }
}