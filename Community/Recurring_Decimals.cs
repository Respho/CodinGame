using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static string fractionToDecimal(int numerator, int denominator)
    {
        if (numerator == 0)
        {
            return "0";
        }

        var res = new StringBuilder();
        var hashtable = new Dictionary<long, int>();
        var sign = (numerator > 0 && denominator < 0) || (numerator < 0 && denominator > 0);
        var denominatorlong = Math.Abs((long)denominator);
        var numeratorlong = Math.Abs((long)numerator);

        // before .
        res.Append(numeratorlong / denominatorlong);

        var remain = numeratorlong % denominatorlong;

        // add .
        if (remain != 0)
        {
            res.Append('.');
        }

        // after .
        var count = res.Length;
        while (remain != 0)
        {
            hashtable.Add(remain, count++);
            res.Append((remain * 10) / denominatorlong);
            var nextRemain = (remain * 10) % denominatorlong;
            if (hashtable.ContainsKey(nextRemain))
            {
                res.Insert(hashtable[nextRemain], '(');
                res.Append(')');
                break;
            }

            remain = nextRemain;
        }
        if (sign)
        {
            res.Insert(0, '-');
        }
        return res.ToString();
    }
        
    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine());

        Console.WriteLine(fractionToDecimal(1, n));
    }
}