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
            List<Movie> res = mainClass.getMovieWithTitle(Console.ReadLine(), 2);
            foreach (var x in res)
            {
                Console.WriteLine(x.getMovieTitle());
                Console.WriteLine(x.getMovieDesc());
            }
        }
    }
}
