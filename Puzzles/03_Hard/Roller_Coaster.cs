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
        string[] inputs = Console.ReadLine().Split(' ');
        int L = int.Parse(inputs[0]);
        int C = int.Parse(inputs[1]);
        int N = int.Parse(inputs[2]);
        int[] Groups = new int[N];
        int ppl = 0;
        for (int i = 0; i < N; i++)
        {
            int pi = int.Parse(Console.ReadLine());
            Groups[i] = pi;
            ppl += pi;
        }
        
        Dictionary<int, Tuple<int, int>> Mem = new Dictionary<int, Tuple<int, int>>();

        long total = 0;
        int pointer = 0;
        int rounds = 0;
        while (rounds < C)
        {
            //
            int cap = L;
            if (cap >= ppl)
            {
                total += ppl;
            }
            else if (Mem.ContainsKey(pointer))
            {
                Tuple<int, int> r = Mem[pointer];
                total += r.Item1;
                pointer = r.Item2;
            }
            else
            {
                int p = pointer; int c = 0;
                while (cap >= Groups[pointer])
                {
                    cap -= Groups[pointer];
                    c += Groups[pointer];
                    total += Groups[pointer];
                    pointer++;
                    if (pointer == N) pointer = 0;
                }
                Mem.Add(p, new Tuple<int, int>(c, pointer));
            }
            //
            rounds++;
        }

        Console.WriteLine(total);
    }
}