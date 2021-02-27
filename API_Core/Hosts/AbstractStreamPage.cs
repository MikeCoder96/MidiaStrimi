using System.Collections.Generic;

namespace API_Core.Hosts
{
    public abstract class AbstractStreamPage
    {
        public abstract List<TvSerie> searchTvSeries(string title);
        public abstract List<Movie> searchMovie(string title);
        public abstract void retrieveStreamLinks(Movie movie);
        public abstract void retrieveTvStreamLinks(TvSerie serie);
        public abstract List<Movie> retreiveTopMovies();
    }
}
