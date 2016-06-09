using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    /*
    static long sumDivisors(long num)
    {
        List<long> Factors = getFactors(num);
        //
        List<Tuple<long, long>> PrimePowers =
            Factors.GroupBy(f => f).Select(g => new Tuple<long, long>(g.Key, g.Count())).ToList();
        //
        long result = 1;
        foreach (Tuple<long, long> t in PrimePowers)
        {
            //http://mathschallenge.net/library/number/sum_of_divisors
            //func(p^a) = (p^(a + 1) - 1) / (p - 1)
            long factor = t.Item1; long power = t.Item2;
            long part = Convert.ToInt64(Math.Pow(factor, power + 1) - 1) / (factor - 1);
            //func(a*b) = func(a) * func(b)
            result *= part;
        }
        return result;
    }

    static List<long> getFactors(long num)
    {
        List<long> Factors = new List<long>();
        long remainder = num; long divisor = 2;
        while (divisor * divisor <= remainder)
        {
            if (remainder % divisor == 0)
            {
                Factors.Add(divisor);
                remainder = remainder / divisor;
            }
            else divisor++;
        }
        Factors.Add(remainder);
        //
        return Factors;
    }
    */
    
    static long sd(long n)
    {
        long sum = 0;
        for (long i = 1; i <= n; i++)
        {
            sum += n / i * i;
        }

        return sum;
    }

    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine());

        Console.WriteLine(sd(n));
    }
}
