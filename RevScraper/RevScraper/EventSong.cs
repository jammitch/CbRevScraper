using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevScraper
{
    internal class EventSong
    {
        public string SongName { get; set; }
        public string SongArtist { get; set; }
        public int SongBestScore { get; set; }
        public int SongTotalScore { get; set; }

        private EventSong()
        {
        }

        public static EventSong ParseFromElement(HtmlNode element)
        {
            EventSong song = new EventSong();

            HtmlNode missionDetail = element.ChildNodes[3]; // evMissionDetail
            song.SongName = HtmlEntity.DeEntitize(missionDetail.ChildNodes[1].InnerText.Trim());
            song.SongArtist = HtmlEntity.DeEntitize(missionDetail.ChildNodes[3].InnerText.Trim());

            // If this is a stamp event, this won't be there.
            if (element.ChildNodes.Count >= 8)
            {
                HtmlNode score = element.ChildNodes[7]; // evmScoreList
                int songBestScore;
                if (int.TryParse(score.ChildNodes[3].InnerText.Trim(), out songBestScore))
                {
                    song.SongBestScore = songBestScore;
                }
                else
                {
                    song.SongBestScore = 0;
                }

                int songTotalScore;
                if (int.TryParse(score.ChildNodes[7].InnerText.Trim(), out songTotalScore))
                {
                    song.SongTotalScore = songBestScore;
                }
                else
                {
                    song.SongTotalScore = 0;
                }
            }

            return song;
        }
    }
}
