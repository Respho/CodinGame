using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class CodinGame
{
    //The alphabet
    static Dictionary<char, string> Alphabet = new Dictionary<char, string>();
    //Keeps all words in the dictionary for reference
    static HashSet<string> Words = new HashSet<string>();
    //Keeps the unique codes and number of words with the same code
    static Dictionary<string, ulong> Codes = new Dictionary<string, ulong>();
    //
    static List<int> CodeLengths = new List<int>();
    //
    static Dictionary<string, ulong> Cache = new Dictionary<string, ulong>();

    static void addWord(string word)
    {
        Words.Add(word);
        string code = ToCode(word);
        if (Codes.ContainsKey(code))
            Codes[code] = Codes[code] + 1;
        else
            Codes.Add(code, 1);
    }

    static string ToCode(string word)
    {
        if (Alphabet.Count == 0)
        {
            Alphabet.Add('A', ".-"); Alphabet.Add('B', "-...");
            Alphabet.Add('C', "-.-."); Alphabet.Add('D', "-..");
            Alphabet.Add('E', "."); Alphabet.Add('F', "..-.");
            Alphabet.Add('G', "--."); Alphabet.Add('H', "....");
            Alphabet.Add('I', ".."); Alphabet.Add('J', ".---");
            Alphabet.Add('K', "-.-"); Alphabet.Add('L', ".-..");
            Alphabet.Add('M', "--"); Alphabet.Add('N', "-.");
            Alphabet.Add('O', "---"); Alphabet.Add('P', ".--.");
            Alphabet.Add('Q', "--.-"); Alphabet.Add('R', ".-.");
            Alphabet.Add('S', "..."); Alphabet.Add('T', "-");
            Alphabet.Add('U', "..-"); Alphabet.Add('V', "...-");
            Alphabet.Add('W', ".--"); Alphabet.Add('X', "-..-");
            Alphabet.Add('Y', "-.--"); Alphabet.Add('Z', "--..");
        }

        StringBuilder result = new StringBuilder();
        foreach (char c in word)
        {
            result.Append(Alphabet[c]);
        }
        return result.ToString();
    }

    static ulong GetCombinations(string message)
    {
        //
        if (Cache.ContainsKey(message)) return Cache[message];
        //
        ulong total = 0;
        if (Codes.ContainsKey(message)) total+= Codes[message];
        //
        foreach (int length in CodeLengths.Where(l => l < message.Length))
        {
            string fragment = message.Substring(0, length);
            if (Codes.ContainsKey(fragment))
                total += Codes[fragment] * GetCombinations(message.Substring(fragment.Length));
        }
        //
        Console.Error.WriteLine("c(" + message + ")=" + total);
        Cache.Add(message, total);
        return total;
    }

    static void Main(string[] args)
    {
        string L = Console.ReadLine();
        int N = int.Parse(Console.ReadLine());
        for (int i = 0; i < N; i++)
        {
            string W = Console.ReadLine();
            addWord(W);
        }

        CodeLengths = Codes.Keys.OrderBy(c => c.Length).GroupBy(c => c.Length).Select(l => l.Key).ToList();

        ulong combinations = GetCombinations(L);
        Console.WriteLine(combinations);
    }
}
