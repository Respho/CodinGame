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
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);
        char[,] map = new char[width, height];
        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine();
            for (int j = 0; j < line.Length; j++)
            {
                map[j, i] = line[j];
            }
        }

        for (int x = 0; x < width; x++)
        {
            int count = 0;
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == '#') count++;
            }
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (height - y <= count) ? '#' : '.';
            }
        }

        for (int j = 0; j < height; j++)
        {
            string l = "";
            for (int i = 0; i < width; i++)
            {
                l += map[i, j];
            }
            Console.WriteLine(l);
        }
    }
}
