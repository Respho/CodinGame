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
    public static bool CheckParentesis(string str)
    {
        if (str == "")
            return true;
    
        Stack<char> stack = new Stack<char>();
        for (int i = 0; i < str.Length; i++)
        {
            char current = str[i];
            if (current == '{' || current == '(' || current == '[')
            {
                stack.Push(current);
            }
    
    
            if (current == '}' || current == ')' || current == ']')
            {
                if (stack.Count == 0)
                    return false;
    
                char last = stack.Peek();
                if (current == '}' && last == '{' || current == ')' && last == '('
                    || current == ']' && last == '[')
                    stack.Pop();
                else 
                    return false;
            }
    
        }
    
        return stack.Count == 0;
    }

    static void Main(string[] args)
    {
        string expression = Console.ReadLine();

        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");

        Console.WriteLine(CheckParentesis(expression).ToString().ToLower());
    }
}