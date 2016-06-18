using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    //The Raw Data
    static Tuple<int, int>[] Data = null;
    //The derivatives at each point
    static double[] Delta = null;

    static Dictionary<int, string> Complexities = new Dictionary<int, string>
        {
            {1, "O(1)"}, {2, "O(log n)"}, {3, "O(n)"}, {4, "O(n log n)"},
            {5, "O(n^2)"}, {6, "O(n^2 log n)"}, {7, "O(n^3)"}, {8, "O(2^n)"}
        };

    static double getPoint(int input, int function)
    {
        switch (function)
        {
            case 2: //O(log n)
                return Math.Log(input, Math.E);
            case 3: //O(n)
                return 1.0 * input;
            case 4: //O(n log n)
                return 1.0 * input * Math.Log(input, Math.E);
            case 5: //O(n^2)
                return 1.0 * input * input;
            case 6: //O(n^2 log n)
                return 1.0 * input * input * Math.Log(input, Math.E);
            case 7: //O(n^3)
                return 1.0 * input * input * input;
            case 8: //O(2^n)
                return Math.Pow(2.0, 1.0 * input);
        }
        return 1.0;
    }

    static void process()
    {
        double check = 1.0 * Data[Data.Length - 1].Item2 / Data[0].Item2;
        if (check < 1.5)
        {
            Console.WriteLine(Complexities[1]);
            return;
        }

        List<Tuple<string, double>> Ratings = new List<Tuple<string, double>>();
        double[] delta = new double[Data.Length - 1];
        for (int o = 2; o <= 8; o++)
        {
            //Calculate the ideal curve
            for (int i = 1; i <= delta.Length; i++)
            {
                double y2 = getPoint(Data[i].Item1, o);
                double y1 = getPoint(Data[i - 1].Item1, o);
                double d = 1.0 * (y2 - y1) / (Data[i].Item1 - Data[i - 1].Item1);
                delta[i - 1] = d;
            }
            //Skip checking of case if it goes out of bounds
            if (delta.ToList().Exists(d => double.IsNaN(d))) continue;

            //Scale the derivatives to match the input
            double scale = delta[delta.Length - 1] / Delta[Delta.Length - 1];
            scale *= delta[delta.Length - 2] / Delta[Delta.Length - 2];
            scale *= delta[delta.Length - 3] / Delta[Delta.Length - 3];
            scale = Math.Pow(scale, 1.0 / 3);
            for (int i = 0; i < delta.Length; i++)
                delta[i] = delta[i] / scale;

            //Calculate the correlation
            double sumDiff = 1;
            for (int i = 0; i < Delta.Length; i++)
                sumDiff *= Math.Max(Math.Abs(delta[i] / Delta[i]), Math.Abs(Delta[i] / delta[i]));

            //
            Ratings.Add(new Tuple<string, double>(Complexities[o], sumDiff));
        }

        //
        string result = Ratings.OrderBy(r => r.Item2).First().Item1;
        Console.WriteLine(result);
    }

    static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        Data = new Tuple<int, int>[N];
        Delta = new double[N - 1];
        for (int i = 0; i < N; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int num = int.Parse(inputs[0]);
            int t = int.Parse(inputs[1]);
            //
            Data[i] = new Tuple<int, int>(num, t);
            //
            if (i > 0)
            {
                double d = 1.0 * (Data[i].Item2 - Data[i - 1].Item2) / (Data[i].Item1 - Data[i - 1].Item1);
                Delta[i - 1] = d;
            }
        }

        process();

        //Console.WriteLine("answer");
    }
}