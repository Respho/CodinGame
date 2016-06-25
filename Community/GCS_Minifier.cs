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
        string input = "";
        for (int i = 0; i < N; i++)
        {
            string cGSContent = Console.ReadLine();
            input += cGSContent;
        }
        //
        StringBuilder sb = new StringBuilder();
        bool isString = false; bool isVariable = false;
        List<string> variables = new List<string>();
        string variableName = "";
        foreach (char c in input)
        {
            if (c == '\'') isString = !isString;
            if (c == ' ' && !isString) continue;
            if (c == '$' && !isString)
            {
                if (!isVariable)
                {
                    variableName = "";
                    isVariable = true;
                }
                else if (isVariable)
                {
                    if (!variables.Contains(variableName))
                        variables.Add(variableName);
                    isVariable = false;
                }
            }
            if (isVariable) variableName += c;
            sb.Append(c);
        }
        //
        string minified = sb.ToString();
        char subs = 'a';
        foreach (string v in variables)
        {
            Console.Error.WriteLine("v " + v);
            minified = minified.Replace(v + "$", "$" + subs + "$");
            subs++;
        }
        //
        Console.WriteLine(minified);
    }
}