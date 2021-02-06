using API_Core;
using System;
using System.Collections.Generic;

namespace midiastrimi_cli
{
    class Program
    {
        static readonly MainClass mainClass = new MainClass();
        static void Main()
        {
            Console.WriteLine("Hi, i'm MidiaStrimi CLI and i'm here to help you!\n" +
                "Before start, please chose a provider:\n1) CB01\n2) AltaDefinizione\n3) YesMovies\n");
            Console.Write("Your input: ");
            int provider = int.Parse(Console.ReadLine());
            Console.WriteLine("Search movie or get topList");
            Console.Write("Your input: ");
            int option = int.Parse(Console.ReadLine());
            mainClass.initialize(provider);
            if (option == 1)
            {
                Console.Write("Please insert a movie title: ");
                string movieTitle = Console.ReadLine();
                List<Movie> retrieved = mainClass.getMovieWithTitle(movieTitle);

                int index = 0;
                foreach (var x in retrieved)
                {
                    Console.WriteLine("\nChoice: " + index++.ToString());
                    Console.WriteLine("Title: " + x.getMovieTitle());
                    int charCounter = 0;
                    Console.Write('\t');                                                                              //CB01 strange whitespaces.....
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
                    Console.WriteLine("\n");
                }
                Console.Write("Your choice: ");
                int choiceI = int.Parse(Console.ReadLine());
                mainClass.getStreamList(retrieved[choiceI]);
                Console.WriteLine("Here your stream links:");
                foreach (var x in retrieved[choiceI].getStreams())
                {
                    Console.WriteLine("\t" + x);
                }
            }
            else
            {
                //Test for top list CB01 actually
                mainClass.getTopList();
            }
        }
    }
}
