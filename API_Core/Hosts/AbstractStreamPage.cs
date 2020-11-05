using API_Core.Hosts.CB01;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core.Hosts
{
    public abstract class AbstractStreamPage
    {
        public abstract List<Movie> searchMovie(string title);
    }
}
