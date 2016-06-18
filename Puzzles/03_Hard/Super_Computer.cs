using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    struct Job { public int StartDay, EndDay; }
    static List<Job> Jobs = new List<Job>();
    
    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int J = int.Parse(inputs[0]);
            int D = int.Parse(inputs[1]);
            Jobs.Add(new Job(){ StartDay = J, EndDay = J + D });
        }

        Jobs = Jobs.OrderBy(j => j.EndDay).ToList();
        //int s = Jobs.OrderBy(j => j.StartDay).First().StartDay;
        int s = 0, count = 0;
        foreach (Job j in Jobs)
        {
            if (j.StartDay >= s)
            {
                s = j.EndDay;
                count++;
            }
        }

        Console.WriteLine(count);
    }
}