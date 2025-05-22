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
    static void Guessing(List<string> values, string password, int guessIndex, int guessesTotal)
    {
        string guess = values[guessIndex];

        if (password == guess)
        {
            Console.WriteLine($"\nYour password is {guess}");
        }

        else
        {
            Guessing(values, password, guessIndex + 1, guessesTotal + 1);
        }
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
                Console.WriteLine("Please Enter Text");
            }

        } while (string.IsNullOrWhiteSpace(password));

        // Password entered - Continue
        Stopwatch stopwatch = new Stopwatch();

        List<string> values = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };


        int guessesTotal = 0;
        int guessIndex = 0;

        // Start timer
        stopwatch.Start();
        Guessing(values, password, guessIndex, guessesTotal);
        stopwatch.Stop();

        Console.WriteLine("Elapsed time: {0} ms or {1} seconds", stopwatch.ElapsedMilliseconds, stopwatch.Elapsed.TotalSeconds);
    }
}