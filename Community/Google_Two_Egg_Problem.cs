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

        int sum = 0;
        for (int i = 1; i < 100; i++)
        {
            sum += i;
            if (sum >= N)
            {
                Console.WriteLine("" + i);
                break;
            }
        }
    }
}
