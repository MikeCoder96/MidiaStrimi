using API_Core;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace midiastrimi_cli
{
    class Program
    {
        static void Main()
        {
            MainClass mainClass = new MainClass();
            List<Movie> res = mainClass.getMovieWithTitle(Console.ReadLine(), 1);
            foreach (var x in res)
            {
                Console.WriteLine(x.getMovieTitle());
                Console.WriteLine(x.getMovieDesc());
            }
        }
    }
}
