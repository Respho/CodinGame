using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static int Count;
    static string[] Inputs;
    static string[] Names;

    static void Output(int depth)
    {
        string output = "";
        for (int i = 0; i <= depth; i++)
        {
            output += Names[i] + " > ";
        }
        Console.WriteLine(output.Substring(0, output.Length - 3));
    }

    static void Main(string[] args)
    {
        Count = int.Parse(Console.ReadLine());
        Inputs = new string[Count];
        Names = new string[Count];
        for (int i = 0; i < Count; i++)
        {
            string line = Console.ReadLine();
            Inputs[i] = line;
        }

        //
        int pointer = -1;
        foreach (string line in Inputs)
        {
            int level = line.Count(c => c == '.');
            string name = line.Substring(level);
            //Console.Error.WriteLine("Level " + level + " " + name);
            //
            if (level <= pointer) Output(pointer);
            //
            pointer = level;
            Names[pointer] = name;
        }

        //
        Output(pointer);
    }
}