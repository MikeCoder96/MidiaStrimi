using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core
{
    public class SerieStream
    {
        public string episode { get; set; }
        public List<(string,string)> links { get; set; }

        public SerieStream(string episode, List<(string, string)> links)
        {
            this.episode = episode;
            this.links = links;
        }
    }

    public class TvSerie
    {
        public string SerieTitle { get; set; }
        public string SerieDescription { get; set; }
        public string SerieImage { get; set; }
        public Uri pageLink { get; set; }
        public List<SerieStream> streamLinks { get; set; }

        public TvSerie(string s_SerieTitle, string s_SerieDescription, string s_SerieImage, Uri s_pageLink)
        {
            this.SerieTitle = s_SerieTitle;
            this.SerieDescription = s_SerieDescription;
            this.pageLink = s_pageLink;
            this.SerieImage = s_SerieImage;
            this.streamLinks = new List<SerieStream>();
        }

        public string getSerieTitle()
        {
            return this.SerieTitle;
        }

        public string getSerieImage()
        {
            return this.SerieImage;
        }

        public string getSerieDesc()
        {
            return this.SerieDescription;
        }

        public Uri getSeriePageLink()
        {
            return this.pageLink;
        }

        public void addLink(string episode, List<(string, string)> links)
        {
            this.streamLinks.Add(new SerieStream(episode, links));
        }

        public List<SerieStream> getStreams()
        {
            return this.streamLinks;
        }
    }
}
