using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static string convert(long P, string C)
    {
        long modulo = Convert.ToInt64(C.Length);
        string result = "";
        while (P >= 0)
        {
            long remainder = P % modulo;
            result += C[Convert.ToInt32(remainder)];
            P -= remainder;
            P -= 1;
            if (P > 0) P /= modulo;
        }

        return result;
    }

    static void Main(string[] args)
    {
        long P = long.Parse(Console.ReadLine());
        string C = Console.ReadLine();
        Console.Error.WriteLine(P + " -> " + C);

        //
        Console.WriteLine(convert(P, C));
        return;

        //Test
        string alpha = "abcd";
        for (int i = 0; i < 30; i++)
        {
            Console.WriteLine("" + i + " -> " +
                convert(Convert.ToInt64(i), alpha));
        }
    }
}
