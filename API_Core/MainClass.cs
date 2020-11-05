using API_Core.Hosts;
using System.Collections.Generic;

namespace API_Core
{
    public class MainClass
    {
        public List<Movie> getMovieWithTitle(string movie, int option)
        {
            var res = HostChooser.chooseWebsite(option).searchMovie(movie);
            return res;
        }
    }
}
