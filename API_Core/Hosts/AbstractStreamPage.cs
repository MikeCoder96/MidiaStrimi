using System.Collections.Generic;

namespace API_Core.Hosts
{
    public abstract class AbstractStreamPage
    {
        public abstract List<Movie> searchMovie(string title);
    }
}
