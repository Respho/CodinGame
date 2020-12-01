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
        var numbers = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            var row = Console.ReadLine();
            Console.Error.WriteLine(row);
            string[] inputs = row.Split(' ');
            for (int j = 0; j < 4; j++)
            {
                int CNT = int.Parse(inputs[j]);
                numbers.Add(CNT);
            }
        }
        var line = Console.ReadLine();
        Console.Error.WriteLine(line);
        int fours = int.Parse(line);

        //
        int totalScore = 0;
        foreach (int t in numbers.Where(n => n != 0))
        {
            int score = (Convert.ToInt32(Math.Log2(t)) - 1) * t;
            Console.Error.WriteLine("Tile " + t + " is worth " + score);
            totalScore += score;
        }
        totalScore -= fours * 4;

        //
        int totalSum = numbers.Sum();
        int turns = (totalSum - fours * 2) / 2 - 2;
        Console.Error.WriteLine("Tiles sum " + totalSum);

        //
        Console.WriteLine(totalScore);
        Console.WriteLine(turns);
    }
}
