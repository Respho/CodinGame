using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static void Main(string[] args)
    {
        int L = int.Parse(Console.ReadLine());
        int H = int.Parse(Console.ReadLine());
        string T = Console.ReadLine();
        Console.Error.WriteLine(L.ToString() + ", " + H.ToString() + ", " + T);
        string[] sprite = new string[H];
        string[] rows = new string[H];
        for (int i = 0; i < H; i++)
        {
            string ROW = Console.ReadLine();
            Console.Error.WriteLine(ROW);
            sprite[i] = ROW;
            rows[i] = "";
        }


        foreach (char c in T)
        {
            //
            int index = 26;
            if ((c >= 'a' && c <= 'z') | (c >= 'A' && c <= 'Z'))
            {
                index = Convert.ToInt32(c.ToString().ToUpper()[0]) - 65; 
            }

            //
            for (int j = 0; j < H; j++)
            {
                rows[j] = rows[j] + sprite[j].Substring(index * L, L);
            }
        }

        foreach (string row in rows)
        {
            Console.WriteLine(row);
        }
    }
}
