using API_Core.Hosts;
using System.Collections.Generic;

namespace API_Core
{
    public class MainClass
    {
        private AbstractStreamPage providerChoosed;

        public void initialize(int option)
        {
            if (providerChoosed == null)
                providerChoosed = HostChooser.chooseWebsite(option);
        }

        public List<Movie> getMovieWithTitle(string movie)
        {
            var res = providerChoosed.searchMovie(movie);
            return res;
        }

        public void getStreamList(Movie movie)
        {
            providerChoosed.retrieveStreamLinks(movie);
        }

        public List<Movie> getTopList()
        {
            return providerChoosed.retreiveTopMovies();
        }
    }
}
