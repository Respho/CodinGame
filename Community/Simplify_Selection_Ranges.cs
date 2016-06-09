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
        string N = Console.ReadLine();
        N = N.Replace("[", "").Replace("]", "");
        string[] tokens = N.Split(',');
        int[] nums = new int[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            nums[i] = int.Parse(tokens[i]);
        }
        Array.Sort(nums);

        //
        int tempLower = nums[0];
        int tempUpper = nums[0];
        string res = "";
        res += tempLower.ToString();
        for (int i = 1; i < nums.Length; i++)
        {
            if (nums[i] - tempUpper > 1)
            {
                if (tempUpper != tempLower)
                {
                    res += tempUpper.ToString();
                }
                res += ",";
                tempUpper = nums[i];
                tempLower = nums[i];
                res += tempLower.ToString();
            }
            else
            {
                tempUpper++;
                res += "-";
            }
        }
        if (tempUpper != tempLower)
        {
            res += tempUpper.ToString();
        }
        
        //
        while (res.Contains("--"))
        {
            res = res.Replace("--", "-");
        }
        
        string final = "";
        foreach (string s in res.Split(','))
        {
            if (s.Contains("-"))
            {
                string[] parts = s.Split('-');
                int diff = int.Parse(parts[1]) - int.Parse(parts[0]);
                if (diff == 1)
                {
                    string r = s.Replace("-", ",");
                    final += r + ",";
                    continue;
                }
            }
            final += s + ",";
        }
        final = final.Trim(',');

        Console.WriteLine(final);
    }
}