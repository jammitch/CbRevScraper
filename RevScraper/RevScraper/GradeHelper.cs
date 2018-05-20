using System.Text.RegularExpressions;

namespace RevScraper
{
    public static class GradeHelper
    {
        private static Regex _gradeRegex = new Regex("https://rev-srw.ac.capcom.jp/assets/common/img_common/grade_([0-9]+).");

        public static int GetGradeFromImageUrl(string gradeImageUrl)
        {
            return int.Parse(_gradeRegex.Match(gradeImageUrl).Groups[1].Value);
        }

        public static string GetGrade(int gradeValue)
        {
            switch (gradeValue)
            {
                case 0:
                    return "S++";
                case 1:
                    return "S+";
                case 2:
                    return "S";
                case 3:
                    return "A+";
                case 4:
                    return "A";
                case 5:
                    return "B+";
                case 6:
                    return "B";
                case 7:
                    return "C";
                case 8:
                    return "D";
                case 9:
                    return "E";
                case 10:
                    return "F";
                default:
                    return "-";
            }
        }
    }
}
