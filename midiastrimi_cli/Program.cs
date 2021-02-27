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
            while (true)
            {
                Console.Write("Choose your provider: ");
                int provider = int.Parse(Console.ReadLine());
                Console.WriteLine("Search movie(1), get topList(2), series(3)");
                Console.Write("Your input: ");
                int option = int.Parse(Console.ReadLine());
                mainClass.initialize(provider);

                switch (option)
                {
                    case 1:
                        Console.Write("Please insert a movie title: ");
                        string movieTitle = Console.ReadLine();
                        List<Movie> movieRetrieved = mainClass.getMovieWithTitle(movieTitle);

                        int movieIndex = 0;
                        foreach (var x in movieRetrieved)
                        {
                            Console.WriteLine("\nChoice: " + movieIndex++.ToString());
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
                        int choiceM = int.Parse(Console.ReadLine());
                        mainClass.getStreamList(movieRetrieved[choiceM]);
                        Console.WriteLine("Here your stream links:");
                        foreach (var x in movieRetrieved[choiceM].getStreams())
                        {
                            Console.WriteLine("\t" + x.link);
                        }
                        break;
                    case 2:
                        //Test for top list CB01 actually
                        mainClass.getTopList();
                        break;
                    case 3:
                        Console.Write("Please insert a series tv title: ");
                        var toSearch = Console.ReadLine();
                        var seriesRetrieved = mainClass.getSeriesWithTitle(toSearch);
                        int seriesIndex = 0;
                        foreach (var x in seriesRetrieved)
                        {
                            Console.WriteLine("\nChoice: " + seriesIndex++.ToString());
                            Console.WriteLine("Title: " + x.getSerieTitle());
                            int charCounter = 0;
                            Console.Write('\t');                                                                              //CB01 strange whitespaces.....
                            string to_print = x.getSerieDesc().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("                                                                                               ", " ");
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
                        int choiceS = int.Parse(Console.ReadLine());
                        mainClass.getTvStreamList(seriesRetrieved[choiceS]);
                        Console.WriteLine("Here your stream links:");
                        foreach (var x in seriesRetrieved[choiceS].getStreams())
                        {
                            Console.WriteLine(x.episode);
                            foreach (var y in x.links)
                            {
                                Console.WriteLine(y.Item1 + "  " + y.Item2);
                            }
                        }
                        break;

                    default:
                        Console.WriteLine("Wrong choice!!");
                        break;
                }
            }
        }
    }
}
