using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevScraper
{
    internal class ChallengeSongScore
    {
        public string SongName { get; set; }
        public string SongArtist { get; set; }
        public int SongLevel { get; set; }
        public int SongClearRate { get; set; }
        public int SongScore { get; set; }
        public int SongGradeValue { get; set; }
        public bool IsSongFullCombo { get; set; }

        private ChallengeSongScore()
        {
        }

        public string SongGrade
        {
            get
            {
                return GradeHelper.GetGrade(SongGradeValue);
            }
        }

        public static ChallengeSongScore ParseFromElement(HtmlNode element)
        {
            ChallengeSongScore score = new ChallengeSongScore();

            HtmlNode missionDetail = element.ChildNodes[5];
            score.SongName = HtmlEntity.DeEntitize(missionDetail.ChildNodes[1].InnerText.Trim());
            score.SongArtist = HtmlEntity.DeEntitize(missionDetail.ChildNodes[3].InnerText.Trim());
            if (missionDetail.ChildNodes.Count == 7)
            {
                score.IsSongFullCombo = true;
            }

            HtmlNode scoreList = element.ChildNodes[7];
            score.SongLevel = int.Parse(scoreList.ChildNodes[3].InnerText.Trim());
            score.SongClearRate = int.Parse(scoreList.ChildNodes[7].InnerText.Trim().TrimEnd('%'));
            score.SongScore = int.Parse(scoreList.ChildNodes[11].InnerText.Trim());

            HtmlNode gradeContainer = element.ChildNodes[9];
            string gradeImageUrl = gradeContainer.ChildNodes[1].Attributes["src"].Value;
            score.SongGradeValue = GradeHelper.GetGradeFromImageUrl(gradeImageUrl);

            return score;
        }
    }
}
