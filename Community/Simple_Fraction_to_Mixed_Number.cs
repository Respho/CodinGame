using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static int GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    static void Convert(string xY)
    {
        string[] tokens = xY.Split('/');
        int n = int.Parse(tokens[0]);
        int d = int.Parse(tokens[1]);
        if (d == 0)
        {
            Console.WriteLine("DIVISION BY ZERO"); return;
        }
        if (d < 0)
        {
            d *= -1; n *= -1;
        }
        if (n < 0)
        {
            Console.Write("-");
            n = -n;
        }
        if (n == 0)
        {
            Console.WriteLine("0"); return;
        }
        if (n >= d)
        {
            Console.Write(n / d);
            n = n % d;
            if (n % d != 0)
                Console.Write(" ");
        }
        if (n == 0)
        {
            Console.WriteLine(""); return;
        }
        //
        int gcd = GCD(n, d);
        n /= gcd; d /= gcd;
        Console.WriteLine(n + "/" + d);
    }

    static void Test(string[] args)
    {
        Convert("0/-174");
        Convert("-0/-174");
        Convert("2/2");
        Convert("999999/-999999");
        Convert("-999999/-999999");
        Convert("-999999/999999");
        Convert("7338162/8696067");
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string xY = Console.ReadLine();
            Convert(xY);
        }
    }
}
