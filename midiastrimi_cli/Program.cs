using API_Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace midiastrimi_cli
{
    class Program
    {
        static MainClass mainClass = new MainClass();
        static void Main()
        {
            Console.WriteLine("Hi, i'm MidiaStrimi CLI and i'm here to help you!\n" +
                "Before start, please chose a provider:\n1) CB01\n2) AltaDefinizione\n3) YesMovies\n");
            Console.Write("Your input: ");
            int provider = int.Parse(Console.ReadLine());
            Console.Write("Please insert a movie title: ");
            string movieTitle = Console.ReadLine();
            List<Movie> retrieved = mainClass.getMovieWithTitle(movieTitle, provider);

            foreach (var x in retrieved)
            {
                Console.WriteLine("Title: " + x.getMovieTitle());
                int charCounter = 0;
                Console.Write('\t');
                string to_print = x.getMovieDesc().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("                                                                                               ", " ");
                for (int i = 0; i < to_print.Length; i++)
                {
                    Console.Write(to_print[i]);
                    charCounter++;
                    if (charCounter == 97)
                    {
                        Console.WriteLine();
                        charCounter = 0;
                        Console.Write('\t');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
