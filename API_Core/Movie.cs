using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core
{
    public class Movie
    {
        private readonly string movieTitle;
        private readonly string movieDescription;

        public Movie(string s_movieTitle, string s_movieDescription)
        {
            this.movieTitle = s_movieTitle;
            this.movieDescription = s_movieDescription;
        }

        public string getMovieTitle()
        {
            return this.movieTitle;
        }

        public string getMovieDesc()
        {
            return this.movieDescription;
        }
    }
}
