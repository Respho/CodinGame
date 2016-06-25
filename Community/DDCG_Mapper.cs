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
        int L = int.Parse(Console.ReadLine());
        int N = int.Parse(Console.ReadLine());
        List<Tuple<string, int>> Patterns = new List<Tuple<string, int>>();
        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            string pattern = inputs[0];
            int tempo = int.Parse(inputs[1]);
            Patterns.Add(new Tuple<string, int>(pattern, tempo));
        }

        for (int row = L; row >= 1; row--)
        {
            string line = new string('0', Patterns[0].Item1.Length);
            foreach (Tuple<string, int> pattern in Patterns)
            {
                if (row % pattern.Item2 == 0)
                {
                    //
                    string result = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == 'X' || pattern.Item1[i] == 'X')
                            result += "X";
                        else
                            result += "0";
                    }
                    line = result;
                }
            }
            Console.WriteLine(line);
        }
    }
}