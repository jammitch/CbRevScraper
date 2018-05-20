using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RevScraper
{
    internal enum ChartDifficulty
    {
        Unknown = 0,
        Easy = 1,
        Standard = 2,
        Hard = 3,
        Master = 4,
        Unlimited = 5
    }

    internal enum ChartClearType
    {
        Unknown = 0,
        Unplayed = 1,
        Failed = 2,
        Cleared = 3,
        Survival = 4,
        Ultimate = 5
    }

    internal class ChartScore
    {
        public ChartDifficulty Difficulty { get; set; }
        public int Level { get; set; }
        public int NoteCount { get; set; }

        public int HighScore { get; set; }
        public decimal ClearRate { get; set; }
        public decimal RankPoints { get; set; }

        public int GradeValue { get; set; }
        public ChartClearType ClearType { get; set; }
        public bool IsFullCombo { get; set; }

        private ChartScore()
        {

        }

        public string Grade
        {
            get
            {
                return GradeHelper.GetGrade(GradeValue);
            }
        }

        public static ChartScore ParseFromElement(HtmlNode element)
        {
            ChartScore score = new ChartScore();

            HtmlNode resultHead = element.ChildNodes[1]; // pdm-resultHead
            foreach(ChartDifficulty difficulty in Enum.GetValues(typeof(ChartDifficulty)))
            {
                if (resultHead.HasClass(difficulty.ToString().ToLower()))
                {
                    score.Difficulty = difficulty;
                    break;
                }
            }

            score.Level = int.Parse(resultHead.ChildNodes[3].SelectSingleNode("text()").InnerText.Trim());
            score.NoteCount = int.Parse(resultHead.ChildNodes[5].SelectSingleNode("text()").InnerText.Trim());

            HtmlNode leftResult = element.ChildNodes[3].ChildNodes[1]; // pdResultList
            score.HighScore = int.Parse(leftResult.ChildNodes[3].InnerText.Trim());
            score.ClearRate = decimal.Parse(leftResult.ChildNodes[7].InnerText.Trim().TrimEnd('%'));

            decimal rankPoint;
            if (decimal.TryParse(leftResult.ChildNodes[11].InnerText.Trim(), out rankPoint))
            {
                score.RankPoints = rankPoint;
            }
            else
            {
                score.RankPoints = 0;
            }

            HtmlNode rightResult = element.ChildNodes[5].ChildNodes[1]; // pdResultIco

            HtmlNode clearContainer = rightResult.ChildNodes[1]; // li class=clear
            if (clearContainer.ChildNodes.Count > 1)
            {
                HtmlNode clearImage = clearContainer.ChildNodes[1].ChildNodes[1];
                string clearImageUrl = clearImage.Attributes["src"].Value;
                if (clearImageUrl.StartsWith("https://rev-srw.ac.capcom.jp/assets/common/img_common/bnr_ULTIMATE_CLEAR.png"))
                {
                    score.ClearType = ChartClearType.Ultimate;
                }
                else if (clearImageUrl.StartsWith("https://rev-srw.ac.capcom.jp/assets/common/img_common/bnr_SURVIVAL_CLEAR.png"))
                {
                    score.ClearType = ChartClearType.Survival;
                }
            }

            HtmlNode gradeContainer = rightResult.ChildNodes[3]; // li class=grade
            string gradeImageUrl = gradeContainer.ChildNodes[1].Attributes["src"].Value;
            score.GradeValue = GradeHelper.GetGradeFromImageUrl(gradeImageUrl);

            if (rightResult.ChildNodes.Count == 7)
            {
                score.IsFullCombo = true;
            }

            if (score.ClearType == ChartClearType.Unknown)
            {
                if (score.GradeValue == 11)
                {
                    score.ClearType = ChartClearType.Unplayed;
                }
                else if (score.GradeValue == 10)
                {
                    score.ClearType = ChartClearType.Failed;
                }
                else
                {
                    score.ClearType = ChartClearType.Cleared;
                }
            }

            return score;
        }
    }
}
