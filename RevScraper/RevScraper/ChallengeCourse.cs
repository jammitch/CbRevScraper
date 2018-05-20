using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevScraper
{
    internal class ChallengeCourse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MaxCombo { get; set; }
        public int Score { get; set; }
        public decimal ClearRate { get; set; }
        public decimal RankPoint { get; set; }
        public IList<ChallengeSongScore> Scores { get; set; }

        private ChallengeCourse()
        {
            Scores = new List<ChallengeSongScore>();
        }

        public static ChallengeCourse ParseFromElement(HtmlNode element, string id)
        {
            ChallengeCourse course = new ChallengeCourse();

            course.Id = id;

            course.Name = HtmlEntity.DeEntitize(element.ChildNodes[1].ChildNodes[1].ChildNodes[3].InnerText.Trim());
            course.MaxCombo = int.Parse(element.ChildNodes[5].ChildNodes[3].ChildNodes[0].InnerText.Trim());
            course.Score = int.Parse(element.ChildNodes[7].ChildNodes[3].ChildNodes[0].InnerText.Trim());
            course.ClearRate = decimal.Parse(element.ChildNodes[9].ChildNodes[3].ChildNodes[0].InnerText.Trim().TrimEnd('%'));
            decimal rankPoint;
            if (decimal.TryParse(element.ChildNodes[11].ChildNodes[3].ChildNodes[0].InnerText.Trim(), out rankPoint))
            {
                course.RankPoint = rankPoint;
            }
            else
            {
                course.RankPoint = 0;
            }

            return course;
        }

        public static void SerializeHeadersToCSV(StreamWriter stream)
        {
            stream.Write(nameof(Id));
            stream.Write(",");
            stream.Write(nameof(Name));
            stream.Write(",");
            stream.Write(nameof(MaxCombo));
            stream.Write(",");
            stream.Write(nameof(Score));
            stream.Write(",");
            stream.Write(nameof(ClearRate));
            stream.Write(",");
            stream.Write(nameof(RankPoint));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongName));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongArtist));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongLevel));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongClearRate));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongScore));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.SongGrade));
            stream.Write(",");
            stream.Write(nameof(ChallengeSongScore.IsSongFullCombo));
            stream.WriteLine();
        }

        public void SerializeToCSV(StreamWriter stream)
        {
            foreach (ChallengeSongScore chart in Scores)
            {
                stream.Write(Id);
                stream.Write(",");
                stream.Write(Name);
                stream.Write(",");
                stream.Write(MaxCombo);
                stream.Write(",");
                stream.Write(Score);
                stream.Write(",");
                stream.Write(ClearRate);
                stream.Write(",");
                stream.Write(RankPoint);
                stream.Write(",");
                stream.Write(chart.SongName);
                stream.Write(",");
                stream.Write(chart.SongArtist);
                stream.Write(",");
                stream.Write(chart.SongLevel);
                stream.Write(",");
                stream.Write(chart.SongClearRate);
                stream.Write(",");
                stream.Write(chart.SongScore);
                stream.Write(",");
                stream.Write(chart.SongGrade);
                stream.Write(",");
                stream.Write(chart.IsSongFullCombo.ToString());
                stream.WriteLine();
            }
        }
    }
}
