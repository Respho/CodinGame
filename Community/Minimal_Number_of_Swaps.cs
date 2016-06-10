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
        int n = int.Parse(Console.ReadLine());
        string[] inputs = Console.ReadLine().Split(' ');
        int[] nums = new int[n];
        for (int i = 0; i < n; i++)
        {
            int x = int.Parse(inputs[i]);
            nums[i] = x;
        }

        int p1 = 0, p0 = n - 1;
        int swaps = 0;
        while (p1 < p0)
        {
            while (p1 < n && nums[p1] == 1) p1++;
            while (p0 >= 0 && nums[p0] == 0) p0--;
            if (p1 < p0)
            {
                nums[p1] = 1; nums[p0] = 0;
                swaps++;
            }
        }

        Console.WriteLine(swaps);
    }
}