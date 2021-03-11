using System;
using System.Collections.Generic;
using System.Text;

namespace API_Core
{
    public class SerieStream
    {
        public string episode { get; set; }
        public Dictionary<string, string> links { get; set; }

        public SerieStream(string episode, Dictionary<string, string> links)
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
        public int SerieProvider { get; set; }
        public string SeriePoints { get; set; }
        public Uri pageLink { get; set; }
        public List<SerieStream> streamLinks { get; set; }

        public TvSerie(string s_SerieTitle, string s_SerieDescription, int i_SerieProvider, string s_SerieImage, Uri s_pageLink, string s_SeriePoints = "N/A")
        {
            this.SerieTitle = s_SerieTitle;
            this.SerieDescription = s_SerieDescription;
            this.pageLink = s_pageLink;
            this.SerieProvider = i_SerieProvider;
            this.SerieImage = s_SerieImage;
            this.SeriePoints = s_SeriePoints;
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

        public void addLink(string episode, Dictionary<string, string> links)
        {
            this.streamLinks.Add(new SerieStream(episode, links));
        }

        public List<SerieStream> getStreams()
        {
            return this.streamLinks;
        }
    }
}
