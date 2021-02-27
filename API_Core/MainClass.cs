using API_Core.Hosts;
using System.Collections.Generic;

namespace API_Core
{
    public class MainClass
    {
        private AbstractStreamPage providerChoosed;
        private int option_selected;

        public void initialize(int option)
        {
            if (providerChoosed == null)
            {
                providerChoosed = HostChooser.chooseWebsite(option);
                option_selected = option;
            }
            else if (option != option_selected)
            {
                providerChoosed = HostChooser.chooseWebsite(option);
                option_selected = option;
            }
        }

        public List<Movie> getMovieWithTitle(string movie)
        {
            var res = providerChoosed.searchMovie(movie);
            return res;
        }

        public List<TvSerie> getSeriesWithTitle(string serie)
        {
            var res = providerChoosed.searchTvSeries(serie);
            return res;
        }

        public void getStreamList(Movie movie)
        {
            providerChoosed.retrieveStreamLinks(movie);
        }

        public void getTvStreamList(TvSerie serie)
        {
            providerChoosed.retrieveTvStreamLinks(serie);
        }

        public List<Movie> getTopList()
        {
            return providerChoosed.retreiveTopMovies();
        }
    }
}
