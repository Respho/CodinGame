using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static Dictionary<int, int> memory = new Dictionary<int, int>();

    static List<string> formulas = new List<string>();

    static int getArgValue(string arg)
    {
        if (arg.StartsWith("$"))
        {
            int index = int.Parse(arg.Replace("$", ""));
            return getCellValue(index);
        }
        return int.Parse(arg);
    }

    static int getCellValue(int cellIndex)
    {
        //
        if (memory.ContainsKey(cellIndex)) return memory[cellIndex];
        //
        string formula = formulas[cellIndex];
        string[] tokens = formula.Split(' ');
        if (tokens[0] == "VALUE")
        {
            int value = getArgValue(tokens[1]);
            memory[cellIndex] = value;
            return value;
        }
        //
        int value1 = getArgValue(tokens[1]);
        int value2 = getArgValue(tokens[2]);
        //
        int answer = 0;
        if (tokens[0] == "ADD") answer = value1 + value2;
        if (tokens[0] == "SUB") answer = value1 - value2;
        if (tokens[0] == "MULT") answer = value1 * value2;
        //
        memory[cellIndex] = answer;
        return answer;
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            var line = Console.ReadLine();
            formulas.Add(line);
            string[] inputs = line.Split(' ');
            string operation = inputs[0];
            string arg1 = inputs[1];
            string arg2 = inputs[2];
        }
        for (int i = 0; i < N; i++)
        {
            int value = getCellValue(i);
            Console.WriteLine(value);
        }
    }
}