using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
    {
        {'I', 1},
        {'V', 5},
        {'X', 10},
        {'L', 50},
        {'C', 100},
        {'D', 500},
        {'M', 1000}
    };

    static int RomanToInteger(string roman)
    {
        int number = 0;
        for (int i = 0; i < roman.Length; i++)
        {
            if (i + 1 < roman.Length && RomanMap[roman[i]] < RomanMap[roman[i + 1]])
            {
                number -= RomanMap[roman[i]];
            }
            else
            {
                number += RomanMap[roman[i]];
            }
        }
        return number;
    }

    static string IntToRoman(int num)
    {
        List<Tuple<string, int>> conv = new List<Tuple<string, int>>()
        {
            new Tuple<string, int>("M", 1000),
            new Tuple<string, int>("CM", 900),
            new Tuple<string, int>("D", 500),
            new Tuple<string, int>("CD", 400),
            new Tuple<string, int>("C", 100),
            new Tuple<string, int>("XC", 90),
            new Tuple<string, int>("L", 50),
            new Tuple<string, int>("XL", 40),
            new Tuple<string, int>("X", 10),
            new Tuple<string, int>("IX", 9),
            new Tuple<string, int>("V", 5),
            new Tuple<string, int>("IV", 4),
            new Tuple<string, int>("I", 1)
        };

        int remainder = num;
        string result = "";
        foreach (Tuple<string, int> c in conv)
        {
            while (remainder >= c.Item2)
            {
                result += c.Item1;
                remainder -= c.Item2;
            }
        }
        return result;
    }

    static void Main(string[] args)
    {
        string rom1 = Console.ReadLine();
        string rom2 = Console.ReadLine();

        int ans = RomanToInteger(rom1) + RomanToInteger(rom2);

        Console.WriteLine(IntToRoman(ans));
    }
}
