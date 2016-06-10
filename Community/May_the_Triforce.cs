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

        string result = "";
        //Phase1
        for (int i = 0; i < N; i++)
        {
            result += new string(' ', N - i + N - 1);
            result += new string('*', i * 2 + 1);
            result += "|";
        }
        //Phase2
        for (int i = 0; i < N; i++)
        {
            result += new string(' ', N - i - 1);
            result += new string('*', i * 2 + 1);
            result += new string(' ', (N - i) * 2 - 1);
            result += new string('*', i * 2 + 1);
            result += "|";
        }

        //Output
        string[] lines = result.Split('|');
        char[] ca = lines[0].ToCharArray();
        ca[0] = '.';
        lines[0] = new string(ca);
        
        foreach (string l in lines)
        {
            if (l.Trim().Length > 0)
            Console.WriteLine(l);
        }
    }
}