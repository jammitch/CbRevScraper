using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevScraper
{
    internal class MusicDetail
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string BPM { get; set; }
        public IDictionary<ChartDifficulty, ChartScore> Scores;

        private MusicDetail()
        {
            Scores = new Dictionary<ChartDifficulty, ChartScore>();
        }

        public static MusicDetail ParseFromElement(HtmlNode element, string id)
        {
            MusicDetail detail = new MusicDetail();
            detail.Id = id;

            HtmlNode container = element.ChildNodes[3];
            detail.Title = HtmlEntity.DeEntitize(container.ChildNodes[1].InnerText.Trim());
            detail.Artist = HtmlEntity.DeEntitize(container.ChildNodes[3].InnerText.Trim());
            detail.BPM = HtmlEntity.DeEntitize(container.ChildNodes[5].SelectSingleNode("text()").InnerText.Trim());

            return detail;
        }

        public void AddChart(ChartScore score)
        {
            Scores.Add(score.Difficulty, score);
        }

        public static void SerializeHeadersToCSV(StreamWriter stream)
        {
            stream.Write(nameof(Id));
            stream.Write(",");
            stream.Write(nameof(Title));
            stream.Write(",");
            stream.Write(nameof(Artist));
            stream.Write(",");
            stream.Write(nameof(BPM));
            stream.Write(",");
            stream.Write(nameof(ChartScore.Difficulty));
            stream.Write(",");
            stream.Write(nameof(ChartScore.Level));
            stream.Write(",");
            stream.Write(nameof(ChartScore.NoteCount));
            stream.Write(",");
            stream.Write(nameof(ChartScore.HighScore));
            stream.Write(",");
            stream.Write(nameof(ChartScore.ClearRate));
            stream.Write(",");
            stream.Write(nameof(ChartScore.RankPoints));
            stream.Write(",");
            stream.Write(nameof(ChartScore.Grade));
            stream.Write(",");
            stream.Write(nameof(ChartScore.ClearType));
            stream.Write(",");
            stream.Write(nameof(ChartScore.IsFullCombo));
            stream.WriteLine();
        }

        public void SerializeToCSV(StreamWriter stream)
        {
            foreach (ChartScore chart in Scores.Values)
            {
                stream.Write(Id);
                stream.Write(",");
                stream.Write(Title);
                stream.Write(",");
                stream.Write(Artist);
                stream.Write(",");
                stream.Write(BPM);
                stream.Write(",");
                stream.Write(chart.Difficulty.ToString());
                stream.Write(",");
                stream.Write(chart.Level);
                stream.Write(",");
                stream.Write(chart.NoteCount);
                stream.Write(",");
                stream.Write(chart.HighScore);
                stream.Write(",");
                stream.Write(chart.ClearRate);
                stream.Write(",");
                stream.Write(chart.RankPoints);
                stream.Write(",");
                stream.Write(chart.Grade);
                stream.Write(",");
                stream.Write(chart.ClearType.ToString());
                stream.Write(",");
                stream.Write(chart.IsFullCombo.ToString());
                stream.WriteLine();
            }
        }
    }
}
