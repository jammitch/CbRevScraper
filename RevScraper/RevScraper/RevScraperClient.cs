using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RevScraper
{
    class RevScraperClient : WebClient
    {
        public const double ThrottleRate = 2.0;
        private static readonly Uri _baseUri = new Uri("https://rev-srw.ac.capcom.jp/", UriKind.Absolute);
        private static readonly Regex _idRegex = new Regex("^https://rev-srw.ac.capcom.jp/.*/(.*)$");

        public bool IsLoggedIn { get; private set; }
        private DateTime LastRequest { get; set; }

        public CookieContainer CookieContainer { get; private set; }

        public RevScraperClient()
        {
            LastRequest = DateTime.Now.AddSeconds(-10);
            CookieContainer = new CookieContainer();
            this.Encoding = Encoding.UTF8;
        }

        public bool Login(string banapassId, string pin)
        {
            NameValueCollection postData = new NameValueCollection();
            postData.Add("ac", banapassId);
            postData.Add("passwd", pin);

            byte[] responseBytes = UploadValues(new Uri(_baseUri, "/webloginconfirm"), postData);
            string responseString = Encoding.UTF8.GetString(responseBytes);

            CookieCollection cookies = CookieContainer.GetCookies(_baseUri);
            IsLoggedIn = (cookies["_rst"] != null);
            return IsLoggedIn;
        }

        public IReadOnlyCollection<Uri> GetSongUris()
        {
            if (!IsLoggedIn)
            {
                return null;
            }

            string responseString = DownloadString(new Uri(_baseUri, "/playdatamusic"));

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//div[@class='pdMusicData']/a");
            return (
                from node in nodes
                select new Uri(_baseUri, node.Attributes["href"].Value)
            ).ToList();
        }

        public MusicDetail GetMusicDetail(Uri uri)
        {
            string id = _idRegex.Match(uri.ToString()).Groups[1].Value;
            string responseString = DownloadString(uri);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNode musicDetailElement = document.DocumentNode.SelectNodes("//div[contains(@class, 'pdMusicDetail')]")[0];
            MusicDetail musicDetail = MusicDetail.ParseFromElement(musicDetailElement, id);

            HtmlNodeCollection charts = document.DocumentNode.SelectNodes("//div[@class='pdm-result']");
            foreach (HtmlNode chartElement in charts)
            {
                ChartScore chart = ChartScore.ParseFromElement(chartElement);
                musicDetail.AddChart(chart);
            }

            return musicDetail;
        }

        public IReadOnlyCollection<Uri> GetCourseUris()
        {
            if (!IsLoggedIn)
            {
                return null;
            }

            string responseString = DownloadString(new Uri(_baseUri, "/playdatachallenge"));

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//a[@class='c-event__item']");
            return (
                from node in nodes
                select new Uri(_baseUri, node.Attributes["href"].Value)
            ).ToList();
        }

        public ChallengeCourse GetChallengeCourse(Uri uri)
        {
            string id = _idRegex.Match(uri.ToString()).Groups[1].Value;
            string responseString = DownloadString(uri);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNode challengeCourseElement = document.DocumentNode.SelectNodes("//div[contains(@class, 'pdBlock')]")[0];
            ChallengeCourse challengeCourse = ChallengeCourse.ParseFromElement(challengeCourseElement, id);

            HtmlNodeCollection songs = document.DocumentNode.SelectNodes("//div[contains(@class, 'chMissionBlock')]");
            foreach (HtmlNode songElement in songs)
            {
                ChallengeSongScore song = ChallengeSongScore.ParseFromElement(songElement);
                challengeCourse.Scores.Add(song);
            }

            return challengeCourse;
        }

        public IReadOnlyCollection<Uri> GetEventUris()
        {
            if (!IsLoggedIn)
            {
                return null;
            }

            string responseString = DownloadString(new Uri(_baseUri, "/playdataevent"));

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//a[@class='c-event__item']");
            return (
                from node in nodes
                select new Uri(_baseUri, node.Attributes["href"].Value)
            ).ToList();
        }

        public Event GetEvent(Uri uri)
        {
            string id = _idRegex.Match(uri.ToString()).Groups[1].Value;
            string responseString = DownloadString(uri);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(responseString);

            HtmlNode eventElement = document.DocumentNode.SelectNodes("//div[contains(@class, 'pdBlock')]")[0];
            Event eventObject = Event.ParseFromElement(eventElement, id);

            HtmlNodeCollection songs = document.DocumentNode.SelectNodes("//div[contains(@class, 'evMissionBlock')]");
            if (songs != null)
            {
                foreach (HtmlNode songElement in songs)
                {
                    EventSong song = EventSong.ParseFromElement(songElement);
                    eventObject.Songs.Add(song);
                }
            }

            return eventObject;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            // Intentionally slow down.
            double timeToWait = (ThrottleRate - (DateTime.Now - LastRequest).TotalSeconds);
            if (timeToWait > 0)
            {
                Thread.Sleep((int)(timeToWait * 1000));
            }

            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;

            LastRequest = DateTime.Now;
            return request;
        }
    }
}
