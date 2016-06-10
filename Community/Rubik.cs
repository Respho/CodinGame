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

        int cube = N * N * N;
        if (N > 2) cube -= (N - 2) * (N - 2) * (N - 2);

        Console.WriteLine(cube);
    }
}