using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static long sumOfDigits(long n)
    {
        long temp = n;
        long sum = 0;
        while (temp > 0)
        {
            sum += temp % 10;
            temp /= 10;
        }
        return sum;
    }

    static void Main(string[] args)
    {
        long r1 = long.Parse(Console.ReadLine());
        long r2 = long.Parse(Console.ReadLine());

        while (r1 != r2)
        {
            if (r1 > r2)
                r2 += sumOfDigits(r2);
            else
                r1 += sumOfDigits(r1);
        }

        Console.WriteLine(r1);
    }
}