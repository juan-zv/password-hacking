using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static bool passwordFound = false;
    static object lockObj = new();

    
    static async Task Main()
    {
        Console.WriteLine("Hello and Welcome");

        // Prompt User to enter password (text is hidden)
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

        // Character set
        List<string> values = new List<string> {
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "1","2","3","4","5","6","7","8","9","0","!","@","#","$","%","^","&","*","(",")","?","{","}","/","'",
            "\\",",","|","+","=","\"","-","_"
        };
        
        // Stopwatch for single thread
        Stopwatch stopwatch = new Stopwatch();

        // Start timer
        stopwatch.Start();
        // Guessing(values, password, guessesTotal);
        for (int length = 1; !passwordFound; length++)
        {
            GenerateCombinations("", length, values, password);
        }
        // Stop timer / Present the time it took to solve guess the password
        stopwatch.Stop();
        Console.WriteLine("\nElapsed time: {0} ms ({1} seconds)", stopwatch.ElapsedMilliseconds, stopwatch.Elapsed.TotalSeconds);

        // Reset to false for multithreading
        passwordFound = false;

        Console.WriteLine("\nNow using multithreading to find the password...");

        Stopwatch stopwatch2 = new Stopwatch();
        stopwatch2.Start();

        // Cancelation token for threads to stop once the password is found
        var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Create threads
        List<Task> multiThread = new List<Task>();
        int numThreads = Environment.ProcessorCount;    // Threads created based on CPU cores (ex. 8 cores = 8 threads)
        int charsPerThread = values.Count / numThreads; // Split up characterset evenly across threads

        for (int i = 0; i < numThreads; i++)
        {
            int startIndex = i * charsPerThread;
            int endIndex = (i == numThreads - 1) ? values.Count : startIndex + charsPerThread;

            multiThread.Add(Task.Run(() => Multi(values, password!, startIndex, endIndex, token, cts), token));
        }

        try
        {
            await Task.WhenAll(multiThread);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("One thread found the password, others were cancelled.");
        }

        stopwatch2.Stop();
        Console.WriteLine("\nElapsed time: {0} ms ({1} seconds)", stopwatch2.ElapsedMilliseconds, stopwatch2.Elapsed.TotalSeconds);
    }



    // Recursive call to try all possibilities for single thread
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



    // Recursive call to try all possibilities for multi thread
    static void BruteForce(string current, List<string> values, int maxLength, string password, CancellationToken token, CancellationTokenSource cts)
    {
        if (token.IsCancellationRequested || passwordFound) return;

        if (current.Length == maxLength)
        {
            if (current == password)
            {
                lock (lockObj)
                {
                    if (!passwordFound)
                    {
                        passwordFound = true;
                        Console.WriteLine($"\n[Thread {Thread.CurrentThread.ManagedThreadId}] Password found: {current}");
                        cts.Cancel();
                    }
                }
            }
            return;
        }

        foreach (string c in values)
        {
            if (token.IsCancellationRequested || passwordFound) return;
            BruteForce(current + c, values, maxLength, password, token, cts);
        }
    }



    // Create Multiple threads starting at different locations
    static void Multi(List<string> values, string password, int startIndex, int endIndex, CancellationToken token, CancellationTokenSource cts)
    {
        int length = 1;

        try
        {
            while (!passwordFound && !token.IsCancellationRequested)
            {
                for (int i = startIndex; i < endIndex && !passwordFound && !token.IsCancellationRequested; i++)
                {
                    BruteForce(values[i], values, length, password, token, cts);
                }
                length++;
            }
        }
        catch (OperationCanceledException)
        {
            // Task was cancelled
        }
    }



    // Hide user input
    static string ReadHiddenInput()
    {
        string input = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                input += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[0..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return input;
    }
}
