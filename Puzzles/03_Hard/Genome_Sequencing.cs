using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static string min(string w1, string w2)
    {
        if (w1.Contains(w2)) return w1;
        if (w2.Contains(w1)) return w2;

        //        
        string ws = w1, wl = w2;
        if (w1.Length > w2.Length)
        {
            wl = w1; ws = w2;
        }

        //
        int best = 0; string r = "";
        //      ABXXXXCBA
        //CBAXXXAB
        for (int i = 1; i < ws.Length; i++)
        {
            string wls = wl.Substring(0, i);
            string wss = ws.Substring(ws.Length - i);
            if (wls.Equals(wss) && i > best)
            {
                best = i;
                r = ws.Substring(0, ws.Length - i) + wl;
            }
        }
        //ABXXXXCBA
        //      CBAXXXAB
        for (int i = 1; i < ws.Length; i++)
        {
            string wls = wl.Substring(wl.Length - i);
            string wss = ws.Substring(0, i);
            if (wls.Equals(wss) && i > best)
            {
                best = i;
                r = wl.Substring(0, wl.Length - i) + ws;
            }
        }
        if (best > 0) return r;

        //
        return w1 + w2;
    }
    
    static int evalSeq(int[] seq, string[] Subs)
    {
        string inter = Subs[seq[0]];
        for (int i = 1; i < Subs.Length; i++)
        {
            inter = min(inter, Subs[seq[i]]);
        }
        int length = inter.Length;
        return length;
    }

    static IEnumerable<IEnumerable<T>>
        GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });
    
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        string[] Subs = new string[N];
        for (int i = 0; i < N; i++)
        {
            string subseq = Console.ReadLine();
            Subs[i] = subseq;
        }

        //
        List<int> seed = new List<int>();
        for (int i = 0; i < N; i++) seed.Add(i);
        IEnumerable<IEnumerable<int>> lists = GetPermutations(seed, seed.Count);
        //
        int minimum = 10000;
        foreach (IEnumerable<int> list in lists)
        {
            int[] seq = list.ToArray();
            string line = "";
            foreach (int i in seq)
            {
                line += i.ToString() + " ";
            }
            line += "| ";
            int eval = evalSeq(seq, Subs);
            line += eval;
            Console.Error.WriteLine(line);
            //
            minimum = Math.Min(minimum, eval);
        }

        Console.WriteLine(minimum);
    }
}