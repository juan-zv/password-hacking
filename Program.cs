using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;


class Program
{
    // Hides password text
    static string ReadHiddenInput()
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write("*");
            }

        } while (key.Key != ConsoleKey.Enter);

        return input.ToString();
    }

    // Guesses Password
    static void Guessing(List<string> values, string password)
    {
        // Try all single-letter guesses
        foreach (var letter in values)
        {
            if (letter == password)
            {
                Console.WriteLine($"\nYour password is {letter}");
                return;
            }
        }

        // Try all two-letter combinations
        foreach (var first in values)
        {
            foreach (var second in values)
            {
                string guess = first + second;
                if (guess == password)
                {
                    Console.WriteLine($"\nYour password is {guess}");
                    return;
                }
            }
        }

        // Try all three-letter combinations
        foreach (var first in values)
        {
            foreach (var second in values)
            {
                foreach (var third in values)
                {
                    string guess = first + second + third;
                    if (guess == password)
                    {
                        Console.WriteLine($"\nYour password is {guess}");
                        return;
                    }
                }
            }
        }

        Console.WriteLine("\nPassword not found with one or two-letter guesses.");
    }

    // Main Program
    static void Main()
    {
        // Prompt User to enter Password
        Console.Write("Hello and Welcome\n");

        // Enter Password
        string? password;

        do
        {
            Console.Write("Enter a password: ");
            password = ReadHiddenInput();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\nPlease Enter Text");
            }

        } while (string.IsNullOrWhiteSpace(password));

        // Password entered - Continue
        Stopwatch stopwatch = new Stopwatch();

        List<string> values = new List<string> {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "?", "{", "}", "/", "'","\\",",","|","+","=","\"","-","_"
        };


        int guessesTotal = 0;

        // Start timer
        stopwatch.Start();
        Guessing(values, password);
        stopwatch.Stop();

        Console.WriteLine("Elapsed time: {0} ms or {1} seconds", stopwatch.ElapsedMilliseconds, stopwatch.Elapsed.TotalSeconds);
    }
}