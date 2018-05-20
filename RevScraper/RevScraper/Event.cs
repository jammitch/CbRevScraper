using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevScraper
{
    class Event
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string Dates { get; set; }
        public int Rank { get; set; }
        public int BestScores { get; set; }
        public int TotalScores { get; set; }
        public List<EventSong> Songs { get; set; }

        private Event()
        {
            Songs = new List<EventSong>();
        }

        public static Event ParseFromElement(HtmlNode element, string id)
        {
            Event eventObject = new Event();

            eventObject.Id = id;

            HtmlNode eventDetail = element.ChildNodes[1].ChildNodes[1]; // c-event__item
            eventObject.EventName = HtmlEntity.DeEntitize(eventDetail.ChildNodes[3].InnerText.Trim());
            eventObject.Dates = HtmlEntity.DeEntitize(eventDetail.ChildNodes[5].ChildNodes[1].ChildNodes[2].InnerText.Trim());

            int rank;
            if (int.TryParse(element.ChildNodes[5].ChildNodes[3].ChildNodes[0].InnerText.Trim(), out rank))
            {
                eventObject.Rank = rank;
            }
            else
            {
                eventObject.Rank = 0;
            }

            // For some events, this might just be a stamp count. I don't care enough to separate the two.
            int bestScores;
            if (int.TryParse(element.ChildNodes[7].ChildNodes[3].ChildNodes[0].InnerText.Trim(), out bestScores))
            {
                eventObject.BestScores = bestScores;
            }
            else
            {
                eventObject.BestScores = 0;
            }

            // If this is a stamp event, this won't be there.
            if (element.ChildNodes[9].ChildNodes.Count >= 4)
            {
                int totalScores;
                if (int.TryParse(element.ChildNodes[9].ChildNodes[3].ChildNodes[0].InnerText.Trim(), out totalScores))
                {
                    eventObject.TotalScores = totalScores;
                }
                else
                {
                    eventObject.TotalScores = 0;
                }
            }

            return eventObject;
        }

        public static void SerializeHeadersToCSV(StreamWriter stream)
        {
            stream.Write(nameof(Id));
            stream.Write(",");
            stream.Write(nameof(EventName));
            stream.Write(",");
            stream.Write(nameof(Dates));
            stream.Write(",");
            stream.Write(nameof(Rank));
            stream.Write(",");
            stream.Write(nameof(BestScores));
            stream.Write(",");
            stream.Write(nameof(TotalScores));
            stream.Write(",");
            stream.Write(nameof(EventSong.SongName));
            stream.Write(",");
            stream.Write(nameof(EventSong.SongArtist));
            stream.Write(",");
            stream.Write(nameof(EventSong.SongBestScore));
            stream.Write(",");
            stream.Write(nameof(EventSong.SongTotalScore));
            stream.WriteLine();
        }

        public void SerializeToCSV(StreamWriter stream)
        {
            if (Songs.Count == 0)
            {
                stream.Write(Id);
                stream.Write(",");
                stream.Write(EventName);
                stream.Write(",");
                stream.Write(Dates);
                stream.Write(",");
                stream.Write(Rank);
                stream.Write(",");
                stream.Write(BestScores);
                stream.Write(",");
                stream.Write(TotalScores);
                stream.WriteLine(",,,,");
            }
            else
            {
                foreach (EventSong song in Songs)
                {
                    stream.Write(Id);
                    stream.Write(",");
                    stream.Write(EventName);
                    stream.Write(",");
                    stream.Write(Dates);
                    stream.Write(",");
                    stream.Write(Rank);
                    stream.Write(",");
                    stream.Write(BestScores);
                    stream.Write(",");
                    stream.Write(TotalScores);
                    stream.Write(",");
                    stream.Write(song.SongName);
                    stream.Write(",");
                    stream.Write(song.SongArtist);
                    stream.Write(",");
                    stream.Write(song.SongBestScore);
                    stream.Write(",");
                    stream.Write(song.SongTotalScore);
                    stream.WriteLine();
                }
            }
        }
    }
}
