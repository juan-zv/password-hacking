using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Reflection.PortableExecutable;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;



// Current Program demostrates the processing speed of cpu to bruteforce a password by technique of multiprocessing and threading
class Program
{
    // Hides text as user inputs password
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


    // Multi Threading function
    static void Multi(List<string> values, int guessesTotal, string password, int startPosition, CancellationToken token, CancellationTokenSource cts)
    {
        for (int i = startPosition; i < values.Count; i++)
        {
            // stop if cancellation requested
            if (token.IsCancellationRequested)
                return;

            foreach (var second in values)
            {
                foreach (var third in values)
                {
                    foreach (var fourth in values)
                    {
                        foreach (var fifth in values)
                        {
                            foreach (var sixth in values)
                            {
                                foreach (var seventh in values)
                                {
                                    foreach (var eighth in values)
                                    {
                                        foreach (var ninth in values)
                                        {
                                            ++guessesTotal;
                                            string guess = values[i] + second + third + fourth + fifth + sixth + seventh + eighth;
                                            if (guess == password)
                                            {
                                                Console.WriteLine($"\nYour password is {guess} in {guessesTotal} trys");
                                                cts.Cancel();  // signal cancellation to all other tasks
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }



    static bool passwordFound = false;

    // Main Program
    static async Task Main()
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

        // All possible characters
        List<string> values = new List<string> {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "?", "{", "}", "/", "'","\\",",","|","+","=","\"","-","_"
        };


        // Track number of Guesses
        int guessesTotal = 0;

        // Start timer
        stopwatch.Start();
        // Guessing(values, password, guessesTotal);
        for (int length = 1; !passwordFound; length++)
        {
            GenerateCombinations("", length, values, password);
        }
        // Stop timer
        stopwatch.Stop();

        Console.WriteLine("Elapsed time: {0} ms or {1} seconds", stopwatch.ElapsedMilliseconds, stopwatch.Elapsed.TotalSeconds);


        // Multi Threading Use 
        Console.WriteLine("\n\n\nWe will now use multithreading");


        var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        List<Task> multiThread = new List<Task>();

        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 0, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 10, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 20, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 30, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 40, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 50, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 60, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 70, token, cts), token));
        multiThread.Add(Task.Run(() => Multi(values, guessesTotal, password, 80, token, cts), token));

        try
        {
            await Task.WhenAll(multiThread);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("One task completed early, others cancelled.");
        }



    }

    // Recursive call to try all possibilities
    static void GenerateCombinations(string current, int maxLength, List<string> vaules, string password)
    {
        if (passwordFound) return;

        if (current.Length == maxLength)
        {
            Console.WriteLine($"Trying: {current}");
            if (current == password)
            {
                Console.WriteLine($"Password found: {current}");
                passwordFound = true;
            }
            return;
        }

        foreach (string c in vaules)
        {
            GenerateCombinations(current + c, maxLength, vaules, password);
        }
    }
}