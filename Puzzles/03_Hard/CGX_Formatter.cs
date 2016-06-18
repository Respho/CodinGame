using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static string CGX = "";

    static string process()
    {
        //
        CGX = CGX.Trim();
        //
        List<string> tokens = new List<string>();
        string tempString = "";
        string tempPrim = "";
        foreach (char c in CGX)
        {
            //Busy reading a string
            if (!string.IsNullOrEmpty(tempString))
            {
                //Close a string
                if (c == '\'')
                {
                    tokens.Add(tempString + c);
                    tempString = "";
                    continue;
                }
                //Append to string
                tempString += c;
                continue;
            }
            //Start a string
            if (c == '\'')
            {
                tempString += c;
                continue;
            }
            //
            if (c == '(' || c == ')' || c == '=' || c == ';')
            {
                if (!string.IsNullOrEmpty(tempPrim))
                {
                    tokens.Add(tempPrim);
                    tempPrim = "";
                }
                tokens.Add("" + c);
                continue;
            }
            if (string.IsNullOrWhiteSpace("" + c)) continue;
            //Prim
            tempPrim += c;
        }
        if (!string.IsNullOrEmpty(tempPrim))
        {
            tokens.Add(tempPrim);
            tempPrim = "";
        }

        string total = "";
        foreach (string line in tokens)
        {
            if (line.Equals("("))
                total += "\n" + line + "\n";
            else if (line.Equals(";"))
                total += line + "\n";
            else if (line.StartsWith(")"))
                total += "\n" + line;
            else if (!string.IsNullOrEmpty(line))
                total += line;
        }

        string[] lines = total.Split('\n');
        int depth = 0;
        foreach (string l in lines)
        {
            if (l.Trim().Length == 0) continue;
            if (l.StartsWith(")")) depth--;
            Console.WriteLine(new string(' ', depth * 4) + l.Trim());
            if (l.Equals("(")) depth++;
        }

        return CGX;
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string cGXLine = Console.ReadLine();
            CGX += cGXLine;
        }

        process();
    }
}